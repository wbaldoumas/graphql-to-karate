using FluentAssertions;
using GraphQLToKarate.CommandLine.Settings;
using NSubstitute;
using NUnit.Framework;
using System.IO.Abstractions;
using GraphQLToKarate.Library.Mappings;
using GraphQLToKarate.Library.Tokens;

namespace GraphQLToKarate.CommandLine.Tests.Settings;

[TestFixture]
internal sealed class ConvertCommandSettingsLoaderTests
{
    private IFile? _mockFile;
    private ICustomScalarMappingLoader? _mockCustomScalarMappingLoader;
    private IConvertCommandSettingsLoader? _subjectUnderTest;

    [SetUp]
    public void SetUp()
    {
        _mockFile = Substitute.For<IFile>();
        _mockCustomScalarMappingLoader = Substitute.For<ICustomScalarMappingLoader>();
        _subjectUnderTest = new ConvertCommandSettingsLoader(_mockFile, _mockCustomScalarMappingLoader);
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
    [TestCase(true)]
    [TestCase(false)]
    public async Task ConvertCommandSettingsLoader_loads_expected_settings_when_custom_scalar_mapping_is_present(
        bool mockCustomScalarMappingLoaderIsFileLoadableReturn)
    {
        // arrange
        var convertCommandSettings = new ConvertCommandSettings(_mockFile!, _mockCustomScalarMappingLoader!)
        {
            InputFile = "schema.graphql",
            CustomScalarMapping = "config.json",
            OutputFile = "karate.feature",
            BaseUrl = "baseUrl",
            ExcludeQueries = false,
            QueryName = GraphQLToken.Query,
            TypeFilter = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Hello"
            }
        };

        _mockFile!
            .ReadAllTextAsync(convertCommandSettings.InputFile)
            .Returns(SomeGraphQLSchema);

        var expectedCustomScalarMapping = new Dictionary<string, string>
        {
            { "key1", "value1" },
            { "key2", "value2" },
            { "key3", "value3" }
        };

        _mockCustomScalarMappingLoader!
            .IsFileLoadable(convertCommandSettings.CustomScalarMapping)
            .Returns(mockCustomScalarMappingLoaderIsFileLoadableReturn);

        _mockCustomScalarMappingLoader
            .IsTextLoadable(convertCommandSettings.CustomScalarMapping)
            .Returns(!mockCustomScalarMappingLoaderIsFileLoadableReturn);

        if (mockCustomScalarMappingLoaderIsFileLoadableReturn)
        {
            _mockCustomScalarMappingLoader
                .LoadFromFileAsync(convertCommandSettings.CustomScalarMapping)
                .Returns(expectedCustomScalarMapping);
        }
        else
        {
            _mockCustomScalarMappingLoader
                .LoadFromText(convertCommandSettings.CustomScalarMapping)
                .Returns(expectedCustomScalarMapping);
        }

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
            QueryName = null
        };

        _mockFile!
            .ReadAllTextAsync(convertCommandSettings.InputFile)
            .Returns(SomeGraphQLSchema);

        var expectedCustomScalarMapping = new Dictionary<string, string>();

        // act
        var loadedConvertCommandSettings = await _subjectUnderTest!.LoadAsync(convertCommandSettings);

        // assert
        loadedConvertCommandSettings.OutputFile.Should().Be(convertCommandSettings.OutputFile);
        loadedConvertCommandSettings.GraphQLSchema.Should().Be(SomeGraphQLSchema);
        loadedConvertCommandSettings.CustomScalarMapping.Should().BeEquivalentTo(expectedCustomScalarMapping);
        loadedConvertCommandSettings.BaseUrl.Should().Be(convertCommandSettings.BaseUrl);
        loadedConvertCommandSettings.ExcludeQueries.Should().Be(convertCommandSettings.ExcludeQueries);
        loadedConvertCommandSettings.QueryName.Should().Be(GraphQLToken.Query);
        loadedConvertCommandSettings.TypeFilter.Should().BeEquivalentTo(convertCommandSettings.TypeFilter);
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
            .IsFileLoadable(convertCommandSettings.CustomScalarMapping)
            .Returns(false);

        _mockCustomScalarMappingLoader!
            .IsTextLoadable(convertCommandSettings.CustomScalarMapping)
            .Returns(false);

        var expectedCustomScalarMapping = new Dictionary<string, string>();

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
    }
}