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

            var testGraphQLObjectDefinitionWithNestedFieldArguments = new GraphQLObjectTypeDefinition
            {
                Name = new GraphQLName("PersonWithFriendsWithArguments"),
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
                            },
                            Arguments = new GraphQLArgumentsDefinition
                            {
                                Items = new List<GraphQLInputValueDefinition>
                                {
                                    new()
                                    {
                                        Name = new GraphQLName("ids"),
                                        Type = new GraphQLListType
                                        {
                                            Type = new GraphQLNamedType
                                            {
                                                Name = new GraphQLName("String")
                                            }
                                        }
                                    },
                                    new()
                                    {
                                        Name = new GraphQLName("location"),
                                        Type = new GraphQLNonNullType
                                        {
                                            Type = new GraphQLNamedType
                                            {
                                                Name = new GraphQLName("String")
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var testGraphQLObjectDefinitionWithScalarFieldArguments = new GraphQLObjectTypeDefinition
            {
                Name = new GraphQLName("PersonWithFavoriteColors"),
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
                            Name = new GraphQLName("favoriteColors"),
                            Type = new GraphQLListType
                            {
                                Type = new GraphQLNamedType
                                {
                                    Name = new GraphQLName("Color")
                                }
                            },
                            Arguments = new GraphQLArgumentsDefinition
                            {
                                Items = new List<GraphQLInputValueDefinition>
                                {
                                    new()
                                    {
                                        Name = new GraphQLName("filter"),
                                        Type = new GraphQLListType
                                        {
                                            Type = new GraphQLNamedType
                                            {
                                                Name = new GraphQLName("String")
                                            }
                                        }
                                    }
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

            var testGraphQLUserDefinedTypes = new GraphQLUserDefinedTypes
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
                        testGraphQLObjectDefinitionWithNestedFieldArguments.Name.StringValue,
                        testGraphQLObjectDefinitionWithNestedFieldArguments
                    },
                    {
                        testGraphQLObjectDefinitionWithNesting.Name.StringValue,
                        testGraphQLObjectDefinitionWithNesting
                    },
                    {
                        testGraphQLObjectDefinitionWithScalarFieldArguments.Name.StringValue,
                        testGraphQLObjectDefinitionWithScalarFieldArguments
                    }
                }
            };

            yield return new TestCaseData(
                testGraphQLFieldDefinition,
                testGraphQLUserDefinedTypes,
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
                testGraphQLUserDefinedTypes,
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

            var testGraphQLFieldDefinitionWithArguments = new GraphQLFieldDefinition
            {
                Name = new GraphQLName("personById"),
                Type = new GraphQLNamedType
                {
                    Name = new GraphQLName("Person")
                },
                Arguments = new GraphQLArgumentsDefinition
                {
                    Items = new List<GraphQLInputValueDefinition>
                    {
                        new()
                        {
                            Name = new GraphQLName("id"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName("Int")
                            }
                        }
                    }
                }
            };

            yield return new TestCaseData(
                testGraphQLFieldDefinitionWithArguments,
                testGraphQLUserDefinedTypes,
                new GraphQLQueryFieldType(testGraphQLFieldDefinitionWithArguments)
                {
                    QueryString = """
                                query PersonByIdTest($id: Int) {
                                  personById(id: $id) {
                                    id
                                    name
                                    favoriteNumber
                                    favoriteColor
                                  }
                                }
                                """
                }
            ).SetName("Converter is able to convert simple query with arguments.");

            var testGraphQLFieldDefinitionWithNestedArguments = new GraphQLFieldDefinition
            {
                Name = new GraphQLName("personWithFriendsById"),
                Type = new GraphQLNamedType
                {
                    Name = new GraphQLName("PersonWithFriendsWithArguments")
                },
                Arguments = new GraphQLArgumentsDefinition
                {
                    Items = new List<GraphQLInputValueDefinition>
                    {
                        new()
                        {
                            Name = new GraphQLName("id"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName("Int")
                            }
                        }
                    }
                }
            };

            var testGraphQLFieldDefinitionWithScalarArguments = new GraphQLFieldDefinition()
            {
                Name = new GraphQLName("personWithFavoriteColors"),
                Type = new GraphQLNamedType
                {
                    Name = new GraphQLName("PersonWithFavoriteColors")
                }
            };

            yield return new TestCaseData(
                testGraphQLFieldDefinitionWithNestedArguments,
                testGraphQLUserDefinedTypes,
                new GraphQLQueryFieldType(testGraphQLFieldDefinitionWithNestedArguments)
                {
                    QueryString = """
                                query PersonWithFriendsByIdTest($id: Int, $ids: [String], $location: String!) {
                                  personWithFriendsById(id: $id) {
                                    id
                                    name
                                    friends(ids: $ids, location: $location) {
                                      id
                                      name
                                      favoriteNumber
                                      favoriteColor
                                    }
                                  }
                                }
                                """
                }
            ).SetName("Converter is able to convert simple query with nested arguments.");


            yield return new TestCaseData(
                testGraphQLFieldDefinitionWithScalarArguments,
                testGraphQLUserDefinedTypes,
                new GraphQLQueryFieldType(testGraphQLFieldDefinitionWithScalarArguments)
                {
                    QueryString = """
                                query PersonWithFavoriteColorsTest($filter: [String]) {
                                  personWithFavoriteColors {
                                    id
                                    name
                                    favoriteNumber
                                    favoriteColors(filter: $filter)
                                  }
                                }
                                """
                }
            ).SetName("Converter is able to convert simple query with scalar arguments.");
        }
    }
}