using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Exceptions;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Mappings;
using GraphQLToKarate.Library.Tokens;
using NSubstitute;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Converters;

[TestFixture]
internal sealed class GraphQLScalarToExampleValueConverterTests
{
    private GraphQLScalarToExampleValueConverter? _subjectUnderTest;
    private IGraphQLDocumentAdapter? _mockGraphQLDocumentAdapter;
    private ICustomScalarMapping? _mockCustomScalarMapping;

    [SetUp]
    public void SetUp()
    {
        _mockCustomScalarMapping = Substitute.For<ICustomScalarMapping>();
        _subjectUnderTest = new GraphQLScalarToExampleValueConverter(_mockCustomScalarMapping);
        _mockGraphQLDocumentAdapter = Substitute.For<IGraphQLDocumentAdapter>();
    }

    [Test]
    [TestCase(GraphQLToken.Id)]
    [TestCase(GraphQLToken.String)]
    public void Convert_generates_string_when_GraphQLType_is_String_or_Id(string graphQLTypeName)
    {
        // arrange
        var graphQLType = new GraphQLNamedType(new GraphQLName(graphQLTypeName));

        // act
        var exampleValue = _subjectUnderTest!.Convert(graphQLType, _mockGraphQLDocumentAdapter!);

        // assert
        exampleValue.Length.Should().BeGreaterThan(2, "because it should be a value surrounded by quotes.");
        exampleValue.Should().StartWith("\"").And.EndWith("\"");
    }

    [Test]
    [TestCase(GraphQLToken.Int, "^\\d+$")]
    [TestCase(GraphQLToken.Float, "^\\d+\\.\\d+$")]
    public void Convert_generates_number_when_GraphQLType_is_Int_or_Float(
        string graphQLTypeName,
        string expectedRegexMatch)
    {
        // arrange
        var graphQLType = new GraphQLNamedType(new GraphQLName(graphQLTypeName));

        // act
        var exampleValue = _subjectUnderTest!.Convert(graphQLType, _mockGraphQLDocumentAdapter!);

        // assert
        exampleValue.Should().MatchRegex(expectedRegexMatch);
    }

    [Test]
    public void Convert_generates_boolean_when_GraphQLType_is_Boolean()
    {
        // arrange
        var graphQLType = new GraphQLNamedType(new GraphQLName(GraphQLToken.Boolean));

        // act
        var exampleValue = _subjectUnderTest!.Convert(graphQLType, _mockGraphQLDocumentAdapter!);

        // assert
        exampleValue.Should().BeOneOf("true", "false");
    }

    [Test]
    public void Convert_generates_enum_when_GraphQLType_is_Enum()
    {
        // arrange
        var graphQLType = new GraphQLNamedType(new GraphQLName("MyEnum"));

        var graphQLEnumType = new GraphQLEnumTypeDefinition(new GraphQLName("MyEnum"))
        {
            Values = new GraphQLEnumValuesDefinition(new List<GraphQLEnumValueDefinition>
            {
                new(
                    new GraphQLName("Value1"),
                    new GraphQLEnumValue(new GraphQLName("one"))
                ),
                new(
                    new GraphQLName("Value2"),
                    new GraphQLEnumValue(new GraphQLName("two"))
                )
            })
        };

        _mockGraphQLDocumentAdapter!
            .IsGraphQLEnumTypeDefinition(graphQLType.NameValue())
            .Returns(true);

        _mockGraphQLDocumentAdapter
            .GetGraphQLEnumTypeDefinition(graphQLType.NameValue())
            .Returns(graphQLEnumType);

        // act
        var exampleValue = _subjectUnderTest!.Convert(graphQLType, _mockGraphQLDocumentAdapter!);

        // assert
        exampleValue.Should().BeOneOf("Value1", "Value2");
    }

    [Test]
    public void Convert_generates_expected_example_value_from_custom_scalar_mapping_that_maps_to_number()
    {
        // arrange
        var graphQLType = new GraphQLNamedType(new GraphQLName("MyCustomScalar"));

        _mockCustomScalarMapping!
            .TryGetKarateType(graphQLType.NameValue(), out Arg.Any<string>()!)
            .Returns(callInfo =>
                {
                    callInfo[1] = KarateToken.Number;

                    return true;
                }
            );

        // act
        var exampleValue = _subjectUnderTest!.Convert(graphQLType, _mockGraphQLDocumentAdapter!);

        // assert
        exampleValue.Should().MatchRegex("^\\d+$");
    }

