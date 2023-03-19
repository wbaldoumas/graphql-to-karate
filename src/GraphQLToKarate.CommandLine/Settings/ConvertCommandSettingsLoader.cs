using GraphQLToKarate.CommandLine.Exceptions;
using GraphQLToKarate.CommandLine.Mappers;
using GraphQLToKarate.Library.Mappings;
using Microsoft.Extensions.Logging;
using System.IO.Abstractions;
using System.Text.Json;

namespace GraphQLToKarate.CommandLine.Settings;

/// <inheritdoc cref="IConvertCommandSettingsLoader"/>
internal sealed class ConvertCommandSettingsLoader : IConvertCommandSettingsLoader
{
    private readonly IFile _file;
    private readonly ICustomScalarMappingLoader _customScalarMappingLoader;
    private readonly IGraphQLToKarateUserConfigurationMapper _graphQLToKarateUserConfigurationMapper;
    private readonly ILogger<ConvertCommandSettingsLoader> _logger;

    public ConvertCommandSettingsLoader(
        IFile file,
        ICustomScalarMappingLoader customScalarMappingLoader,
        IGraphQLToKarateUserConfigurationMapper graphQLToKarateUserConfigurationMapper,
        ILogger<ConvertCommandSettingsLoader> logger)
    {
        _file = file;
        _customScalarMappingLoader = customScalarMappingLoader;
        _logger = logger;
        _graphQLToKarateUserConfigurationMapper = graphQLToKarateUserConfigurationMapper;
    }

    public async Task<LoadedConvertCommandSettings> LoadAsync(ConvertCommandSettings convertCommandSettings)
    {
        var graphQLSchema = await _file.ReadAllTextAsync(convertCommandSettings.InputFile!).ConfigureAwait(false);

        if (!string.IsNullOrEmpty(convertCommandSettings.ConfigurationFile))
        {
            return await LoadFromUserConfigurationFile(
                graphQLSchema,
                convertCommandSettings.InputFile!,
                convertCommandSettings.ConfigurationFile
            ).ConfigureAwait(false);
        }

        var customScalarMapping = await _customScalarMappingLoader.LoadAsync(
            convertCommandSettings.CustomScalarMapping
        ).ConfigureAwait(false);

        return new LoadedConvertCommandSettings
        {
            GraphQLSchema = graphQLSchema,
            InputFile = convertCommandSettings.InputFile!,
            OutputFile = convertCommandSettings.OutputFile!,
            CustomScalarMapping = customScalarMapping,
            BaseUrl = convertCommandSettings.BaseUrl ?? Options.BaseUrlOptionDefaultValue,
            ExcludeQueries = convertCommandSettings.ExcludeQueries,
            IncludeMutations = convertCommandSettings.IncludeMutations,
            QueryName = convertCommandSettings.QueryName ?? Options.QueryNameOptionDefaultValue,
            MutationName = convertCommandSettings.MutationName ?? Options.MutationNameOptionDefaultValue,
            TypeFilter = convertCommandSettings.TypeFilter,
            QueryOperationFilter = convertCommandSettings.QueryOperationFilter,
            MutationOperationFilter = convertCommandSettings.MutationOperationFilter
        };
    }

    private async Task<LoadedConvertCommandSettings> LoadFromUserConfigurationFile(
        string graphQLSchema,
        string inputFile,
        string configurationFile)
    {
        try
        {
            var configurationFileContent = await _file.ReadAllTextAsync(configurationFile).ConfigureAwait(false);

            var graphQLToKarateUserConfiguration = JsonSerializer.Deserialize<GraphQLToKarateUserConfiguration>(
                configurationFileContent,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );

            if (graphQLToKarateUserConfiguration is null)
            {
                throw new InvalidOperationException("Invalid configuration file format.");
            }

            graphQLToKarateUserConfiguration.GraphQLSchema = graphQLSchema;
            graphQLToKarateUserConfiguration.InputFile = inputFile;

            return _graphQLToKarateUserConfigurationMapper.Map(graphQLToKarateUserConfiguration);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, GraphQLToKarateConfigurationException.DefaultMessage);

            throw new GraphQLToKarateConfigurationException(
                GraphQLToKarateConfigurationException.DefaultMessage,
                exception
            );
        }
    }
}