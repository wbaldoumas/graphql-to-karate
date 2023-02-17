using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Exceptions;
using GraphQLToKarate.Library.Tokens;
using NSubstitute;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Converters;

[TestFixture]
internal sealed class GraphQLScalarToExampleValueConverterTests
{
    private GraphQLScalarToExampleValueConverter? _subjectUnderTest;
    private IGraphQLDocumentAdapter? _mockGraphQLDocumentAdapter;

    [SetUp]
    public void SetUp()
    {
        _subjectUnderTest = new GraphQLScalarToExampleValueConverter();
        _mockGraphQLDocumentAdapter = Substitute.For<IGraphQLDocumentAdapter>();
    }

    [Test]
    [TestCase(GraphQLToken.Id)]
    [TestCase(GraphQLToken.String)]
    public void Convert_generates_string_when_GraphQLType_is_String_or_Id(string graphQLTypeName)
    {
        // arrange
        var graphQLType = new GraphQLNamedType
        {
            Name = new GraphQLName(graphQLTypeName)
        };

        // act
        var exampleValue = _subjectUnderTest!.Convert(graphQLType, _mockGraphQLDocumentAdapter!);

        // assert
        exampleValue.Length.Should().BeGreaterThan(2, "because it should be a value surrounded by quotes.");
        exampleValue.Should().StartWith("\"").And.EndWith("\"");
    }

    [Test]
    [TestCase(GraphQLToken.Int, "^\\d+$")]
    [TestCase(GraphQLToken.Float, "^\\d+\\.\\d{2}$")]
    public void Convert_generates_number_when_GraphQLType_is_Int_or_Float(string graphQLTypeName, string expectedRegexMatch)
    {
        // arrange
        var graphQLType = new GraphQLNamedType
        {
            Name = new GraphQLName(graphQLTypeName)
        };

        // act
        var exampleValue = _subjectUnderTest!.Convert(graphQLType, _mockGraphQLDocumentAdapter!);

        // assert
        exampleValue.Should().MatchRegex(expectedRegexMatch);
    }

    [Test]
    public void Convert_generates_boolean_when_GraphQLType_is_Boolean()
    {
        // arrange
        var graphQLType = new GraphQLNamedType
        {
            Name = new GraphQLName(GraphQLToken.Boolean)
        };

        // act
        var exampleValue = _subjectUnderTest!.Convert(graphQLType, _mockGraphQLDocumentAdapter!);

        // assert
        exampleValue.Should().BeOneOf("true", "false");
    }

    [Test]
    public void Convert_generates_enum_when_GraphQLType_is_Enum()
    {
        // arrange
        var graphQLType = new GraphQLNamedType
        {
            Name = new GraphQLName("MyEnum")
        };

        var graphQLEnumType = new GraphQLEnumTypeDefinition
        {
            Name = new GraphQLName("MyEnum"),
            Values = new GraphQLEnumValuesDefinition
            {
                Items = new List<GraphQLEnumValueDefinition>
                {
                    new()
                    {
                        Name = new GraphQLName("Value1")
                    },
                    new()
                    {
                        Name = new GraphQLName("Value2")
                    }
                }
            }
        };

        _mockGraphQLDocumentAdapter!
            .IsGraphQLEnumTypeDefinition(graphQLType.Name.StringValue)
            .Returns(true);

        _mockGraphQLDocumentAdapter
            .GetGraphQLEnumTypeDefinition(graphQLType.Name.StringValue)
            .Returns(graphQLEnumType);

        // act
        var exampleValue = _subjectUnderTest!.Convert(graphQLType, _mockGraphQLDocumentAdapter!);

        // assert
        exampleValue.Should().BeOneOf("Value1", "Value2");
    }

    [Test]
    public void Convert_throws_exception_when_enum_not_found()
    {
        // arrange
        var graphQLType = new GraphQLNamedType
        {
            Name = new GraphQLName("MyEnum")
        };

        _mockGraphQLDocumentAdapter!
            .IsGraphQLEnumTypeDefinition(graphQLType.Name.StringValue)
            .Returns(true);

        _mockGraphQLDocumentAdapter
            .GetGraphQLEnumTypeDefinition(graphQLType.Name.StringValue)
            .Returns((GraphQLEnumTypeDefinition?) null);

        // act
        var act = () => _subjectUnderTest!.Convert(graphQLType, _mockGraphQLDocumentAdapter!);

        // assert
        act.Should().Throw<InvalidGraphQLTypeException>();
    }

    [Test]
    public void Convert_throws_exception_when_enum_values_are_empty()
    {
        // arrange
        var graphQLType = new GraphQLNamedType
        {
            Name = new GraphQLName("MyEnum")
        };

        var graphQLEnumType = new GraphQLEnumTypeDefinition
        {
            Name = new GraphQLName("MyEnum"),
            Values = null
        };

        _mockGraphQLDocumentAdapter!
            .IsGraphQLEnumTypeDefinition(graphQLType.Name.StringValue)
            .Returns(true);

        _mockGraphQLDocumentAdapter
            .GetGraphQLEnumTypeDefinition(graphQLType.Name.StringValue)
            .Returns(graphQLEnumType);

        // act
        var act = () => _subjectUnderTest!.Convert(graphQLType, _mockGraphQLDocumentAdapter!);

        // assert
        act.Should().Throw<InvalidGraphQLTypeException>();
    }
}
