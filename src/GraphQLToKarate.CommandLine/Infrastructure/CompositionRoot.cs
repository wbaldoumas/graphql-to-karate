global using GraphQLToKarate.CommandLine.Infrastructure;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;

namespace GraphQLToKarate.CommandLine.Infrastructure;

internal static class CompositionRoot
{
    public static CommandApp Build() => CommandAppConfigurator.ConfigureCommandApp(
        TypeRegistrarConfigurator.ConfigureTypeRegistrar()
    );
}