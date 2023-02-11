using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;
using NSubstitute;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Converters;

[TestFixture]
internal sealed class GraphQLFieldDefinitionConverterTests
{
    private IGraphQLInputValueDefinitionConverterFactory? _mockGraphQLInputValueDefinitionConverterFactory;
    private IGraphQLFieldDefinitionConverter? _subjectUnderTest;

    [SetUp]
    public void SetUp()
    {
        _mockGraphQLInputValueDefinitionConverterFactory = Substitute.For<IGraphQLInputValueDefinitionConverterFactory>();

        _mockGraphQLInputValueDefinitionConverterFactory
            .Create()
            .Returns(new GraphQLInputValueDefinitionConverter());

        _subjectUnderTest = new GraphQLFieldDefinitionConverter(_mockGraphQLInputValueDefinitionConverterFactory);
    }

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
                                Name = new GraphQLName(GraphQLToken.String)
                            }
                        },
                        new()
                        {
                            Name = new GraphQLName("name"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName(GraphQLToken.String)
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
                                Name = new GraphQLName(GraphQLToken.String)
                            }
                        },
                        new()
                        {
                            Name = new GraphQLName("name"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName(GraphQLToken.String)
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
                                Name = new GraphQLName(GraphQLToken.String)
                            }
                        },
                        new()
                        {
                            Name = new GraphQLName("name"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName(GraphQLToken.String)
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
                                                Name = new GraphQLName(GraphQLToken.String)
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
                                                Name = new GraphQLName(GraphQLToken.String)
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
                                Name = new GraphQLName(GraphQLToken.String)
                            }
                        },
                        new()
                        {
                            Name = new GraphQLName("name"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName(GraphQLToken.String)
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
                                                Name = new GraphQLName(GraphQLToken.String)
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
                                Name = new GraphQLName(GraphQLToken.String)
                            }
                        },
                        new()
                        {
                            Name = new GraphQLName("name"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName(GraphQLToken.String)
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
                                Name = new GraphQLName(GraphQLToken.String)
                            }
                        },
                        new()
                        {
                            Name = new GraphQLName("name"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName(GraphQLToken.String)
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

            var blogPost = new GraphQLObjectTypeDefinition
            {
                Name = new GraphQLName("BlogPost"),
                Fields = new GraphQLFieldsDefinition
                {
                    Items = new List<GraphQLFieldDefinition>
                    {
                        new()
                        {
                            Name = new GraphQLName("id"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName(GraphQLToken.String)
                            }
                        },
                        new()
                        {
                            Name = new GraphQLName("author"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName("Blogger")
                            }
                        }
                    }
                }
            };

            var blogger = new GraphQLObjectTypeDefinition
            {
                Name = new GraphQLName("Blogger"),
                Fields = new GraphQLFieldsDefinition
                {
                    Items = new List<GraphQLFieldDefinition>
                    {
                        new()
                        {
                            Name = new GraphQLName("id"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName(GraphQLToken.String)
                            }
                        },
                        new()
                        {
                            Name = new GraphQLName("name"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName(GraphQLToken.String)
                            }
                        },
                        new()
                        {
                            Name = new GraphQLName("posts"),
                            Type = new GraphQLListType
                            {
                                Type = new GraphQLNamedType
                                {
                                    Name = new GraphQLName(blogPost.Name)
                                }
                            },
                            Arguments = new GraphQLArgumentsDefinition
                            {
                                Items = new List<GraphQLInputValueDefinition>
                                {
                                    new()
                                    {
                                        Name = new GraphQLName("filter"),
                                        Type = new GraphQLNamedType
                                        {
                                            Name = new GraphQLName(GraphQLToken.String)
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var bloggerWhoComments = new GraphQLObjectTypeDefinition
            {
                Name = new GraphQLName("BloggerWhoComments"),
                Fields = new GraphQLFieldsDefinition
                {
                    Items = new List<GraphQLFieldDefinition>
                    {
                        new()
                        {
                            Name = new GraphQLName("id"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName(GraphQLToken.String)
                            }
                        },
                        new()
                        {
                            Name = new GraphQLName("name"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName(GraphQLToken.String)
                            }
                        },
                        new()
                        {
                            Name = new GraphQLName("posts"),
                            Type = new GraphQLListType
                            {
                                Type = new GraphQLNamedType
                                {
                                    Name = new GraphQLName("BlogPostWithComments")
                                }
                            }
                        }
                    }
                }
            };

            var blogComment = new GraphQLObjectTypeDefinition
            {
                Name = new GraphQLName("BlogComment"),
                Fields = new GraphQLFieldsDefinition
                {
                    Items = new List<GraphQLFieldDefinition>
                    {
                        new()
                        {
                            Name = new GraphQLName("id"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName(GraphQLToken.String)
                            }
                        },
                        new()
                        {
                            Name = new GraphQLName("content"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName(GraphQLToken.String)
                            }
                        },
                        new()
                        {
                            Name = new GraphQLName("author"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName(bloggerWhoComments.Name)
                            }
                        }
                    }
                }
            };

            var blogPostWithComments = new GraphQLObjectTypeDefinition
            {
                Name = new GraphQLName("BlogPostWithComments"),
                Fields = new GraphQLFieldsDefinition
                {
                    Items = new List<GraphQLFieldDefinition>
                    {
                        new()
                        {
                            Name = new GraphQLName("id"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName(GraphQLToken.String)
                            }
                        },
                        new()
                        {
                            Name = new GraphQLName("content"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName(GraphQLToken.String)
                            }
                        },
                        new()
                        {
                            Name = new GraphQLName("comments"),
                            Type = new GraphQLListType
                            {
                                Type = new GraphQLNamedType
                                {
                                    Name = new GraphQLName(blogComment.Name)
                                }
                            }
                        }
                    }
                }
            };

            var bestFriend = new GraphQLObjectTypeDefinition
            {
                Name = new GraphQLName("BestFriend"),
                Fields = new GraphQLFieldsDefinition
                {
                    Items = new List<GraphQLFieldDefinition>
                    {
                        new()
                        {
                            Name = new GraphQLName("id"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName(GraphQLToken.String)
                            }
                        },
                        new()
                        {
                            Name = new GraphQLName("name"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName(GraphQLToken.String)
                            }
                        },
                        new()
                        {
                            Name = new GraphQLName("bestFriend"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName("BestFriend")
                            }
                        }
                    }
                }
            };

            var blogPostWithArguments = new GraphQLObjectTypeDefinition
            {
                Name = new GraphQLName("BlogPostWithArguments"),
                Fields = new GraphQLFieldsDefinition
                {
                    Items = new List<GraphQLFieldDefinition>
                    {
                        new()
                        {
                            Name = new GraphQLName("id"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName(GraphQLToken.String)
                            }
                        },
                        new()
                        {
                            Name = new GraphQLName("content"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName(GraphQLToken.String)
                            },
                            Arguments = new GraphQLArgumentsDefinition
                            {
                                Items = new List<GraphQLInputValueDefinition>
                                {
                                    new()
                                    {
                                        Name = new GraphQLName("filter"),
                                        Type = new GraphQLNamedType
                                        {
                                            Name = new GraphQLName(GraphQLToken.String)
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var blogUnion = new GraphQLUnionTypeDefinition
            {
                Name = new GraphQLName("BlogUnion"),
                Types = new GraphQLUnionMemberTypes
                {
                    Items = new List<GraphQLNamedType>
                    {
                        new()
                        {
                            Name = new GraphQLName(blogPost.Name)
                        },
                        new()
                        {
                            Name = new GraphQLName(blogPostWithComments.Name)
                        }, 
                        new()
                        {
                            Name = new GraphQLName(blogPostWithArguments.Name)
                        }
                    }
                }
            };

            var bloggerWithUnionPosts = new GraphQLObjectTypeDefinition
            {
                Name = new GraphQLName("BloggerWithUnionPosts"),
                Fields = new GraphQLFieldsDefinition
                {
                    Items = new List<GraphQLFieldDefinition>
                    {
                        new()
                        {
                            Name = new GraphQLName("id"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName(GraphQLToken.String)
                            }
                        },
                        new()
                        {
                            Name = new GraphQLName("name"),
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName(GraphQLToken.String)
                            }
                        },
                        new()
                        {
                            Name = new GraphQLName("posts"),
                            Type = new GraphQLListType
                            {
                                Type = new GraphQLNamedType
                                {
                                    Name = new GraphQLName(blogUnion.Name)
                                }
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
                    personInterfaceWithFriend,
                    blogPost,
                    blogger,
                    blogPostWithComments,
                    bloggerWhoComments,
                    blogComment,
                    bestFriend,
                    blogPostWithArguments,
                    blogUnion,
                    bloggerWithUnionPosts
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
                                """,
                    Arguments = new List<GraphQLArgumentTypeBase>()
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
                                """,
                    Arguments = new List<GraphQLArgumentTypeBase>()
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
                                """,
                    Arguments = new List<GraphQLArgumentTypeBase>
                    {
                        new GraphQLArgumentType("id", "id", GraphQLToken.Int)
                    }
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
                                """,
                    Arguments = new List<GraphQLArgumentTypeBase>
                    {
                        new GraphQLArgumentType("id", "id", GraphQLToken.Int),
                        new GraphQLListArgumentType(new GraphQLArgumentType("ids", "ids", GraphQLToken.String)),
                        new GraphQLNonNullArgumentType(new GraphQLArgumentType("location", "location", GraphQLToken.String))
                    }
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
                                """,
                    Arguments = new List<GraphQLArgumentTypeBase>
                    {
                        new GraphQLListArgumentType(
                            new GraphQLArgumentType("filter", "filter", GraphQLToken.String)
                        )
                    }
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
                                """,
                    Arguments = new List<GraphQLArgumentTypeBase>()
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
                                """,
                    Arguments = new List<GraphQLArgumentTypeBase>()
                }
            ).SetName("Converter is able to convert simple query with nested interface return type.");

            var blogPostQueryFieldDefinition = new GraphQLFieldDefinition
            {
                Name = new GraphQLName("blogPost"),
                Type = new GraphQLNamedType
                {
                    Name = new GraphQLName(blogPost.Name)
                }
            };

            yield return new TestCaseData(
                blogPostQueryFieldDefinition,
                graphQLDocumentAdapter,
                new GraphQLQueryFieldType(blogPostQueryFieldDefinition)
                {
                    QueryString = """
                                query BlogPostTest {
                                  blogPost {
                                    id
                                    author {
                                      id
                                      name
                                    }
                                  }
                                }
                                """,
                    Arguments = new List<GraphQLArgumentTypeBase>()
                }
            ).SetName("Converter is able to convert query with cyclical types.");

            var blogPostWithCommentsQueryFieldDefinition = new GraphQLFieldDefinition
            {
                Name = new GraphQLName("blogPostWithComments"),
                Type = new GraphQLNamedType
                {
                    Name = new GraphQLName(blogPostWithComments.Name)
                }
            };

            yield return new TestCaseData(
                blogPostWithCommentsQueryFieldDefinition,
                graphQLDocumentAdapter,
                new GraphQLQueryFieldType(blogPostWithCommentsQueryFieldDefinition)
                {
                    QueryString = """
                                query BlogPostWithCommentsTest {
                                  blogPostWithComments {
                                    id
                                    content
                                    comments {
                                      id
                                      content
                                      author {
                                        id
                                        name
                                      }
                                    }
                                  }
                                }
                                """,
                    Arguments = new List<GraphQLArgumentTypeBase>()
                }
            ).SetName("Converter is able to convert query with non-direct cyclical types.");

            var bestFriendQueryFieldDefinition = new GraphQLFieldDefinition
            {
                Name = new GraphQLName("bestFriend"),
                Type = new GraphQLNamedType
                {
                    Name = new GraphQLName(bestFriend.Name)
                }
            };

            yield return new TestCaseData(
                bestFriendQueryFieldDefinition,
                graphQLDocumentAdapter,
                new GraphQLQueryFieldType(bestFriendQueryFieldDefinition)
                {
                    QueryString = """
                                query BestFriendTest {
                                  bestFriend {
                                    id
                                    name
                                  }
                                }
                                """,
                    Arguments = new List<GraphQLArgumentTypeBase>()
                }
            ).SetName("Converter is able to convert query with immediate cyclical types.");

            var blogUnionQueryFieldDefinition = new GraphQLFieldDefinition
            {
                Name = new GraphQLName("blogUnion"),
                Type = new GraphQLNamedType
                {
                    Name = new GraphQLName(blogUnion.Name)
                }
            };

            yield return new TestCaseData(
                blogUnionQueryFieldDefinition,
                graphQLDocumentAdapter,
                new GraphQLQueryFieldType(blogUnionQueryFieldDefinition)
                {
                    QueryString = """
                                query BlogUnionTest($filter: String, $filter1: String) {
                                  blogUnion {
                                    ... on BlogPost {
                                      id
                                      author {
                                        id
                                        name
                                        posts(filter: $filter) {
                                          id
                                        }
                                      }
                                    }
                                    ... on BlogPostWithComments {
                                      id
                                      content
                                      comments {
                                        id
                                        content
                                        author {
                                          id
                                          name
                                          posts {
                                            id
                                            content
                                          }
                                        }
                                      }
                                    }
                                    ... on BlogPostWithArguments {
                                      id
                                      content(filter: $filter1)
                                    }
                                  }
                                }
                                """,
                    Arguments = new List<GraphQLArgumentTypeBase>
                    {
                        new GraphQLArgumentType("filter", "filter", GraphQLToken.String),
                        new GraphQLArgumentType("filter", "filter1", GraphQLToken.String),
                    }
                }
            ).SetName("Converter is able to convert query with arguments and union return type.");

            var bloggerWithUnionPostsQueryFieldDefinition = new GraphQLFieldDefinition
            {
                Name = new GraphQLName("bloggerWithUnionPosts"),
                Type = new GraphQLNamedType
                {
                    Name = new GraphQLName(bloggerWithUnionPosts.Name)
                }
            };

            yield return new TestCaseData(
                bloggerWithUnionPostsQueryFieldDefinition,
                graphQLDocumentAdapter,
                new GraphQLQueryFieldType(bloggerWithUnionPostsQueryFieldDefinition)
                {
                    QueryString = """
                                query BloggerWithUnionPostsTest($filter: String, $filter1: String) {
                                  bloggerWithUnionPosts {
                                    id
                                    name
                                    posts {
                                      ... on BlogPost {
                                        id
                                        author {
                                          id
                                          name
                                          posts(filter: $filter) {
                                            id
                                          }
                                        }
                                      }
                                      ... on BlogPostWithComments {
                                        id
                                        content
                                        comments {
                                          id
                                          content
                                          author {
                                            id
                                            name
                                            posts {
                                              id
                                              content
                                            }
                                          }
                                        }
                                      }
                                      ... on BlogPostWithArguments {
                                        id
                                        content(filter: $filter1)
                                      }
                                    }
                                  }
                                }
                                """,
                    Arguments = new List<GraphQLArgumentTypeBase>
                    {
                        new GraphQLArgumentType("filter", "filter", GraphQLToken.String),
                        new GraphQLArgumentType("filter", "filter1", GraphQLToken.String),
                    }
                }
            ).SetName("Converter is able to convert query with arguments and union return type.");
        }
    }
}