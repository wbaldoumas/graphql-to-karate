using System.IO.Abstractions;
using FluentAssertions;
using GraphQLToKarate.CommandLine.Settings;
using NSubstitute;
using NUnit.Framework;

namespace GraphQLToKarate.CommandLine.Tests.Settings;

[TestFixture]
internal sealed class ConvertCommandSettingsTests
{
    private IFile? _mockFile;

    [SetUp]
    public void SetUp() => _mockFile = Substitute.For<IFile>();

    [TestCaseSource(nameof(TestCases))]
    public void ConvertSettings_validation_returns_expected_validation_result(
        string? graphQLSchemaFile,
        string? customScalarMappingFile,
        bool graphQLSchemaFileExistsReturn,
        bool customScalarMappingFileExistsReturn,
        string? expectedMessage)
    {
        // arrange
        _mockFile!.Exists(graphQLSchemaFile).Returns(graphQLSchemaFileExistsReturn);
        _mockFile.Exists(customScalarMappingFile).Returns(customScalarMappingFileExistsReturn);

        var settings = new ConvertCommandSettings(_mockFile)
        {
            InputFile = graphQLSchemaFile,
            CustomScalarMappingFile = customScalarMappingFile
        };

        // act
        var result = settings.Validate();

        // assert
        if (expectedMessage is null)
        {
            result.Successful.Should().BeTrue();
        }
        else
        {
            result.Successful.Should().BeFalse();
            result.Message.Should().Be(expectedMessage);
        }
    }

    private static IEnumerable<TestCaseData> TestCases
    {
        get
        {
            yield return new TestCaseData(
                null,
                "config.json",
                true,
                true,
                "Please provide a valid file path and filename for the GraphQL schema to convert."
            ).SetName("When InputFile name is null, settings are invalid.");

            yield return new TestCaseData(
                string.Empty,
                "config.json",
                true,
                true,
                "Please provide a valid file path and filename for the GraphQL schema to convert."
            ).SetName("When InputFile name is empty, settings are invalid.");

            yield return new TestCaseData(
                "schema.graphql",
                "config.json",
                false,
                true,
                "GraphQL schema file does not exist. Please provide a valid file path and filename for the GraphQL schema to convert."
            ).SetName("When non-null and non-empty InputFile name points to file that doesn't exist, settings are invalid.");

            yield return new TestCaseData(
                "schema.graphql",
                "customScalarMapping.json",
                true,
                false,
                "Please provide a valid file path and filename for the custom scalar mapping passed to the --csm|--custom-scalar-mapping option."
            ).SetName("When non-null and non-empty custom scalar mapping filename points to file that doesn't exist, settings are invalid.");

            yield return new TestCaseData(
                "schema.graphql",
                null,
                true,
                false,
                null
            ).SetName("When non-null and non-empty InputFile name points to file that exists, settings are valid (null custom scalar mapping).");

            yield return new TestCaseData(
                "schema.graphql",
                string.Empty,
                true,
                false,
                null
            ).SetName("When non-null and non-empty InputFile name points to file that exists, settings are valid (empty custom scalar mapping).");

            yield return new TestCaseData(
                "schema.graphql",
                "customScalarMapping.json",
                true,
                true,
                null
            ).SetName("When non-null and non-empty custom scalar mapping filename points to file that exists, settings are valid.");
        }
    }
}