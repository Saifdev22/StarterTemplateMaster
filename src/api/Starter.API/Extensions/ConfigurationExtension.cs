namespace Starter.API.Extensions;

internal static class ConfigurationExtension
{
    internal static void AddModuleConfiguration(this IConfigurationBuilder configurationBuilder, string[] modules)
    {
        foreach (string module in modules)
        {
            configurationBuilder.AddJsonFile($"Configurations/{module}.config.json", true, true);
            configurationBuilder.AddJsonFile($"Configurations/{module}.config.Development.json", true, true);
        }
    }
}