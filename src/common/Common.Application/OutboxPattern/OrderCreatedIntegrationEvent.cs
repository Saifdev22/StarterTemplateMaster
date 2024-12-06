namespace Common.Application.OutboxPattern;

public sealed record OrderCreatedIntegrationEvent(Guid OrderId);