    [Test]
    public void Convert_generates_expected_example_value_from_custom_scalar_mapping_that_maps_to_string()
    {
        // arrange
        var graphQLType = new GraphQLNamedType(new GraphQLName("MyCustomScalar"));

        _mockCustomScalarMapping!
            .TryGetKarateType(graphQLType.NameValue(), out Arg.Any<string>()!)
            .Returns(callInfo =>
                {
                    callInfo[1] = KarateToken.String;

                    return true;
                }
            );

        // act
        var exampleValue = _subjectUnderTest!.Convert(graphQLType, _mockGraphQLDocumentAdapter!);

        // assert
        exampleValue.Length.Should().BeGreaterThan(2, "because it should be a value surrounded by quotes.");
    }

    [Test]
    public void Convert_generates_expected_example_value_from_custom_scalar_mapping_that_maps_to_boolean()
    {
        // arrange
        var graphQLType = new GraphQLNamedType(new GraphQLName("MyCustomScalar"));

        _mockCustomScalarMapping!
            .TryGetKarateType(graphQLType.NameValue(), out Arg.Any<string>()!)
            .Returns(callInfo =>
                {
                    callInfo[1] = KarateToken.Boolean;

                    return true;
                }
            );

        // act
        var exampleValue = _subjectUnderTest!.Convert(graphQLType, _mockGraphQLDocumentAdapter!);

        // assert
        exampleValue.Should().BeOneOf("true", "false");
    }

    [Test]
    public void Convert_generates_default_example_value_from_custom_scalar_mapping_that_maps_to_unknown_type()
    {
        // arrange
        var graphQLType = new GraphQLNamedType(new GraphQLName("MyCustomScalar"));

        _mockCustomScalarMapping!
            .TryGetKarateType(graphQLType.NameValue(), out Arg.Any<string>()!)
            .Returns(callInfo =>
                {
                    callInfo[1] = "UnknownType";

                    return true;
                }
            );

        // act
        var exampleValue = _subjectUnderTest!.Convert(graphQLType, _mockGraphQLDocumentAdapter!);

        // assert
        exampleValue.Should().Be("<some value>");
    }

    [Test]
    public void Convert_throws_exception_when_enum_not_found()
    {
        // arrange
        var graphQLType = new GraphQLNamedType(new GraphQLName("MyEnum"));

        _mockGraphQLDocumentAdapter!
            .IsGraphQLEnumTypeDefinition(graphQLType.NameValue())
            .Returns(true);

        _mockGraphQLDocumentAdapter
            .GetGraphQLEnumTypeDefinition(graphQLType.NameValue())
            .Returns((GraphQLEnumTypeDefinition?)null);

        // act
        var act = () => _subjectUnderTest!.Convert(graphQLType, _mockGraphQLDocumentAdapter!);

        // assert
        act.Should().Throw<InvalidGraphQLTypeException>();
    }

    [Test]
    public void Convert_throws_exception_when_enum_values_are_empty()
    {
        // arrange
        var graphQLType = new GraphQLNamedType(new GraphQLName("MyEnum"));

        var graphQLEnumType = new GraphQLEnumTypeDefinition(new GraphQLName("MyEnum"))
        {
            Values = null
        };

        _mockGraphQLDocumentAdapter!
            .IsGraphQLEnumTypeDefinition(graphQLType.NameValue())
            .Returns(true);

        _mockGraphQLDocumentAdapter
            .GetGraphQLEnumTypeDefinition(graphQLType.NameValue())
            .Returns(graphQLEnumType);

        // act
        var act = () => _subjectUnderTest!.Convert(graphQLType, _mockGraphQLDocumentAdapter!);

        // assert
        act.Should().Throw<InvalidGraphQLTypeException>();
    }

    [Test]
    public void Convert_throws_exception_when_GraphQLType_is_not_a_scalar()
    {
        // arrange
        var graphQLType = new GraphQLNamedType(new GraphQLName("MyType"));

        _mockGraphQLDocumentAdapter!
            .IsGraphQLEnumTypeDefinition(graphQLType.NameValue())
            .Returns(false);

        // act
        var exampleValue = _subjectUnderTest!.Convert(graphQLType, _mockGraphQLDocumentAdapter!);

        // assert
        exampleValue.Should().Be("<some value>");
    }
}