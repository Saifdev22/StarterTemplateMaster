using Common.Application.Clock;
using Common.Application.Database;
using Common.Application.EventBus;
using Common.Infrastructure.Inbox;
using Common.Infrastructure.Serialization;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;
using System.Application.Common.Interfaces;
using System.Data.Common;
using System.Domain.Features.Tenant;

namespace Inventory.Infrastructure.Common.Inbox;

[DisallowConcurrentExecution]
internal sealed class ProcessInboxJob(
        ITenantService tenantService,
        IDbConnectionFactory dbConnectionFactory,
        IServiceScopeFactory serviceScopeFactory,
        IDateTimeProvider dateTimeProvider,
        IOptions<InboxOptions> inboxOptions,
        ILogger<ProcessInboxJob> logger) : IJob
{
    private const string ModuleName = "Inventory";

    public async Task Execute(IJobExecutionContext context)
    {
        List<TenantM> tenantList = await tenantService.GetAllTenants();

        foreach (TenantM tenant in tenantList)
        {
            logger.LogInformation("{Module} - Beginning to process inbox messages for tenant: {TenantDb}\"", ModuleName, tenant.DatabaseName);

            await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync(tenant.ConnectionString);
            await using DbTransaction transaction = await connection.BeginTransactionAsync();

            IReadOnlyList<InboxMessageResponse> inboxMessages = await GetInboxMessagesAsync(connection, transaction);

            foreach (InboxMessageResponse inboxMessage in inboxMessages)
            {
                Exception? exception = null;

                try
                {
                    IIntegrationEvent integrationEvent = JsonConvert.DeserializeObject<IIntegrationEvent>(
                            inboxMessage.Content,
                            SerializerSettings.Instance)!;

                    using IServiceScope scope = serviceScopeFactory.CreateScope();

                    IEnumerable<IIntegrationEventHandler> handlers = IntegrationEventHandlersFactory.GetHandlers(
                            integrationEvent.GetType(),
                            scope.ServiceProvider,
                            Presentation.AssemblyReference.Assembly);

                    foreach (IIntegrationEventHandler integrationEventHandler in handlers)
                    {
                        await integrationEventHandler.Handle(integrationEvent, context.CancellationToken);
                    }
                }
                catch (Exception caughtException)
                {
                    logger.LogError(
                            caughtException,
                            "{Module} - Exception while processing inbox message {MessageId}",
                            ModuleName,
                            inboxMessage.Id);

                    exception = caughtException;
                }

                await UpdateInboxMessageAsync(connection, transaction, inboxMessage, exception);
            }

            await transaction.CommitAsync();

            logger.LogInformation("{Module} - Completed processing inbox messages for tenant: {TenantDb}", ModuleName, tenant.DatabaseName);
        }


    }

    private async Task<IReadOnlyList<InboxMessageResponse>> GetInboxMessagesAsync(
            DbConnection connection,
            DbTransaction transaction)
    {
        string sql =
                $"""
             SELECT TOP {inboxOptions.Value.BatchSize}
                Id AS {nameof(InboxMessageResponse.Id)},
                Content AS {nameof(InboxMessageResponse.Content)}
             FROM [IN].[InboxMessages]
             WHERE ProcessedOnUtc IS NULL
             ORDER BY OccurredOnUtc
             """;

        IEnumerable<InboxMessageResponse> inboxMessages = await connection.QueryAsync<InboxMessageResponse>(
                sql,
                transaction: transaction);

        return inboxMessages.AsList();
    }

    private async Task UpdateInboxMessageAsync(
            DbConnection connection,
            DbTransaction transaction,
            InboxMessageResponse inboxMessage,
            Exception? exception)
    {

        const string sql =
        """
            UPDATE [IN].[InboxMessages]
            SET ProcessedOnUtc = @ProcessedOnUtc,
                error = @Error
            WHERE Id = @Id
            """;

        await connection.ExecuteAsync(
                sql,
                new
                {
                    inboxMessage.Id,
                    ProcessedOnUtc = dateTimeProvider.UtcNow,
                    Error = exception?.ToString()
                },
                transaction: transaction);
    }

    internal sealed record InboxMessageResponse(Guid Id, string Content);
}
