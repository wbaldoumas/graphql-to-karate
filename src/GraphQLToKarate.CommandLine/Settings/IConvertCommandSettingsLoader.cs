namespace GraphQLToKarate.CommandLine.Settings;

/// <summary>
///     Loads GraphQL-to-Karate data from a given <see cref="ConvertCommandSettings"/>.
/// </summary>
internal interface IConvertCommandSettingsLoader
{
    /// <summary>
    ///     Load the GraphQL-to-Karate data to convert from the given <paramref name="convertCommandSettings"/>.
    /// </summary>
    /// <param name="convertCommandSettings">The settings to load data into the application with.</param>
    /// <returns>The loaded settings data.</returns>
    Task<LoadedConvertCommandSettings> LoadAsync(ConvertCommandSettings convertCommandSettings);
}