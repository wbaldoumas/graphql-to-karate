using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Exceptions;
using GraphQLToKarate.Library.Types;
using GraphQLToKarate.Tests.Mocks;
using NSubstitute;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Converters;

[TestFixture]
internal sealed class GraphQLInputValueDefinitionConverterTests
{
    private IGraphQLDocumentAdapter? _mockQLDocumentAdapter;
    private IGraphQLInputValueToExampleValueConverter? _mockGraphQLInputValueToExampleValueConverter;
    private IGraphQLInputValueDefinitionConverter? _subjectUnderTest;

    [SetUp]
    public void SetUp()
    {
        _mockQLDocumentAdapter = Substitute.For<IGraphQLDocumentAdapter>();
        _mockGraphQLInputValueToExampleValueConverter = Substitute.For<IGraphQLInputValueToExampleValueConverter>();
        _subjectUnderTest = new GraphQLInputValueDefinitionConverter(_mockGraphQLInputValueToExampleValueConverter);
    }

    [TestCaseSource(nameof(ConvertTestCases))]
    public void Convert_ReturnsExpectedResult(
        GraphQLInputValueDefinition inputValueDefinition,
        Type expectedType,
        string expectedVariableName,
        string expectedVariableTypeName)
    {
        // act
        var result = _subjectUnderTest!.Convert(inputValueDefinition, _mockQLDocumentAdapter!);

        // assert
        result.Should().BeOfType(expectedType);
        result.VariableName.Should().Be(expectedVariableName);
        result.VariableTypeName.Should().Be(expectedVariableTypeName);
    }

    private static IEnumerable<TestCaseData> ConvertTestCases
    {
        get
        {
            yield return new TestCaseData(
                new GraphQLInputValueDefinition
                {
                    Name = new GraphQLName("age"),
                    Type = new GraphQLNamedType
                    {
                        Name = new GraphQLName("Int")
                    }
                },
                typeof(GraphQLArgumentType),
                "age",
                "Int"
            ).SetName("With Named Type");

            yield return new TestCaseData(
                new GraphQLInputValueDefinition
                {
                    Name = new GraphQLName("hobbies"),
                    Type = new GraphQLListType
                    {
                        Type = new GraphQLNamedType
                        {
                            Name = new GraphQLName("String")
                        }
                    }
                },
                typeof(GraphQLListArgumentType),
                "hobbies",
                "[String]"
            ).SetName("With List Type");

            yield return new TestCaseData(
                new GraphQLInputValueDefinition
                {
                    Name = new GraphQLName("email"),
                    Type = new GraphQLNonNullType
                    {
                        Type = new GraphQLNamedType
                        {
                            Name = new GraphQLName("String")
                        }
                    }
                },
                typeof(GraphQLNonNullArgumentType),
                "email",
                "String!"
            ).SetName("With Non-Null Type");

            yield return new TestCaseData(
                new GraphQLInputValueDefinition
                {
                    Name = new GraphQLName("address"),
                    Type = new GraphQLNonNullType
                    {
                        Type = new GraphQLListType
                        {
                            Type = new GraphQLNonNullType
                            {
                                Type = new GraphQLNamedType
                                {
                                    Name = new GraphQLName("String")
                                }
                            }
                        }
                    }
                },
                typeof(GraphQLNonNullArgumentType),
                "address",
                "[String!]!"
            ).SetName("With Non-Null List Type and Named Type");
        }
    }

    [Test]
    public void GetNonReservedVariableName_WithMultipleInputValueDefinitions_ReturnsUniqueVariableNames()
    {
        // arrange
        var inputValueDefinition1 = new GraphQLInputValueDefinition
        {
            Name = new GraphQLName("age"),
            Type = new GraphQLNamedType
            {
                Name = new GraphQLName("Int")
            }
        };

        var inputValueDefinition2 = new GraphQLInputValueDefinition
        {
            Name = new GraphQLName("age"),
            Type = new GraphQLNamedType
            {
                Name = new GraphQLName("Int")
            }
        };

        var inputValueDefinition3 = new GraphQLInputValueDefinition
        {
            Name = new GraphQLName("age"),
            Type = new GraphQLNamedType
            {
                Name = new GraphQLName("Int")
            }
        };

        // act
        var result1 = _subjectUnderTest!.Convert(inputValueDefinition1, _mockQLDocumentAdapter!);
        var result2 = _subjectUnderTest!.Convert(inputValueDefinition2, _mockQLDocumentAdapter!);
        var result3 = _subjectUnderTest!.Convert(inputValueDefinition3, _mockQLDocumentAdapter!);

        // assert
        result1.VariableName.Should().Be("age");
        result2.VariableName.Should().Be("age1");
        result3.VariableName.Should().Be("age2");
    }

    [Test]
    public void Convert_throws_exception_when_unsupported_graphql_type_is_encountered()
    {
        // arrange
        var unsupportedGraphQLType = new UnsupportedGraphQLType();

        var graphQLInputValueDefinition = new GraphQLInputValueDefinition
        {
            Name = new GraphQLName("unsupported"),
            Type = unsupportedGraphQLType
        };

        // act
        var act = () => _subjectUnderTest!.Convert(graphQLInputValueDefinition, _mockQLDocumentAdapter!);

        // assert
        act.Should().ThrowExactly<InvalidGraphQLTypeException>();
    }
}