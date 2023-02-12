global using GraphQLToKarate.CommandLine.Infrastructure;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;

namespace GraphQLToKarate.CommandLine.Infrastructure;

[ExcludeFromCodeCoverage(Justification = "Just abstraction and orchestration of app bootstrapping...")]
internal static class CompositionRoot
{
    public static CommandApp Build(string[]? args) => CommandAppConfigurator.ConfigureCommandApp(
        TypeRegistrarConfigurator.ConfigureTypeRegistrar(args)
    );
}