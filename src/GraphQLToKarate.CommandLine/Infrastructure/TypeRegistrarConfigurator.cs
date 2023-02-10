using GraphQLToKarate.CommandLine.Settings;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using System.IO.Abstractions;
using GraphQLToKarate.Library.Builders;
using GraphQLToKarate.Library.Mappings;

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
        serviceCollection.AddSingleton<ICustomScalarMappingValidator, CustomScalarMappingLoader>();
        serviceCollection.AddSingleton<ICustomScalarMappingLoader, CustomScalarMappingLoader>();

        return new TypeRegistrar(serviceCollection);
    }
}