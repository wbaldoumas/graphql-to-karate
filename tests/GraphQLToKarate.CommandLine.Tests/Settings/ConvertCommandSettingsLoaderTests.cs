using FluentAssertions;
using GraphQLToKarate.CommandLine.Exceptions;
using GraphQLToKarate.CommandLine.Mappers;
using GraphQLToKarate.CommandLine.Settings;
using GraphQLToKarate.Library.Mappings;
using GraphQLToKarate.Library.Tokens;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using System.IO.Abstractions;
using System.Text.Json;

namespace GraphQLToKarate.CommandLine.Tests.Settings;

[TestFixture]
internal sealed class ConvertCommandSettingsLoaderTests
{
    private IFile? _mockFile;
    private ICustomScalarMappingLoader? _mockCustomScalarMappingLoader;
    private IGraphQLToKarateUserConfigurationMapper? _mockGraphQLToKarateUserConfigurationMapper;
    private ILogger<ConvertCommandSettingsLoader>? _mockLogger;
    private IConvertCommandSettingsLoader? _subjectUnderTest;

    [SetUp]
    public void SetUp()
    {
        _mockFile = Substitute.For<IFile>();
        _mockCustomScalarMappingLoader = Substitute.For<ICustomScalarMappingLoader>();
        _mockGraphQLToKarateUserConfigurationMapper = Substitute.For<IGraphQLToKarateUserConfigurationMapper>();
        _mockLogger = Substitute.For<ILogger<ConvertCommandSettingsLoader>>();

        _subjectUnderTest = new ConvertCommandSettingsLoader(
            _mockFile,
            _mockCustomScalarMappingLoader,
            _mockGraphQLToKarateUserConfigurationMapper,
            _mockLogger
        );
    }

    private const string SomeGraphQLSchema = """
            interface Character {
              id: ID!
              name: String!
              race: Race!
            }

            enum Race {
              HOBBIT
              ELF
              DWARF
              MAN
              ORC
            }

            type Hobbit implements Character {
              id: ID!
              name: String!
              race: Race!
              home: String!
            }

            type Elf implements Character {
              id: ID!
              name: String!
              race: Race!
              realm: String!
            }

            type Fellowship {
              id: ID!
              name: String!
              members: [Character]!
              quest: String!
            }

            type Query {
              character(id: ID!): Character
              fellowship(id: ID!, name: String): Fellowship
              allFellowships: [Fellowship]!
            }
            """;

