using Common.Application.Database;
using Common.Application.EventBus;
using Common.Infrastructure.Inbox;
using Dapper;
using System.Data.Common;

namespace Inventory.Infrastructure.Common.Inbox;

internal sealed class IdempotentIntegrationEventHandler<TIntegrationEvent>(
        IIntegrationEventHandler<TIntegrationEvent> decorated,
        IDbConnectionFactory _dbConnectionFactory)
        : IntegrationEventHandler<TIntegrationEvent>
        where TIntegrationEvent : IIntegrationEvent
{
    public override async Task Handle(
            TIntegrationEvent integrationEvent,
            CancellationToken cancellationToken = default)
    {
        await using DbConnection connection = await _dbConnectionFactory.OpenConnectionAsync();

        InboxMessageConsumer inboxMessageConsumer = new(integrationEvent.Id, decorated.GetType().Name);

        if (await InboxConsumerExistsAsync(connection, inboxMessageConsumer))
        {
            return;
        }

        await decorated.Handle(integrationEvent, cancellationToken);

        await InsertInboxConsumerAsync(connection, inboxMessageConsumer);
    }

    private static async Task<bool> InboxConsumerExistsAsync(
            DbConnection dbConnection,
            InboxMessageConsumer inboxMessageConsumer)
    {
        const string sql =
                """
            SELECT EXISTS(
                SELECT 1
                FROM inventory.inbox_message_consumers
                WHERE inbox_message_id = @InboxMessageId AND
                      name = @Name
            )
            """;

        return await dbConnection.ExecuteScalarAsync<bool>(sql, inboxMessageConsumer);
    }

    private static async Task InsertInboxConsumerAsync(
            DbConnection dbConnection,
            InboxMessageConsumer inboxMessageConsumer)
    {
        const string sql =
                """
            INSERT INTO inventory.inbox_message_consumers(inbox_message_id, name)
            VALUES (@InboxMessageId, @Name)
            """;

        await dbConnection.ExecuteAsync(sql, inboxMessageConsumer);
    }
}
