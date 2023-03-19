using FluentAssertions;
using GraphQLToKarate.CommandLine.Mappers;
using GraphQLToKarate.CommandLine.Settings;
using GraphQLToKarate.Library.Mappings;
using NUnit.Framework;

namespace GraphQLToKarate.CommandLine.Tests.Mappers;

public class GraphQLToKarateUserConfigurationMapperTests
{
    [Test]
    public void Map_should_map_to_expected_LoadedConvertCommandSettings()
    {
        // Arrange
        var graphQLToKarateUserConfigurationMapper = new GraphQLToKarateUserConfigurationMapper();

        var graphQLToKarateUserConfiguration = new GraphQLToKarateUserConfiguration
        {
            GraphQLSchema = "SampleSchema",
            InputFile = "SampleInputFile",
            OutputFile = "SampleOutputFile",
            BaseUrl = "https://example.com",
            QueryName = "SampleQuery",
            MutationName = "SampleMutation",
            ExcludeQueries = true,
            CustomScalarMapping = new Dictionary<string, string> { { "key1", "value1" }, { "key2", "value2" } },
            IncludeMutations = true,
            QueryOperationFilter = new HashSet<string> { "queryOperation1", "queryOperation2" },
            MutationOperationFilter = new HashSet<string> { "mutationOperation1", "mutationOperation2" },
            TypeFilter = new HashSet<string> { "type1", "type2" }
        };

        var expectedLoadedConvertCommandSettings = new LoadedConvertCommandSettings
        {
            GraphQLSchema = graphQLToKarateUserConfiguration.GraphQLSchema,
            InputFile = graphQLToKarateUserConfiguration.InputFile,
            OutputFile = graphQLToKarateUserConfiguration.OutputFile,
            BaseUrl = graphQLToKarateUserConfiguration.BaseUrl,
            QueryName = graphQLToKarateUserConfiguration.QueryName,
            MutationName = graphQLToKarateUserConfiguration.MutationName,
            ExcludeQueries = graphQLToKarateUserConfiguration.ExcludeQueries,
            CustomScalarMapping = new CustomScalarMapping(graphQLToKarateUserConfiguration.CustomScalarMapping),
            IncludeMutations = graphQLToKarateUserConfiguration.IncludeMutations,
            QueryOperationFilter = graphQLToKarateUserConfiguration.QueryOperationFilter,
            MutationOperationFilter = graphQLToKarateUserConfiguration.MutationOperationFilter,
            TypeFilter = graphQLToKarateUserConfiguration.TypeFilter
        };

        // act
        var loadedConvertCommandSettings = graphQLToKarateUserConfigurationMapper.Map(graphQLToKarateUserConfiguration);

        // assert
        loadedConvertCommandSettings.Should().BeEquivalentTo(expectedLoadedConvertCommandSettings);
    }
}