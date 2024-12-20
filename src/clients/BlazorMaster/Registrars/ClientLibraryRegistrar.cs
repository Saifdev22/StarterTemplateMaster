using BlazorCommon;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

namespace BlazorMaster.Registrars;

internal sealed class ClientLibraryRegistrar : IWebAssemblyHostBuilderRegistrar
{
    public void RegisterServices(WebAssemblyHostBuilder builder)
    {
        builder.Services.AddClientLibrary();
        builder.Services.AddMudServices();
    }
}