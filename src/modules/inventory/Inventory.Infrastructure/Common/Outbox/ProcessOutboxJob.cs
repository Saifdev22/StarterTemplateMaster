using Common.Application.Clock;
using Common.Application.Database;
using Common.Application.Messaging;
using Common.Domain.Abstractions;
using Common.Infrastructure.Outbox;
using Common.Infrastructure.Serialization;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;
using System.Application.Common.Interfaces;
using System.Data.Common;
using System.Domain.Features.Tenants;

namespace Inventory.Infrastructure.Common.Outbox;

[DisallowConcurrentExecution]
internal sealed class ProcessOutboxJob(
        ITenantService tenantService,
        IDbConnectionFactory _dbConnectionFactory,
        IServiceScopeFactory serviceScopeFactory,
        IDateTimeProvider dateTimeProvider,
        IOptions<OutboxOptions> outboxOptions,
        ILogger<ProcessOutboxJob> logger) : IJob
{
    private const string ModuleName = "Inventory";

    public IServiceScopeFactory ServiceScopeFactory { get; } = serviceScopeFactory;

    public async Task Execute(IJobExecutionContext context)
    {
        List<TenantM> tenantList = await tenantService.GetAllTenants();

        foreach (TenantM tenant in tenantList)
        {
            logger.LogInformation("{Module} - Beginning to process outbox messages for tenant: {TenantDb}\"", ModuleName, tenant.DatabaseName);
            await using DbConnection connection = await _dbConnectionFactory.OpenConnectionAsync(tenant.ConnectionString);
            await using DbTransaction transaction = await connection.BeginTransactionAsync();

            IReadOnlyList<OutboxMessageResponse> outboxMessages = await GetOutboxMessagesAsync(connection, transaction);

            foreach (OutboxMessageResponse outboxMessage in outboxMessages)
            {
                Exception? exception = null;

                try
                {
                    IDomainEvent domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                            outboxMessage.Content,
                            SerializerSettings.Instance)!;

                    using IServiceScope scope = ServiceScopeFactory.CreateScope();

                    IEnumerable<IDomainEventHandler> handlers = DomainEventHandlersFactory.GetHandlers(
                            domainEvent.GetType(),
                            scope.ServiceProvider,
                            Application.AssemblyReference.Assembly);

                    foreach (IDomainEventHandler domainEventHandler in handlers)
                    {
                        await domainEventHandler.Handle(domainEvent, context.CancellationToken);
                    }
                }
                catch (Exception caughtException)
                {
                    logger.LogError(
                            caughtException,
                            "{Module} - Exception while processing outbox message {MessageId}",
                            ModuleName,
                            outboxMessage.Id);

                    exception = caughtException;
                }

                await UpdateOutboxMessageAsync(connection, transaction, outboxMessage, exception);
            }

            await transaction.CommitAsync();

            logger.LogInformation("{Module} - Completed processing outbox messages for tenant: {TenantDb}", ModuleName, tenant.DatabaseName);

        }

    }

    private async Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessagesAsync(
            DbConnection connection,
            DbTransaction transaction)
    {

        string sql =
                $"""
						SELECT TOP {outboxOptions.Value.BatchSize}
								Id AS {nameof(OutboxMessageResponse.Id)},
								Content AS {nameof(OutboxMessageResponse.Content)}
						FROM [IN].[OutboxMessages]
						WHERE ProcessedOnUtc IS NULL
						ORDER BY OccurredOnUtc
				""";

        IEnumerable<OutboxMessageResponse> outboxMessages = await connection.QueryAsync<OutboxMessageResponse>(
                sql,
                transaction: transaction);

        return outboxMessages.ToList();
    }

    private async Task UpdateOutboxMessageAsync(
            DbConnection connection,
            DbTransaction transaction,
            OutboxMessageResponse outboxMessage,
            Exception? exception)
    {
        const string sql =
                """
            UPDATE [IN].[OutboxMessages]
            SET ProcessedOnUtc = @ProcessedOnUtc,
                error = @Error
            WHERE Id = @Id
            """;

        await connection.ExecuteAsync(
                sql,
                new
                {
                    outboxMessage.Id,
                    ProcessedOnUtc = dateTimeProvider.UtcNow,
                    Error = exception?.ToString()
                },
                transaction: transaction);
    }

    internal sealed record OutboxMessageResponse(Guid Id, string Content);
}