    [Test]
    public async Task ConvertCommandSettingsLoader_loads_expected_settings_when_custom_scalar_mapping_is_present()
    {
        // arrange
        var convertCommandSettings = new ConvertCommandSettings(_mockFile!, _mockCustomScalarMappingLoader!)
        {
            InputFile = "schema.graphql",
            CustomScalarMapping = "config.json",
            OutputFile = "karate.feature",
            BaseUrl = "baseUrl",
            ExcludeQueries = false,
            IncludeMutations = true,
            QueryName = GraphQLToken.Query,
            MutationName = GraphQLToken.Mutation,
            TypeFilter = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Hello"
            },
            QueryOperationFilter = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "World"
            }
        };

        _mockFile!
            .ReadAllTextAsync(convertCommandSettings.InputFile)
            .Returns(SomeGraphQLSchema);

        var expectedCustomScalarMapping = new CustomScalarMapping(
            new Dictionary<string, string>
            {
                { "key1", "value1" },
                { "key2", "value2" },
                { "key3", "value3" }
            }
        );

        _mockCustomScalarMappingLoader!
            .LoadAsync(convertCommandSettings.CustomScalarMapping)
            .Returns(expectedCustomScalarMapping);

        // act
        var loadedConvertCommandSettings = await _subjectUnderTest!.LoadAsync(convertCommandSettings);

        // assert
        loadedConvertCommandSettings.OutputFile.Should().Be(convertCommandSettings.OutputFile);
        loadedConvertCommandSettings.GraphQLSchema.Should().Be(SomeGraphQLSchema);
        loadedConvertCommandSettings.CustomScalarMapping.Should().BeEquivalentTo(expectedCustomScalarMapping);
        loadedConvertCommandSettings.BaseUrl.Should().Be(convertCommandSettings.BaseUrl);
        loadedConvertCommandSettings.ExcludeQueries.Should().Be(convertCommandSettings.ExcludeQueries);
        loadedConvertCommandSettings.QueryName.Should().Be(convertCommandSettings.QueryName);
        loadedConvertCommandSettings.MutationName.Should().Be(convertCommandSettings.MutationName);
        loadedConvertCommandSettings.TypeFilter.Should().BeEquivalentTo(convertCommandSettings.TypeFilter);
        loadedConvertCommandSettings.QueryOperationFilter.Should()
            .BeEquivalentTo(convertCommandSettings.QueryOperationFilter);
        loadedConvertCommandSettings.IncludeMutations.Should().Be(convertCommandSettings.IncludeMutations);
    }

    [Test]
    public async Task ConvertCommandSettingsLoader_loads_expected_settings_when_custom_scalar_mapping_is_not_present()
    {
        // arrange
        var convertCommandSettings = new ConvertCommandSettings(_mockFile!, _mockCustomScalarMappingLoader!)
        {
            InputFile = "schema.graphql",
            CustomScalarMapping = null,
            OutputFile = "karate.feature",
            QueryName = null,
            MutationName = null
        };

        _mockFile!
            .ReadAllTextAsync(convertCommandSettings.InputFile)
            .Returns(SomeGraphQLSchema);

        _mockCustomScalarMappingLoader!
            .LoadAsync(convertCommandSettings.CustomScalarMapping)
            .Returns(new CustomScalarMapping());

        var expectedCustomScalarMapping = new CustomScalarMapping();

        // act
        var loadedConvertCommandSettings = await _subjectUnderTest!.LoadAsync(convertCommandSettings);

        // assert
        loadedConvertCommandSettings.OutputFile.Should().Be(convertCommandSettings.OutputFile);
        loadedConvertCommandSettings.GraphQLSchema.Should().Be(SomeGraphQLSchema);
        loadedConvertCommandSettings.CustomScalarMapping.Should().BeEquivalentTo(expectedCustomScalarMapping);
        loadedConvertCommandSettings.BaseUrl.Should().Be(convertCommandSettings.BaseUrl);
        loadedConvertCommandSettings.ExcludeQueries.Should().Be(convertCommandSettings.ExcludeQueries);
        loadedConvertCommandSettings.QueryName.Should().Be(GraphQLToken.Query);
        loadedConvertCommandSettings.MutationName.Should().Be(GraphQLToken.Mutation);
        loadedConvertCommandSettings.TypeFilter.Should().BeEquivalentTo(convertCommandSettings.TypeFilter);
        loadedConvertCommandSettings.QueryOperationFilter.Should()
            .BeEquivalentTo(convertCommandSettings.QueryOperationFilter);
    }

    [Test]
    public async Task ConvertCommandSettingsLoader_loads_expected_settings_when_custom_scalar_mapping_is_invalid()
    {
        // arrange
        var convertCommandSettings = new ConvertCommandSettings(_mockFile!, _mockCustomScalarMappingLoader!)
        {
            InputFile = "schema.graphql",
            CustomScalarMapping = "nothing to see here",
            OutputFile = "karate.feature"
        };

        _mockFile!
            .ReadAllTextAsync(convertCommandSettings.InputFile)
            .Returns(SomeGraphQLSchema);

        _mockCustomScalarMappingLoader!
            .LoadAsync(convertCommandSettings.CustomScalarMapping)
            .Returns(new CustomScalarMapping());

        var expectedCustomScalarMapping = new CustomScalarMapping();

        // act
        var loadedConvertCommandSettings = await _subjectUnderTest!.LoadAsync(convertCommandSettings);

        // assert
        loadedConvertCommandSettings.OutputFile.Should().Be(convertCommandSettings.OutputFile);
        loadedConvertCommandSettings.GraphQLSchema.Should().Be(SomeGraphQLSchema);
        loadedConvertCommandSettings.CustomScalarMapping.Should().BeEquivalentTo(expectedCustomScalarMapping);
        loadedConvertCommandSettings.BaseUrl.Should().Be(convertCommandSettings.BaseUrl);
        loadedConvertCommandSettings.ExcludeQueries.Should().Be(convertCommandSettings.ExcludeQueries);
        loadedConvertCommandSettings.QueryName.Should().Be(convertCommandSettings.QueryName);
        loadedConvertCommandSettings.TypeFilter.Should().BeEquivalentTo(convertCommandSettings.TypeFilter);
        loadedConvertCommandSettings.QueryOperationFilter.Should()
            .BeEquivalentTo(convertCommandSettings.QueryOperationFilter);
    }

    [Test]
    public async Task ConvertCommandSettingsLoader_loads_expected_settings_when_configuration_file_is_present()
    {
        // arrange
        var convertCommandSettings = new ConvertCommandSettings(_mockFile!, _mockCustomScalarMappingLoader!)
        {
            InputFile = "schema.graphql",
            ConfigurationFile = "config.json",
        };

        _mockFile!
            .ReadAllTextAsync(convertCommandSettings.InputFile)
            .Returns(SomeGraphQLSchema);

        const string mockGraphQLToKarateUserConfigurationJson = """
            {
                "outputFile": "outputFile.feature",
                "queryName": "customQuery",
                "mutationName": "customMutation",
                "excludeQueries": false,
                "includeMutations": true,
                "baseUrl": "baseUrl",
                "customScalarMapping": {
                    "Date": "string",
                    "DateTime": "string",
                    "Long": "number"
                },
                "typeFilter": ["Hello"],
                "queryOperationFilter": ["World"],
                "mutationOperationFilter": ["Goodbye"]
            }
            """;

        _mockFile!
            .ReadAllTextAsync(convertCommandSettings.ConfigurationFile)
            .Returns(mockGraphQLToKarateUserConfigurationJson);

        _mockGraphQLToKarateUserConfigurationMapper!
            .Map(
                Arg.Is<GraphQLToKarateUserConfiguration>(
                    arg => arg.GraphQLSchema == SomeGraphQLSchema &&
                           arg.InputFile == convertCommandSettings.InputFile &&
                           arg.OutputFile == "outputFile.feature" &&
                           arg.QueryName.Equals("customQuery", StringComparison.OrdinalIgnoreCase) &&
                           arg.MutationName.Equals("customMutation", StringComparison.OrdinalIgnoreCase) &&
                           arg.ExcludeQueries == false &&
                           arg.IncludeMutations == true &&
                           arg.BaseUrl.Equals("baseUrl", StringComparison.OrdinalIgnoreCase) &&
                           arg.CustomScalarMapping.ContainsKey("Date") &&
                           arg.CustomScalarMapping["Date"].Equals("string", StringComparison.OrdinalIgnoreCase) &&
                           arg.CustomScalarMapping.ContainsKey("DateTime") &&
                           arg.CustomScalarMapping["DateTime"].Equals("string", StringComparison.OrdinalIgnoreCase) &&
                           arg.CustomScalarMapping.ContainsKey("Long") &&
                           arg.CustomScalarMapping["Long"].Equals("number", StringComparison.OrdinalIgnoreCase) &&
                           arg.TypeFilter.Count == 1 &&
                           arg.TypeFilter.First().Equals("Hello", StringComparison.OrdinalIgnoreCase) &&
                           arg.QueryOperationFilter.Count == 1 && arg.QueryOperationFilter.First().Equals("World", StringComparison.OrdinalIgnoreCase) &&
                           arg.MutationOperationFilter.Count == 1 && arg.MutationOperationFilter.First().Equals("Goodbye", StringComparison.OrdinalIgnoreCase)
                )
            )
            .Returns(
                new LoadedConvertCommandSettings
                {
                    GraphQLSchema = SomeGraphQLSchema,
                    InputFile = convertCommandSettings.InputFile,
                    OutputFile = "outputFile.feature",
                    QueryName = "customQuery",
                    MutationName = "customMutation",
                    ExcludeQueries = false,
                    IncludeMutations = true,
                    BaseUrl = "baseUrl",
                    CustomScalarMapping = new CustomScalarMapping(
                        new Dictionary<string, string>
                        {
                            { "date", "string" },
                            { "datetime", "string" },
                            { "long", "number" }
                        }
                    ),
                    TypeFilter = new HashSet<string> { "Hello" },
                    QueryOperationFilter = new HashSet<string> { "World" },
                    MutationOperationFilter = new HashSet<string> { "Goodbye" }
                }
            );

        // act
        var loadedConvertCommandSettings = await _subjectUnderTest!.LoadAsync(convertCommandSettings);

        // assert
        loadedConvertCommandSettings.OutputFile.Should().Be("outputFile.feature");
        loadedConvertCommandSettings.GraphQLSchema.Should().Be(SomeGraphQLSchema);
        loadedConvertCommandSettings.CustomScalarMapping.Should().BeEquivalentTo(
            new CustomScalarMapping(
                new Dictionary<string, string>
                {
                    { "date", "string" },
                    { "datetime", "string" },
                    { "long", "number" }
                }
            )
        );
        loadedConvertCommandSettings.BaseUrl.Should().Be("baseUrl");
        loadedConvertCommandSettings.QueryName.Should().Be("customQuery");
        loadedConvertCommandSettings.MutationName.Should().Be("customMutation");
        loadedConvertCommandSettings.ExcludeQueries.Should().BeFalse();
        loadedConvertCommandSettings.IncludeMutations.Should().BeTrue();
    }

    [Test]
    public async Task ConvertCommandSettingsLoader_throws_exception_when_configuration_file_does_not_exist()
    {
        // arrange
        var convertCommandSettings = new ConvertCommandSettings(_mockFile!, _mockCustomScalarMappingLoader!)
        {
            InputFile = "schema.graphql",
            ConfigurationFile = "config.json",
        };

        _mockFile!
            .ReadAllTextAsync(convertCommandSettings.InputFile)
            .Returns(SomeGraphQLSchema);

        _mockFile!
            .ReadAllTextAsync(convertCommandSettings.ConfigurationFile)
            .Returns(string.Empty);

        // act
        var act = async () => await _subjectUnderTest!.LoadAsync(convertCommandSettings);

        // assert
        await act
            .Should()
            .ThrowAsync<GraphQLToKarateConfigurationException>()
            .WithMessage(GraphQLToKarateConfigurationException.DefaultMessage)
            .WithInnerException(typeof(JsonException));
    }

    [Test]
    public async Task ConvertCommandSettingsLoader_throws_exception_when_configuration_is_null()
    {
        // arrange
        var convertCommandSettings = new ConvertCommandSettings(_mockFile!, _mockCustomScalarMappingLoader!)
        {
            InputFile = "schema.graphql",
            ConfigurationFile = "config.json",
        };

        _mockFile!
            .ReadAllTextAsync(convertCommandSettings.InputFile)
            .Returns(SomeGraphQLSchema);

        _mockFile!
            .ReadAllTextAsync(convertCommandSettings.ConfigurationFile)
            .Returns("null");

        // act
        var act = async () => await _subjectUnderTest!.LoadAsync(convertCommandSettings);

        // assert
        await act
            .Should()
            .ThrowAsync<GraphQLToKarateConfigurationException>()
            .WithMessage(GraphQLToKarateConfigurationException.DefaultMessage)
            .WithInnerException(typeof(InvalidOperationException));
    }
}