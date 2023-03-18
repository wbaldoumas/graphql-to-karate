using FluentAssertions;
using GraphQLToKarate.Library.Mappings;
using NSubstitute;
using NUnit.Framework;
using System.IO.Abstractions;

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
    public void IsValid_returns_false_when_file_does_not_exist()
    {
        // arrange
        _mockFile!
            .Exists(Arg.Any<string>())
            .Returns(false);

        // act
        var isValid = _subjectUnderTest!.IsValid("some-file-path");

        // assert
        isValid.Should().BeFalse();
    }

    [Test]
    [TestCase("  : ,  : , :", false)]
    [TestCase("some random text xyz {123}", false)]
    [TestCase("", true)]
    [TestCase(CustomScalarMappingAsJson, true)]
    [TestCase(CustomScalarMappingAsText, true)]
    [TestCase(CustomScalarMappingAsShortText, true)]
    public void IsValid_returns_expected_result_based_on_file_content(
        string fileContent,
        bool expectedIsValid)
    {
        // arrange
        _mockFile!
            .Exists(Arg.Any<string>())
            .Returns(true);

        _mockFile!
            .ReadAllText(Arg.Any<string>())
            .Returns(fileContent);

        // act
        var isValid = _subjectUnderTest!.IsValid("some-file-path");

        // assert
        isValid.Should().Be(expectedIsValid);
    }

    [Test]
    [TestCase("  : ,  : , :", false)]
    [TestCase("some,random,text", false)]
    [TestCase("", true)]
    [TestCase(CustomScalarMappingAsText, true)]
    [TestCase(CustomScalarMappingAsShortText, true)]
    public void IsValid_returns_expected_result_based_on_text_content(
        string textContent,
        bool expectedIsValid)
    {
        // act
        var result = _subjectUnderTest!.IsValid(textContent);

        // assert
        result.Should().Be(expectedIsValid);
    }

    [Test]
    [TestCase(null)]
    [TestCase("invalid")]
    public async Task LoadAsync_returns_empty_custom_scalar_mapping_when_given_invalid_input(string invalidInput)
    {
        // act
        var customScalarMapping = await _subjectUnderTest!.LoadAsync(invalidInput);

        // assert
        customScalarMapping.Should().BeEquivalentTo(new CustomScalarMapping());
    }

    [Test]
    [TestCaseSource(nameof(LoadFromFileTestCases))]
    public async Task LoadAsync_returns_expected_result_when_file_exists_and_contains_expected_content(
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
        var customScalarMapping = await _subjectUnderTest!.LoadAsync("some-file-path");

        // assert
        customScalarMapping.Should().BeEquivalentTo(expectedCustomScalarMapping);
    }

    [Test]
    [TestCaseSource(nameof(LoadFromTextTestCases))]
    public async Task LoadAsync_returns_expected_result_when_text_contains_expected_content(
        string textContent,
        CustomScalarMapping expectedCustomScalarMapping)
    {
        // act
        var customScalarMapping = await _subjectUnderTest!.LoadAsync(textContent);

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