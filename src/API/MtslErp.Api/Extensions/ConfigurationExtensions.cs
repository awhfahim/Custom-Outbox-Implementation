namespace MtslErp.Api.Extensions;

public static class ConfigurationExtensions
{
    public static void AddModuleConfiguration(this IConfigurationBuilder configurationBuilder, string[] modules)
    {
        foreach (var module in modules)
        {
            configurationBuilder.AddJsonFile($"{module}.module.json", optional: false, true);
        }
    }
}
