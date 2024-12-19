using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazorMaster.Registrars;

internal sealed class AppSettingsRegistrar : IWebAssemblyHostBuilderRegistrar
{
    public void RegisterServices(WebAssemblyHostBuilder builder)
    {
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    }
}
