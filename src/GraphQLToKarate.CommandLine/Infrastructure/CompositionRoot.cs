global using GraphQLToKarate.CommandLine.Infrastructure;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;

namespace GraphQLToKarate.CommandLine.Infrastructure;

/// <summary>
///    Configures the command app.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Just abstraction and orchestration of app bootstrapping...")]
internal static class CompositionRoot
{
    /// <summary>
    ///     Builds the command app.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    /// <returns>The command app.</returns>
    public static CommandApp Build(string[]? args) => CommandAppConfigurator.ConfigureCommandApp(
        TypeRegistrarConfigurator.ConfigureTypeRegistrar(args)
    );
}