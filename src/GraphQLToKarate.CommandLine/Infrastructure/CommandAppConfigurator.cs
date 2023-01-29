using GraphQLToKarate.CommandLine.Commands;
using Spectre.Console.Cli;

namespace GraphQLToKarate.CommandLine.Infrastructure;

internal static class CommandAppConfigurator
{
    public static CommandApp ConfigureCommandApp(ITypeRegistrar typeRegistrar)
    {
        var commandApp = new CommandApp(typeRegistrar);

        commandApp.Configure(configurator =>
        {
            configurator.AddCommand<ConvertCommand>("convert");
        });

        return commandApp;
    }
}