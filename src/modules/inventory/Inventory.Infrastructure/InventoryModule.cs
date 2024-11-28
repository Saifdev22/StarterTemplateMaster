using Common.Application.EventBus;
using Common.Application.Messaging;
using Common.Infrastructure.Outbox;
using Common.Infrastructure.System;
using Common.Presentation.Endpoints;
using Inventory.Application.Common;
using Inventory.Infrastructure.Common.Database;
using Inventory.Infrastructure.Common.Inbox;
using Inventory.Infrastructure.Common.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Inventory.Infrastructure;
public static class InventoryModule
{
    public static IServiceCollection AddInventoryModule(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddDomainEventHandlers();

        services.AddIntegrationEventHandlers();

        services.AddInfrastructure(configuration);

        services.AddEndpoints(Presentation.AssemblyReference.Assembly);

        return services;
    }

    private static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<InventoryDbContext>((sp, options) =>
        {
            CurrentTenant tenantProvider = sp.GetRequiredService<CurrentTenant>();
            string connectionString = tenantProvider.GetConnectionString();

            options.AddInterceptors(sp.GetServices<InsertOutboxMessagesInterceptor>());
            options.UseSqlServer(connectionString, sqlServerOptionsAction: sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 10,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null);
            });
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<InventoryDbContext>());

        services.Configure<OutboxOptions>(configuration.GetSection("Inventory:Outbox"));
        services.ConfigureOptions<ConfigureProcessOutboxJob>();

        services.Configure<InboxOptions>(configuration.GetSection("Inventory:Inbox"));
        services.ConfigureOptions<ConfigureProcessInboxJob>();
    }

    //public static Action<IRegistrationConfigurator, string> ConfigureConsumers(string redisConnectionString)
    //{
    //    return (registrationConfigurator, instanceId) =>
    //    {
    //        //registrationConfigurator.AddConsumer<IntegrationEventConsumer<CreateUserIntegrationEvent>>();
    //    };
    //}
    private static void AddDomainEventHandlers(this IServiceCollection services)
    {
        Type[] domainEventHandlers = Application.AssemblyReference.Assembly
                .GetTypes()
                .Where(t => t.IsAssignableTo(typeof(IDomainEventHandler)))
                .ToArray();

        foreach (Type domainEventHandler in domainEventHandlers)
        {
            services.TryAddScoped(domainEventHandler);

            Type domainEvent = domainEventHandler
                    .GetInterfaces()
                    .Single(i => i.IsGenericType)
                    .GetGenericArguments()
                    .Single();

            Type closedIdempotentHandler = typeof(IdempotentDomainEventHandler<>).MakeGenericType(domainEvent);

            services.Decorate(domainEventHandler, closedIdempotentHandler);
        }
    }

    private static void AddIntegrationEventHandlers(this IServiceCollection services)
    {
        Type[] integrationEventHandlers = Presentation.AssemblyReference.Assembly
                .GetTypes()
                .Where(t => t.IsAssignableTo(typeof(IIntegrationEventHandler)))
                .ToArray();

        foreach (Type integrationEventHandler in integrationEventHandlers)
        {
            services.TryAddScoped(integrationEventHandler);

            Type integrationEvent = integrationEventHandler
                    .GetInterfaces()
                    .Single(i => i.IsGenericType)
                    .GetGenericArguments()
                    .Single();

            Type closedIdempotentHandler =
                    typeof(IdempotentIntegrationEventHandler<>).MakeGenericType(integrationEvent);

            services.Decorate(integrationEventHandler, closedIdempotentHandler);
        }
    }
}
