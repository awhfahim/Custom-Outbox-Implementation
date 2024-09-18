namespace MtslErp.Api.Extensions;

public static class ConfigurationExtensions
{
    public static void AddModuleConfiguration(this IConfigurationBuilder configurationBuilder,
        AvailableModule[] modules)
    {
        foreach (var module in modules)
        {
            configurationBuilder.AddJsonFile($"{module.ToString().ToLower()}.module.json", optional: false,
                true);
        }
    }
}
