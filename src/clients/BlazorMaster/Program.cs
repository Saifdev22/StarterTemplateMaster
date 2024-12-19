using BlazorMaster.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RegisterServices(typeof(Program));

await builder.Build().RunAsync();
