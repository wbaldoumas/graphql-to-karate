using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Types;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Converters;

[TestFixture]
internal sealed class GraphQLQueryFieldConverterTests
{
    private IGraphQLQueryFieldConverter? _subjectUnderTest;

    [SetUp]
    public void SetUp() => _subjectUnderTest = new GraphQLQueryFieldConverter();

    [Test]
    [TestCaseSource(nameof(TestCases))]
    public void Convert(
        GraphQLFieldDefinition graphQLQueryFieldDefinition,
        GraphQLUserDefinedTypes graphQLUeDefinedTypes,
        GraphQLQueryFieldType expectedGraphQLQueryFieldType)
    {
        // act
        var graphQLQueryFieldType = _subjectUnderTest!.Convert(
            graphQLQueryFieldDefinition,
            graphQLUeDefinedTypes
        );

        // assert
        graphQLQueryFieldType
            .Should()
            .BeEquivalentTo(expectedGraphQLQueryFieldType);
    }

    private static IEnumerable<TestCaseData> TestCases
    {
        get
        {
            var testGraphQLFieldDefinition = new GraphQLFieldDefinition
            {
                Name = new GraphQLName("person"),
                Type = new GraphQLNamedType
                {
                    Name = new GraphQLName("Person")
                }
            };

            var testGraphQLObjectDefinition = new GraphQLObjectTypeDefinition
            {
                Name = new GraphQLName("Person"),
                Fields = new GraphQLFieldsDefinition
                {
                    Items = new List<GraphQLFieldDefinition>
                    {
                        new()
                        {
                            Name = new GraphQLName("id"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName("String")
                            }
                        },
                        new()
                        {
                            Name = new GraphQLName("name"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName("String")
                            }
                        },
                        new()
                        {
                            Name = new GraphQLName("favoriteNumber"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName("Integer")
                            }
                        },
                        new()
                        {
                            Name = new GraphQLName("favoriteColor"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName("Color")
                            }
                        }
                    }
                }
            };

            var testGraphQLObjectDefinitionWithNesting = new GraphQLObjectTypeDefinition
            {
                Name = new GraphQLName("PersonWithFriends"),
                Fields = new GraphQLFieldsDefinition
                {
                    Items = new List<GraphQLFieldDefinition>
                    {
                        new()
                        {
                            Name = new GraphQLName("id"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName("String")
                            }
                        },
                        new()
                        {
                            Name = new GraphQLName("name"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName("String")
                            }
                        },
                        new()
                        {
                            Name = new GraphQLName("friends"),
                            Type = new GraphQLListType
                            {
                                Type = new GraphQLNamedType
                                {
                                    Name = new GraphQLName("Person")
                                }
                            }
                        }
                    }
                }
            };

            var testGraphQLEnumTypeDefinition = new GraphQLEnumTypeDefinition()
            {
                Name = new GraphQLName("Color"),
                Values = new GraphQLEnumValuesDefinition
                {
                    Items = new List<GraphQLEnumValueDefinition>
                    {
                        new()
                        {
                            Name = new GraphQLName("RED"),
                            EnumValue = new GraphQLEnumValue
                            {
                                Name = new GraphQLName("RED")
                            }
                        },
                        new()
                        {
                            Name = new GraphQLName("BLUE"),
                            EnumValue = new GraphQLEnumValue
                            {
                                Name = new GraphQLName("BLUE")
                            }
                        },
                        new()
                        {
                            Name = new GraphQLName("GREEN"),
                            EnumValue = new GraphQLEnumValue
                            {
                                Name = new GraphQLName("GREEN")
                            }
                        }
                    }
                }
            };

            yield return new TestCaseData(
                testGraphQLFieldDefinition,
                new GraphQLUserDefinedTypes
                {
                    GraphQLEnumTypeDefinitionsByName = new Dictionary<string, GraphQLEnumTypeDefinition>
                    {
                        {
                            testGraphQLEnumTypeDefinition.Name.StringValue, 
                            testGraphQLEnumTypeDefinition
                        }
                    },
                    GraphQLObjectTypeDefinitionsByName = new Dictionary<string, GraphQLObjectTypeDefinition>
                    {
                        {
                            testGraphQLObjectDefinition.Name.StringValue, 
                            testGraphQLObjectDefinition
                        },
                        {
                            testGraphQLObjectDefinitionWithNesting.Name.StringValue,
                            testGraphQLObjectDefinitionWithNesting
                        }
                    }
                },
                new GraphQLQueryFieldType(testGraphQLFieldDefinition)
                {
                    QueryString = """
                                query PersonTest {
                                  person {
                                    id
                                    name
                                    favoriteNumber
                                    favoriteColor
                                  }
                                }
                                """
                }
            ).SetName("Converter is able to convert simple query.");

            var testNestedGraphQLFieldDefinition = new GraphQLFieldDefinition
            {
                Name = new GraphQLName("personWithFriends"),
                Type = new GraphQLNamedType
                {
                    Name = new GraphQLName("PersonWithFriends")
                }
            };

            yield return new TestCaseData(
                testNestedGraphQLFieldDefinition,
                new GraphQLUserDefinedTypes
                {
                    GraphQLEnumTypeDefinitionsByName = new Dictionary<string, GraphQLEnumTypeDefinition>
                    {
                        {
                            testGraphQLEnumTypeDefinition.Name.StringValue,
                            testGraphQLEnumTypeDefinition
                        }
                    },
                    GraphQLObjectTypeDefinitionsByName = new Dictionary<string, GraphQLObjectTypeDefinition>
                    {
                        {
                            testGraphQLObjectDefinition.Name.StringValue,
                            testGraphQLObjectDefinition
                        },
                        {
                            testGraphQLObjectDefinitionWithNesting.Name.StringValue,
                            testGraphQLObjectDefinitionWithNesting
                        }
                    }
                },
                new GraphQLQueryFieldType(testNestedGraphQLFieldDefinition)
                {
                    QueryString = """
                                query PersonWithFriendsTest {
                                  personWithFriends {
                                    id
                                    name
                                    friends {
                                      id
                                      name
                                      favoriteNumber
                                      favoriteColor
                                    }
                                  }
                                }
                                """
                }
            ).SetName("Converter is able to convert nested query.");
        }
    }
}