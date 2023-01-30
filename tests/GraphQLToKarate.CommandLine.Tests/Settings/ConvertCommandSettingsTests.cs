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
        bool fileExistsReturn,
        string? expectedMessage)
    {
        // arrange
        _mockFile!.Exists(graphQLSchemaFile).Returns(fileExistsReturn);

        var settings = new ConvertCommandSettings(_mockFile) { InputFile = graphQLSchemaFile };

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
                true,
                "Please provide a valid file path and file name for the GraphQL schema to convert."
            ).SetName("When InputFile name is null, settings are invalid.");

            yield return new TestCaseData(
                string.Empty,
                true,
                "Please provide a valid file path and file name for the GraphQL schema to convert."
            ).SetName("When InputFile name is empty, settings are invalid.");

            yield return new TestCaseData(
                "schema.graphql",
                false,
                "GraphQL schema file does not exist. Please provide a valid file path and file name for the GraphQL schema to convert."
            ).SetName("When non-null and non-empty InputFile name points to file that doesn't exist, settings are invalid.");

            yield return new TestCaseData(
                "schema.graphql",
                true,
                null
            ).SetName("When non-null and non-empty InputFile name points to file that exists, settings are valid.");
        }
    }
}