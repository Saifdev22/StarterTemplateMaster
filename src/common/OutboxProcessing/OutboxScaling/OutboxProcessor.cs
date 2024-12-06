﻿using System.Collections.Concurrent;
using System.Diagnostics;
using Dapper;
using MassTransit;
using Npgsql;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using OutboxProcessing.Outbox;

namespace OutboxProcessing.OutboxScaling;

internal sealed class OutboxProcessor(
    NpgsqlDataSource dataSource,
    IPublishEndpoint publishEndpoint,
    ILogger<OutboxProcessor> logger)
{
    private const int BatchSize = 1000;
    private static readonly ConcurrentDictionary<string, Type> TypeCache = new();

    public async Task<int> Execute(CancellationToken cancellationToken = default)
    {
        Stopwatch totalStopwatch = Stopwatch.StartNew();
        Stopwatch stepStopwatch = new();

        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(cancellationToken);

        stepStopwatch.Restart();
        List<OutboxMessage> messages = (await connection.QueryAsync<OutboxMessage>(
            """
            SELECT id AS Id, type AS Type, content AS Content
            FROM outbox_messages
            WHERE processed_on_utc IS NULL
            ORDER BY occurred_on_utc LIMIT @BatchSize
            FOR UPDATE SKIP LOCKED
            """,
            new { BatchSize },
            transaction: transaction)).AsList();
        long queryTime = stepStopwatch.ElapsedMilliseconds;

        ConcurrentQueue<OutboxUpdate> updateQueue = new();

        stepStopwatch.Restart();
        List<Task> publishTasks = messages
            .Select(message => PublishMessage(message, updateQueue, publishEndpoint, cancellationToken))
            .ToList();

        await Task.WhenAll(publishTasks);
        long publishTime = stepStopwatch.ElapsedMilliseconds;

        stepStopwatch.Restart();
        if (!updateQueue.IsEmpty)
        {
            string updateSql =
                """
                UPDATE outbox_messages
                SET processed_on_utc = v.processed_on_utc,
                    error = v.error
                FROM (VALUES
                    {0}
                ) AS v(id, processed_on_utc, error)
                WHERE outbox_messages.id = v.id::uuid
                """;

            List<OutboxUpdate> updates = [.. updateQueue];
            string valuesList = string.Join(",",
                updateQueue.Select((_, i) => $"(@Id{i}, @ProcessedOn{i}, @Error{i})"));

            DynamicParameters parameters = new();

            for (int i = 0; i < updateQueue.Count; i++)
            {
                parameters.Add($"Id{i}", updates[i].Id.ToString());
                parameters.Add($"ProcessedOn{i}", updates[i].ProcessedOnUtc);
                parameters.Add($"Error{i}", updates[i].Error);
            }

            string formattedSql = string.Format(updateSql, valuesList);

            await connection.ExecuteAsync(formattedSql, parameters, transaction: transaction);
        }
        long updateTime = stepStopwatch.ElapsedMilliseconds;

        await transaction.CommitAsync(cancellationToken);

        totalStopwatch.Stop();
        long totalTime = totalStopwatch.ElapsedMilliseconds;

        OutboxLoggers.LogProcessingPerformance(logger, totalTime, queryTime, publishTime, updateTime, messages.Count);

        return messages.Count;
    }

    private static async Task PublishMessage(
        OutboxMessage message,
        ConcurrentQueue<OutboxUpdate> updateQueue,
        IPublishEndpoint publishEndpoint,
        CancellationToken cancellationToken)
    {
        try
        {
            Type messageType = GetOrAddMessageType(message.Type);
            object deserializedMessage = JsonSerializer.Deserialize(message.Content, messageType)!;

            await publishEndpoint.Publish(deserializedMessage, cancellationToken);

            updateQueue.Enqueue(new OutboxUpdate { Id = message.Id, ProcessedOnUtc = DateTime.UtcNow });
        }
        catch (Exception ex)
        {
            updateQueue.Enqueue(
                new OutboxUpdate { Id = message.Id, ProcessedOnUtc = DateTime.UtcNow, Error = ex.ToString() });
        }
    }

    private static Type GetOrAddMessageType(string typename)
    {
        return TypeCache.GetOrAdd(typename, name => Messaging.Contracts.AssemblyReference.Assembly.GetType(name)!);
    }

    private readonly struct OutboxUpdate
    {
        public Guid Id { get; init; }
        public DateTime ProcessedOnUtc { get; init; }
        public string? Error { get; init; }
    }
}