using System.IO.Abstractions;
using FluentAssertions;
using GraphQLToKarate.Library.Mappings;
using NSubstitute;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Mappings;

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

    private const string CustomScalarMappingAsText = " DateTime:string, Long: number,URL :string ";

    private const string CustomScalarMappingAsShortText = "DateTime:string";

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
    [TestCase("  : ,  : , :", false)]
    [TestCase("some random text xyz {123}", false)]
    [TestCase(CustomScalarMappingAsJson, true)]
    [TestCase(CustomScalarMappingAsText, true)]
    [TestCase(CustomScalarMappingAsShortText, true)]
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
    [TestCase("  : ,  : , :", false)]
    [TestCase("some,random,text", false)]
    [TestCase(CustomScalarMappingAsText, true)]
    [TestCase(CustomScalarMappingAsShortText, true)]
    public void IsTextLoadable_returns_expected_result_based_on_text_content(
        string textContent,
        bool expectedIsTextLoadable)
    {
        // act
        var result = _subjectUnderTest!.IsTextLoadable(textContent);

        // assert
        result.Should().Be(expectedIsTextLoadable);
    }

    [Test]
    [TestCaseSource(nameof(LoadFromFileTestCases))]
    public void LoadFromFile_returns_expected_result_when_file_exists_and_contains_expected_content(
        string fileContent,
        CustomScalarMapping expectedCustomScalarMapping)
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
        customScalarMapping.Should().BeEquivalentTo(expectedCustomScalarMapping);
    }

    [Test]
    [TestCaseSource(nameof(LoadFromFileTestCases))]
    public async Task LoadFromFileAsync_returns_expected_result_when_file_exists_and_contains_expected_content(
        string fileContent,
        CustomScalarMapping expectedCustomScalarMapping)
    {
        // arrange
        _mockFile!
            .Exists(Arg.Any<string>())
            .Returns(true);

        _mockFile!
            .ReadAllTextAsync(Arg.Any<string>())
            .Returns(fileContent);

        // act
        var customScalarMapping = await _subjectUnderTest!.LoadFromFileAsync("some-file-path");

        // assert
        customScalarMapping.Should().BeEquivalentTo(expectedCustomScalarMapping);
    }

    [Test]
    [TestCaseSource(nameof(LoadFromTextTestCases))]
    public void LoadFromText_returns_expected_result_when_text_contains_expected_content(
        string textContent,
        CustomScalarMapping expectedCustomScalarMapping)
    {
        // act
        var customScalarMapping = _subjectUnderTest!.LoadFromText(textContent);

        // assert
        customScalarMapping.Should().BeEquivalentTo(expectedCustomScalarMapping);
    }

    private static readonly CustomScalarMapping MultiCustomScalarMapping = new(
        new Dictionary<string, string>
        {
            { "DateTime", "string" },
            { "Long", "number" },
            { "URL", "string" }
        }
    );

    private static readonly CustomScalarMapping SingleCustomScalarMapping = new(
        new Dictionary<string, string>
        {
            { "DateTime", "string" }
        }
    );

    private static IEnumerable<TestCaseData> LoadFromFileTestCases
    {
        get
        {
            yield return new TestCaseData(CustomScalarMappingAsJson, MultiCustomScalarMapping);

            yield return new TestCaseData(CustomScalarMappingAsText, MultiCustomScalarMapping);

            yield return new TestCaseData(CustomScalarMappingAsShortText, SingleCustomScalarMapping);
        }
    }

    private static IEnumerable<TestCaseData> LoadFromTextTestCases
    {
        get
        {
            yield return new TestCaseData(CustomScalarMappingAsText, MultiCustomScalarMapping);

            yield return new TestCaseData(CustomScalarMappingAsShortText, SingleCustomScalarMapping);
        }
    }
}