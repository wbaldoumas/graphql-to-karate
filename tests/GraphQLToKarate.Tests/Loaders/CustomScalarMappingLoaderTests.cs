using NUnit.Framework;
using System.IO.Abstractions;
using FluentAssertions;
using GraphQLToKarate.Library.Mappings;
using NSubstitute;

namespace GraphQLToKarate.Tests.Loaders;

[TestFixture]
internal sealed class CustomScalarMappingLoaderTests
{
    private IFile? _mockFile;
    private ICustomScalarMappingLoader? _subjectUnderTest;

    [SetUp]
    public void SetUp()
    {
        _mockFile = Substitute.For<IFile>();
        _subjectUnderTest = new CustomScalarMappingLoader(_mockFile);
    }

    private const string CustomScalarMappingAsJson = 
        """
        {
            "DateTime": "string",
            "Long": "number",
            "URL": "string"
        }
        """;

    private const string CustomScalarMappingAsText = " DateTime:string, Long: number,URL :string "; // messy user input with spaces

    [Test]
    public void IsFileLoadable_returns_false_when_file_does_not_exist()
    {
        // arrange
        _mockFile!
            .Exists(Arg.Any<string>())
            .Returns(false);

        // act
        var isFileLoadable = _subjectUnderTest!.IsFileLoadable("some-file-path");

        // assert
        isFileLoadable.Should().BeFalse();
    }

    [Test]
    [TestCase("", false)]
    [TestCase("some random text xyz {123}", false)]
    [TestCase(CustomScalarMappingAsJson, true)]
    [TestCase(CustomScalarMappingAsText, true)]
    public void IsFileLoadable_returns_expected_result_based_on_file_content(
        string fileContent, 
        bool expectedIsFileLoadable)
    {
        // arrange
        _mockFile!
            .Exists(Arg.Any<string>())
            .Returns(true);

        _mockFile!
            .ReadAllText(Arg.Any<string>())
            .Returns(fileContent);

        // act
        var isFileLoadable = _subjectUnderTest!.IsFileLoadable("some-file-path");

        // assert
        isFileLoadable.Should().Be(expectedIsFileLoadable);
    }

    [Test]
    [TestCase("", false)]
    [TestCase("some,random,text", false)]
    [TestCase(CustomScalarMappingAsText, true)]
    public void IsFileLoadable_returns_expected_result_based_on_text_content(
        string textContent,
        bool expectedIsTextLoadable)
    {
        // act
        var result = _subjectUnderTest!.IsTextLoadable(textContent);

        // assert
        result.Should().Be(expectedIsTextLoadable);
    }

    [Test]
    [TestCase(CustomScalarMappingAsJson)]
    [TestCase(CustomScalarMappingAsText)]
    public void LoadFromFile_returns_expected_result_when_file_exists_and_contains_expected_content(
        string fileContent)
    {
        // arrange
        _mockFile!
            .Exists(Arg.Any<string>())
            .Returns(true);

        _mockFile!
            .ReadAllText(Arg.Any<string>())
            .Returns(fileContent);

        // act
        var customScalarMapping = _subjectUnderTest!.LoadFromFile("some-file-path");

        // assert
        customScalarMapping.Should().BeEquivalentTo(
            new Dictionary<string, string>
            {
                { "DateTime", "string" },
                { "Long", "number" },
                { "URL", "string" }
            }
        );
    }

    [Test]
    [TestCase(CustomScalarMappingAsText)]
    public void LoadFromText_returns_expected_result_when_text_contains_expected_content(
        string textContent)
    {
        // act
        var customScalarMapping = _subjectUnderTest!.LoadFromText(textContent);

        // assert
        customScalarMapping.Should().BeEquivalentTo(
            new Dictionary<string, string>
            {
                { "DateTime", "string" },
                { "Long", "number" },
                { "URL", "string" }
            }
        );
    }
}