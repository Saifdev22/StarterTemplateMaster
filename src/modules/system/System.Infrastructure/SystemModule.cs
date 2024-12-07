using Common.Application.Authorization;
using Common.Application.Messaging;
using Common.Domain.Abstractions;
using Common.Infrastructure.Database;
using Common.Infrastructure.System;
using Common.Presentation.Endpoints;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Application.Common.Interfaces;
using System.Domain.Features.Identity;
using System.Infrastructure.Common.Authentication;
using System.Infrastructure.Common.Authorization;
using System.Infrastructure.Common.Database;
using System.Infrastructure.Common.Inbox;
using System.Infrastructure.Common.Outbox;
using System.Infrastructure.Features.Identity;
using System.Infrastructure.Features.Tenant;
using System.Presentation.Features.User;

namespace System.Infrastructure;

public static class SystemModule
{
    public static IServiceCollection AddSystemModule(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddDomainEventHandlers();

        //services.AddIntegrationEventHandlers();

        services.AddInfrastructure(configuration);

        services.AddEndpoints(Presentation.AssemblyReference.Assembly);

        return services;
    }

    private static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IIdentityRepository, UserRepository>();
        services.AddScoped<IPermissionService, PermissionService>();

        services.AddDbContext<SystemDbContext>((sp, options) =>
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

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<SystemDbContext>());

        services.Configure<OutboxOptions>(configuration.GetSection("Users:Outbox"));
        services.ConfigureOptions<ConfigureProcessOutboxJob>();

        services.Configure<InboxOptions>(configuration.GetSection("Users:Inbox"));
        services.ConfigureOptions<ConfigureProcessInboxJob>();
    }

    public static void ConfigureConsumers(IRegistrationConfigurator registrationConfigurator, string instanceId)
    {
        ArgumentNullException.ThrowIfNull(registrationConfigurator);

        registrationConfigurator.AddConsumer<GetUserPermissionsRequestConsumer>()
                .Endpoint(c => c.InstanceId = instanceId);
    }

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

    //private static void AddIntegrationEventHandlers(this IServiceCollection services)
    //{
    //    Type[] integrationEventHandlers = Presentation.AssemblyReference.Assembly
    //            .GetTypes()
    //            .Where(t => t.IsAssignableTo(typeof(IIntegrationEventHandler)))
    //            .ToArray();

    //    foreach (Type integrationEventHandler in integrationEventHandlers)
    //    {
    //        services.TryAddScoped(integrationEventHandler);

    //        Type integrationEvent = integrationEventHandler
    //                .GetInterfaces()
    //                .Single(i => i.IsGenericType)
    //                .GetGenericArguments()
    //                .Single();

    //        Type closedIdempotentHandler =
    //                typeof(IdempotentIntegrationEventHandler<>).MakeGenericType(integrationEvent);

    //        services.Decorate(integrationEventHandler, closedIdempotentHandler);
    //    }
    //}

}


