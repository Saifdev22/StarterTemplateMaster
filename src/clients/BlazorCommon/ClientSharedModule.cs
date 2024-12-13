using BlazorCommon.Helpers;
using BlazorCommon.Services.Contracts;
using BlazorCommon.Services.Implementations;
using Blazored.LocalStorage;
using Common.Domain.TransferObjects.System;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorCommon;

public static class ClientSharedModule
{
    public static IServiceCollection AddClientLibrary(this IServiceCollection services)
    {
        //string apiBase = "https://starter.webport.co.za/";
        string apiBase = "https://localhost:7283/";

        services.AddScoped<LocalStorageService>();
        services.AddTransient<CustomHttpDelegate>();
        services.AddScoped<CustomHttpClient>();
        services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

        services.AddHttpClient("SystemApiClient", client =>
        {
            client.BaseAddress = new Uri(apiBase);
        })
            .AddHttpMessageHandler<CustomHttpDelegate>();

        services.AddBlazoredLocalStorage();
        services.AddAuthorizationCore();

        services.AddScoped<ITokenService, TokenService>();

        services.AddScoped<IGenericService<GetAllTenants, CreateTenantDto>, GenericService<GetAllTenants, CreateTenantDto>>();

        return services;
    }
}
