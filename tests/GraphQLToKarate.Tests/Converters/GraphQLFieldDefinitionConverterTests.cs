using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Types;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Converters;

[TestFixture]
internal sealed class GraphQLFieldDefinitionConverterTests
{
    private IGraphQLFieldDefinitionConverter? _subjectUnderTest;

    [SetUp]
    public void SetUp() => _subjectUnderTest = new GraphQLFieldDefinitionConverter();

    [Test]
    [TestCaseSource(nameof(TestCases))]
    public void Convert(
        GraphQLFieldDefinition graphQLQueryFieldDefinition,
        IGraphQLDocumentAdapter graphQLUeDefinedTypes,
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
            var person = new GraphQLObjectTypeDefinition
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

            var personWithFriends = new GraphQLObjectTypeDefinition
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
                                    Name = new GraphQLName(person.Name)
                                }
                            }
                        }
                    }
                }
            };

            var personWithFriendsWithArguments = new GraphQLObjectTypeDefinition
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
                                    Name = new GraphQLName(person.Name)
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

            var personWithFavoriteColorsWithArguments = new GraphQLObjectTypeDefinition
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

            var personInterface = new GraphQLInterfaceTypeDefinition
            {
                Name = new GraphQLName("PersonInterface"),
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
                        }
                    }
                }
            };

            var personInterfaceWithFriend = new GraphQLInterfaceTypeDefinition
            {
                Name = new GraphQLName("NestedPersonInterface"),
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
                        new ()
                        {
                            Name = new GraphQLName("friend"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName(personInterface.Name)
                            }
                        }
                    }
                }
            };

            var colors = new GraphQLEnumTypeDefinition
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

            var graphQLDocument = new GraphQLDocument
            {
                Definitions = new List<ASTNode>
                {
                    colors,
                    person,
                    personWithFriends,
                    personWithFriendsWithArguments,
                    personWithFavoriteColorsWithArguments,
                    personInterface,
                    personInterfaceWithFriend
                }
            };

            var graphQLDocumentAdapter = new GraphQLDocumentAdapter(graphQLDocument);

            var personQueryFieldDefinition = new GraphQLFieldDefinition
            {
                Name = new GraphQLName("person"),
                Type = new GraphQLNamedType
                {
                    Name = new GraphQLName(person.Name)
                }
            };

            yield return new TestCaseData(
                personQueryFieldDefinition,
                graphQLDocumentAdapter,
                new GraphQLQueryFieldType(personQueryFieldDefinition)
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

            var personWithFriendsQueryFieldDefinition = new GraphQLFieldDefinition
            {
                Name = new GraphQLName("personWithFriends"),
                Type = new GraphQLNamedType
                {
                    Name = new GraphQLName(personWithFriends.Name)
                }
            };

            yield return new TestCaseData(
                personWithFriendsQueryFieldDefinition,
                graphQLDocumentAdapter,
                new GraphQLQueryFieldType(personWithFriendsQueryFieldDefinition)
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

            var personWithFriendsWithArgumentsGraphQLQueryFieldDefinition = new GraphQLFieldDefinition
            {
                Name = new GraphQLName("personById"),
                Type = new GraphQLNamedType
                {
                    Name = new GraphQLName(person.Name)
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
                personWithFriendsWithArgumentsGraphQLQueryFieldDefinition,
                graphQLDocumentAdapter,
                new GraphQLQueryFieldType(personWithFriendsWithArgumentsGraphQLQueryFieldDefinition)
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

            var personWithFriendsWithNestedArgumentsGraphQLQueryFieldDefinition = new GraphQLFieldDefinition
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

            var personWithFavoriteColorsGraphQLQueryFieldDefinition = new GraphQLFieldDefinition
            {
                Name = new GraphQLName("personWithFavoriteColors"),
                Type = new GraphQLNamedType
                {
                    Name = new GraphQLName(personWithFavoriteColorsWithArguments.Name)
                }
            };

            yield return new TestCaseData(
                personWithFriendsWithNestedArgumentsGraphQLQueryFieldDefinition,
                graphQLDocumentAdapter,
                new GraphQLQueryFieldType(personWithFriendsWithNestedArgumentsGraphQLQueryFieldDefinition)
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
            ).SetName("Converter is able to convert nested query with nested field arguments.");


            yield return new TestCaseData(
                personWithFavoriteColorsGraphQLQueryFieldDefinition,
                graphQLDocumentAdapter,
                new GraphQLQueryFieldType(personWithFavoriteColorsGraphQLQueryFieldDefinition)
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
            ).SetName("Converter is able to convert simple query with scalar field arguments.");


            var personInterfaceQueryGraphQLFieldDefinition = new GraphQLFieldDefinition
            {
                Name = new GraphQLName("personInterface"),
                Type = new GraphQLNamedType
                {
                    Name = new GraphQLName(personInterface.Name)
                }
            };

            yield return new TestCaseData(
                personInterfaceQueryGraphQLFieldDefinition,
                graphQLDocumentAdapter,
                new GraphQLQueryFieldType(personInterfaceQueryGraphQLFieldDefinition)
                {
                    QueryString = """
                                query PersonInterfaceTest {
                                  personInterface {
                                    id
                                    name
                                  }
                                }
                                """
                }
            ).SetName("Converter is able to convert simple query with interface return type.");

            var nestedPersonInterfaceGraphQLQueryFieldDefinition = new GraphQLFieldDefinition
            {
                Name = new GraphQLName("nestedPersonInterface"),
                Type = new GraphQLNamedType
                {
                    Name = new GraphQLName(personInterfaceWithFriend.Name)
                }
            };

            yield return new TestCaseData(
                nestedPersonInterfaceGraphQLQueryFieldDefinition,
                graphQLDocumentAdapter,
                new GraphQLQueryFieldType(nestedPersonInterfaceGraphQLQueryFieldDefinition)
                {
                    QueryString = """
                                query NestedPersonInterfaceTest {
                                  nestedPersonInterface {
                                    id
                                    name
                                    friend {
                                      id
                                      name
                                    }
                                  }
                                }
                                """
                }
            ).SetName("Converter is able to convert simple query with nested interface return type.");
        }
    }
}