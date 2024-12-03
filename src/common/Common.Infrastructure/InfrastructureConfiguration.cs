using Common.Application.Caching;
using Common.Application.Clock;
using Common.Application.Database;
using Common.Application.EventBus;
using Common.Infrastructure.Authentication;
using Common.Infrastructure.Authorization;
using Common.Infrastructure.Caching;
using Common.Infrastructure.Clock;
using Common.Infrastructure.Database;
using Common.Infrastructure.Outbox;
using Common.Infrastructure.OutboxScaling;
using Common.Infrastructure.System;
using Dapper;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Quartz;
using StackExchange.Redis;

namespace Common.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddCommonInfrastructure(
        this IServiceCollection services,
        string serviceName,
        Action<IRegistrationConfigurator,
        string>[] moduleConfigureConsumers,
        string redisConnectionString,
        string rabbitmqConnectionString,
        string postgresConnectionString)
    {
        services.AddAuthenticationInternal();
        services.AddAuthorizationInternal();

        services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.TryAddSingleton<IEventBus, EventBus.EventBus>();

        services.TryAddSingleton<InsertOutboxMessagesInterceptor>();

        services.AddScoped<CurrentTenant>();
        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

        //Dapper
        SqlMapper.AddTypeHandler(new GenericArrayHandler<string>());

        //Quartz
        services.AddQuartz(configurator =>
        {
            Guid scheduler = Guid.NewGuid();
            configurator.SchedulerId = $"default-id-{scheduler}";
            configurator.SchedulerName = $"default-name-{scheduler}";
        });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        try
        {
            IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
            services.AddSingleton(connectionMultiplexer);
            services.AddStackExchangeRedisCache(options =>
                    options.ConnectionMultiplexerFactory = () => Task.FromResult(connectionMultiplexer));
        }
        catch
        {
            services.AddDistributedMemoryCache();
        }

        services.TryAddSingleton<ICacheService, CacheService>();

        //Outbox - RabbitMQ - MassTransit

        services.AddScoped<OutboxProcessor>();

        services.AddSingleton(_ =>
        {
            //Change to posetgres
            return new NpgsqlDataSourceBuilder(postgresConnectionString).Build();
        });

        services.AddMassTransit(configure =>
        {
            string instanceId = serviceName.ToUpperInvariant().Replace('.', '-');
            foreach (Action<IRegistrationConfigurator, string> configureConsumers in moduleConfigureConsumers)
            {
                configureConsumers(configure, instanceId);
            }

            configure.SetKebabCaseEndpointNameFormatter();

            configure.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitmqConnectionString, hostCfg =>
                {
                    hostCfg.MaxMessageSize(500000);
                });
                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddHostedService<OutboxBackgroundService>();

        //var host = builder.Build();

        //await host.Services.GetRequiredService<DatabaseInitializer>().Execute();

        //host.Run();

        //services
        //        .AddOpenTelemetry()
        //        .ConfigureResource(resource => resource.AddService(serviceName))
        //        .WithTracing(tracing =>
        //        {
        //            tracing
        //                            .AddAspNetCoreInstrumentation()
        //                            .AddHttpClientInstrumentation()
        //                            .AddEntityFrameworkCoreInstrumentation()
        //                            .AddRedisInstrumentation()
        //                            .AddNpgsql()
        //                            .AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName);

        //            tracing.AddOtlpExporter();
        //        });

        return services;
    }
}