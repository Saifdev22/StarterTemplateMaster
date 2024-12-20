using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazorMaster.Registrars;

internal interface IWebAssemblyHostBuilderRegistrar : IRegistrar
{
    void RegisterServices(WebAssemblyHostBuilder builder);
}