namespace Common.Infrastructure.Events;

public sealed record RabbitMqSettings(string Host, string Username = "guest", string Password = "guest");
