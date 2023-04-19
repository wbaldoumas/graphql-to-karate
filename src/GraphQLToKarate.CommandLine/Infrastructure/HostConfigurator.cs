using GraphQLToKarate.CommandLine.Mappers;
using GraphQLToKarate.CommandLine.Prompts;
using GraphQLToKarate.CommandLine.Settings;
using GraphQLToKarate.Library.Builders;
using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Mappings;
using GraphQLToKarate.Library.Parsers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.IO.Abstractions;

namespace GraphQLToKarate.CommandLine.Infrastructure;

/// <summary>
///   Configures the host builder.
/// </summary>
internal static class HostConfigurator
{
    /// <summary>
    ///    Configures the host builder.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    /// <returns>The configured host builder.</returns>
    public static IHostBuilder ConfigureHostBuilder(string[]? args) => Host
        .CreateDefaultBuilder(args)
        .ConfigureServices(serviceCollection =>
        {
            serviceCollection.AddSingleton<IFileSystem, FileSystem>();
            serviceCollection.AddSingleton<IFile, FileWrapper>();
            serviceCollection.AddTransient<ConvertCommandSettings>();
            serviceCollection.AddSingleton<IGraphQLToKarateUserConfigurationMapper, GraphQLToKarateUserConfigurationMapper>();
            serviceCollection.AddSingleton<IConvertCommandSettingsLoader, ConvertCommandSettingsLoader>();
            serviceCollection.AddTransient<IGraphQLToKarateConverterBuilder, GraphQLToKarateConverterBuilder>();
            serviceCollection.AddSingleton<ICustomScalarMappingValidator, CustomScalarMappingLoader>();
            serviceCollection.AddSingleton<ICustomScalarMappingLoader, CustomScalarMappingLoader>();
            serviceCollection.AddSingleton<IGraphQLSchemaParser, GraphQLSchemaParser>();
            serviceCollection.AddSingleton<IGraphQLCyclicToAcyclicConverter, GraphQLCyclicToAcyclicConverter>();
            serviceCollection.AddSingleton<IConvertCommandSettingsPrompt, ConvertCommandSettingsPrompt>();
        })
        .UseSerilog((_, loggerConfiguration) =>
            loggerConfiguration
                .MinimumLevel.ControlledBy(LogCommandSettingsInterceptor.LoggingLevelSwitch)
                .WriteTo.Console(
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message:lj}{NewLine}{Exception}",
                    theme: AnsiConsoleTheme.Sixteen
                )
        );
}