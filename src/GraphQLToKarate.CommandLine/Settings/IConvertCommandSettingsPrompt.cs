namespace GraphQLToKarate.CommandLine.Settings;

/// <summary>
///     Prompt the user for settings in an interactive way.
/// </summary>
internal interface IConvertCommandSettingsPrompt
{
    /// <summary>
    ///     Prompt the user for settings in an interactive way.
    /// </summary>
    /// <param name="initialLoadedConvertCommandSettings">
    ///     The initial settings to use as a starting point. This lets the user pass some initial options in the
    ///     up-front command, which are then used as defaults when they are prompted interactively.
    /// </param>
    /// <returns>The settings that the user has chosen.</returns>
    Task<LoadedConvertCommandSettings> PromptAsync(LoadedConvertCommandSettings initialLoadedConvertCommandSettings);
}