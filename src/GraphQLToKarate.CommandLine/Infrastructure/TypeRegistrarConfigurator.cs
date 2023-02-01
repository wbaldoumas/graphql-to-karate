using GraphQLToKarate.CommandLine.Settings;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using System.IO.Abstractions;
using GraphQLToKarate.Library.Builders;

namespace GraphQLToKarate.CommandLine.Infrastructure;

internal static class TypeRegistrarConfigurator
{
    public static ITypeRegistrar ConfigureTypeRegistrar()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<IFileSystem, FileSystem>();
        serviceCollection.AddSingleton<IFile, FileWrapper>();
        serviceCollection.AddTransient<ConvertCommandSettings>();
        serviceCollection.AddTransient<IConvertCommandSettingsLoader, ConvertCommandSettingsLoader>();
        serviceCollection.AddTransient<IGraphQLToKarateConverterBuilder, GraphQLToKarateConverterBuilder>();

        return new TypeRegistrar(serviceCollection);
    }
}