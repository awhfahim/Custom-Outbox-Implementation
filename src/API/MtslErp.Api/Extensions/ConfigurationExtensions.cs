namespace MtslErp.Api.Extensions;

using Humanizer;

public static class ConfigurationExtensions
{
    public static void AddModuleConfiguration(this IConfigurationBuilder configurationBuilder,
        AvailableModule[] modules)
    {
        foreach (var module in modules)
        {
            configurationBuilder.AddJsonFile($"{module.ToString().Camelize()}.module.json", optional: false,
                true);
        }
    }
}
