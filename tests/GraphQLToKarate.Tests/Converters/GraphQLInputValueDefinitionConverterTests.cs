using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Types;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Converters;

[TestFixture]
internal sealed class GraphQLInputValueDefinitionConverterTests
{
    [TestCaseSource(nameof(ConvertTestCases))]
    public void Convert_ReturnsExpectedResult(
        GraphQLInputValueDefinition inputValueDefinition,
        Type expectedType,
        string expectedVariableName,
        string expectedVariableTypeName)
    {
        // arrange
        var converter = new GraphQLInputValueDefinitionConverter();

        // act
        var result = converter.Convert(inputValueDefinition);

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

        var converter = new GraphQLInputValueDefinitionConverter();

        // act
        var result1 = converter.Convert(inputValueDefinition1);
        var result2 = converter.Convert(inputValueDefinition2);
        var result3 = converter.Convert(inputValueDefinition3);

        // assert
        result1.VariableName.Should().Be("age");
        result2.VariableName.Should().Be("age1");
        result3.VariableName.Should().Be("age2");
    }
}