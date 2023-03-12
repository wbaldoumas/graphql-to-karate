using GraphQLToKarate.CommandLine.Commands;
using Spectre.Console.Cli;

namespace GraphQLToKarate.CommandLine.Infrastructure;

/// <summary>
///     Configures the command app.
/// </summary>
internal static class CommandAppConfigurator
{
    /// <summary>
    ///    Configures the command app.
    /// </summary>
    /// <param name="typeRegistrar">The type registrar to use with the command app.</param>
    /// <returns>The configured command app.</returns>
    public static CommandApp ConfigureCommandApp(ITypeRegistrar typeRegistrar)
    {
        var commandApp = new CommandApp(typeRegistrar);

        commandApp.Configure(configurator =>
        {
            configurator.SetInterceptor(new LogCommandSettingsInterceptor());
            configurator.AddCommand<ConvertCommand>("convert");
        });

        return commandApp;
    }
}