using BlazorCommon.Delegates;
using BlazorCommon.Helpers;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazorMaster.Registrars;

internal sealed class BaseAddressRegistrar : IWebAssemblyHostBuilderRegistrar
{
    public void RegisterServices(WebAssemblyHostBuilder builder)
    {
        builder.Services.AddHttpClient("SystemApiClient", (sp, client) =>
        {
            IWebAssemblyHostEnvironment env = builder.HostEnvironment;
            IConfiguration configuration = sp.GetRequiredService<IConfiguration>();

            if (env.IsDevelopment())
            {
                client.BaseAddress = new Uri(configuration["BaseUrls:Development"]!);
            }

            if (env.IsProduction())
            {
                client.BaseAddress = new Uri(configuration["BaseUrls:Production"]!);
            }

        })
            .AddHttpMessageHandler<CustomHttpDelegate>()
            .AddHttpMessageHandler<CustomHttpErrorHandler>();

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
    }
}