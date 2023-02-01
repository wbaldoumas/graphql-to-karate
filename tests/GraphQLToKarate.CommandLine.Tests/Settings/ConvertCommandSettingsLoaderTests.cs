using FluentAssertions;
using GraphQLToKarate.CommandLine.Settings;
using NSubstitute;
using NUnit.Framework;
using System.IO.Abstractions;

namespace GraphQLToKarate.CommandLine.Tests.Settings;

[TestFixture]
internal sealed class ConvertCommandSettingsLoaderTests
{
    private IFile? _mockFile;
    private IConvertCommandSettingsLoader? _subjectUnderTest;

    [SetUp]
    public void SetUp()
    {
        _mockFile = Substitute.For<IFile>();
        _subjectUnderTest = new ConvertCommandSettingsLoader(_mockFile);
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
        var convertCommandSettings = new ConvertCommandSettings(_mockFile!)
        {
            InputFile = "schema.graphql",
            CustomScalarMappingFile = "config.json",
            OutputFile = "karate.feature"
        };

        const string someCustomScalarMapping = """
            {
              "key1": "value1",
              "key2": "value2",
              "key3": "value3"
            }
            """;

        _mockFile!
            .ReadAllTextAsync(convertCommandSettings.InputFile)
            .Returns(SomeGraphQLSchema);

        _mockFile!
            .ReadAllTextAsync(convertCommandSettings.CustomScalarMappingFile)
            .Returns(someCustomScalarMapping);

        var expectedCustomScalarMapping = new Dictionary<string, string>
        {
            { "key1", "value1" },
            { "key2", "value2" },
            { "key3", "value3" }
        };

        // act
        var loadedConvertCommandSettings = await _subjectUnderTest!.LoadAsync(convertCommandSettings);

        // assert
        loadedConvertCommandSettings.OutputFile.Should().Be(convertCommandSettings.OutputFile);
        loadedConvertCommandSettings.GraphQLSchema.Should().Be(SomeGraphQLSchema);
        loadedConvertCommandSettings.CustomScalarMapping.Should().BeEquivalentTo(expectedCustomScalarMapping);
    }

    [Test]
    public async Task ConvertCommandSettingsLoader_loads_expected_settings_when_custom_scalar_mapping_is_not_present()
    {
        // arrange
        var convertCommandSettings = new ConvertCommandSettings(_mockFile!)
        {
            InputFile = "schema.graphql",
            CustomScalarMappingFile = null,
            OutputFile = "karate.feature"
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
    }
}