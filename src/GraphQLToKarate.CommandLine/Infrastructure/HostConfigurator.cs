using System.IO.Abstractions;
using GraphQLToKarate.CommandLine.Settings;
using GraphQLToKarate.Library.Builders;
using GraphQLToKarate.Library.Mappings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GraphQLToKarate.CommandLine.Infrastructure;

internal static class HostConfigurator
{
    public static IHostBuilder ConfigureHostBuilder(string[]? args) => Host
        .CreateDefaultBuilder(args)
        .ConfigureServices(serviceCollection =>
        {
            serviceCollection.AddSingleton<IFileSystem, FileSystem>();
            serviceCollection.AddSingleton<IFile, FileWrapper>();
            serviceCollection.AddTransient<ConvertCommandSettings>();
            serviceCollection.AddTransient<IConvertCommandSettingsLoader, ConvertCommandSettingsLoader>();
            serviceCollection.AddTransient<IGraphQLToKarateConverterBuilder, GraphQLToKarateConverterBuilder>();
            serviceCollection.AddSingleton<ICustomScalarMappingValidator, CustomScalarMappingLoader>();
            serviceCollection.AddSingleton<ICustomScalarMappingLoader, CustomScalarMappingLoader>();
        });
}