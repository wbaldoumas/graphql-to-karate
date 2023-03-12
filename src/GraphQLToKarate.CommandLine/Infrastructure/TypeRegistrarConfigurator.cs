using Spectre.Console.Cli;

namespace GraphQLToKarate.CommandLine.Infrastructure;

/// <summary>
///     Configures the type registrar with the configured host builder.
/// </summary>
internal static class TypeRegistrarConfigurator
{
    /// <summary>
    ///    Configures the type registrar with the configured host builder.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    /// <returns>The configured type registrar.</returns>
    public static ITypeRegistrar ConfigureTypeRegistrar(string[]? args) => new TypeRegistrar(
        HostConfigurator.ConfigureHostBuilder(args)
    );
}