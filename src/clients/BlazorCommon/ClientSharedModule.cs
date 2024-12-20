using BlazorCommon.Helpers;
using BlazorCommon.Services.Contracts;
using BlazorCommon.Services.Implementations;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorCommon;

public static class ClientSharedModule
{
    public static IServiceCollection AddClientLibrary(this IServiceCollection services)
    {
        services.AddScoped<LocalStorageService>();
        services.AddTransient<CustomHttpDelegate>();
        services.AddScoped<CustomHttpClient>();
        services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

        services.AddBlazoredLocalStorage();
        services.AddAuthorizationCore();

        services.AddScoped<ITokenService, TokenService>();

        services.AddScoped<IGenericService<GetAllTenants, CreateTenantDto>, GenericService<GetAllTenants, CreateTenantDto>>();
        services.AddScoped<IGenericService<ReadTenantTypeDto, WriteTenantType>, GenericService<ReadTenantTypeDto, WriteTenantType>>();

        return services;
    }
}
