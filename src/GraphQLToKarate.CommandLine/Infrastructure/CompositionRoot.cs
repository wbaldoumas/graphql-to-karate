global using GraphQLToKarate.CommandLine.Infrastructure;
using System.Diagnostics.CodeAnalysis;
using Spectre.Console.Cli;

namespace GraphQLToKarate.CommandLine.Infrastructure;

[ExcludeFromCodeCoverage(Justification = "Just abstraction and orchestration of app bootstrapping...")]
internal static class CompositionRoot
{
    public static CommandApp Build() => CommandAppConfigurator.ConfigureCommandApp(
        TypeRegistrarConfigurator.ConfigureTypeRegistrar()
    );
}