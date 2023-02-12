using Spectre.Console.Cli;

namespace GraphQLToKarate.CommandLine.Infrastructure;

internal static class TypeRegistrarConfigurator
{
    public static ITypeRegistrar ConfigureTypeRegistrar(string[]? args) => new TypeRegistrar(
        HostConfigurator.ConfigureHostBuilder(args)
    );
}