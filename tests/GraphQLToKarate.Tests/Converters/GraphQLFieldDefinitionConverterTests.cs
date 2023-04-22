using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Enums;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;
using NSubstitute;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Converters;

[TestFixture]
internal sealed class GraphQLFieldDefinitionConverterTests
{
    private const string ExampleValue = "an example value";
    private IGraphQLInputValueDefinitionConverterFactory? _mockGraphQLInputValueDefinitionConverterFactory;
    private IGraphQLInputValueToExampleValueConverter? _mockGraphQLInputValueToExampleValueConverter;
    private IGraphQLFieldDefinitionConverter? _subjectUnderTest;

    [SetUp]
    public void SetUp()
    {
        _mockGraphQLInputValueToExampleValueConverter = Substitute.For<IGraphQLInputValueToExampleValueConverter>();
        _mockGraphQLInputValueDefinitionConverterFactory =
            Substitute.For<IGraphQLInputValueDefinitionConverterFactory>();

        _mockGraphQLInputValueToExampleValueConverter
            .Convert(Arg.Any<GraphQLInputValueDefinition>(), Arg.Any<IGraphQLDocumentAdapter>())
            .Returns(ExampleValue);

        _mockGraphQLInputValueDefinitionConverterFactory
            .Create()
            .Returns(new GraphQLInputValueDefinitionConverter(_mockGraphQLInputValueToExampleValueConverter));

        _subjectUnderTest = new GraphQLFieldDefinitionConverter(_mockGraphQLInputValueDefinitionConverterFactory);
    }

    [Test]
    [TestCaseSource(nameof(TestCases))]
    public void Convert(
        GraphQLFieldDefinition graphQLQueryFieldDefinition,
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        GraphQLOperationType graphQLOperationType,
        GraphQLOperation expectedGraphQLOperation)
    {
        // act
        var graphQLQueryFieldType = _subjectUnderTest!.Convert(
            graphQLQueryFieldDefinition,
            graphQLDocumentAdapter,
            graphQLOperationType
        );

        // assert
        graphQLQueryFieldType
            .Should()
            .BeEquivalentTo(expectedGraphQLOperation);
    }

    private static IEnumerable<TestCaseData> TestCases
    {
        get
        {
            var person = new GraphQLObjectTypeDefinition(new GraphQLName("Person"))
            {
                Fields = new GraphQLFieldsDefinition(
                    new List<GraphQLFieldDefinition>
                    {
                        new(
                            new GraphQLName("id"),
                            new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                        ),
                        new(
                            new GraphQLName("name"),
                            new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                        ),
                        new(
                            new GraphQLName("favoriteNumber"),
                            new GraphQLNamedType(new GraphQLName("Integer"))
                        ),
                        new(
                            new GraphQLName("favoriteColor"),
                            new GraphQLNamedType(new GraphQLName("Color"))
                        )
                    }
                )
            };

            var personWithFriends = new GraphQLObjectTypeDefinition(new GraphQLName("PersonWithFriends"))
            {
                Fields = new GraphQLFieldsDefinition(
                    new List<GraphQLFieldDefinition>
                    {
                        new(
                            new GraphQLName("id"),
                            new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                        ),
                        new(
                            new GraphQLName("name"),
                            new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                        ),
                        new(
                            new GraphQLName("friends"),
                            new GraphQLListType(new GraphQLNamedType(new GraphQLName(person.Name)))
                        )
                    }
                )
            };

            var personWithFriendsWithArguments = new GraphQLObjectTypeDefinition(
                new GraphQLName("PersonWithFriendsWithArguments"))
            {
                Fields = new GraphQLFieldsDefinition(
                    new List<GraphQLFieldDefinition>
                    {
                        new(
                            new GraphQLName("id"),
                            new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                        ),
                        new(
                            new GraphQLName("name"),
                            new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                        ),
                        new(
                            new GraphQLName("friends"),
                            new GraphQLListType(new GraphQLNamedType(new GraphQLName(person.Name))))
                        {
                            Arguments = new GraphQLArgumentsDefinition(
                                new List<GraphQLInputValueDefinition>
                                {
                                    new(
                                        new GraphQLName("ids"),
                                        new GraphQLListType(new GraphQLNamedType(new GraphQLName(GraphQLToken.String)))
                                    ),
                                    new(
                                        new GraphQLName("location"),
                                        new GraphQLNonNullType(
                                            new GraphQLNamedType(new GraphQLName(GraphQLToken.String)))
                                    )
                                }
                            )
                        }
                    }
                )
            };

            var personWithFavoriteColorsWithArguments = new GraphQLObjectTypeDefinition(
                new GraphQLName("PersonWithFavoriteColors"))
            {
                Fields = new GraphQLFieldsDefinition(
                    new List<GraphQLFieldDefinition>
                    {
                        new(
                            new GraphQLName("id"),
                            new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                        ),
                        new(
                            new GraphQLName("name"),
                            new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                        ),
                        new(
                            new GraphQLName("favoriteNumber"),
                            new GraphQLNamedType(new GraphQLName("Integer"))
                        ),
                        new(new GraphQLName("favoriteColors"),
                            new GraphQLListType(new GraphQLNamedType(new GraphQLName("Color"))))
                        {
                            Arguments = new GraphQLArgumentsDefinition(
                                new List<GraphQLInputValueDefinition>
                                {
                                    new(
                                        new GraphQLName("filter"),
                                        new GraphQLListType(new GraphQLNamedType(new GraphQLName(GraphQLToken.String)))
                                    )
                                }
                            )
                        }
                    }
                )
            };

            var personInterface = new GraphQLInterfaceTypeDefinition(new GraphQLName("PersonInterface"))
            {
                Fields = new GraphQLFieldsDefinition(
                    new List<GraphQLFieldDefinition>
                    {
                        new(
                            new GraphQLName("id"),
                            new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                        ),
                        new(
                            new GraphQLName("name"),
                            new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                        )
                    }
                )
            };

            var personInterfaceWithFriend = new GraphQLInterfaceTypeDefinition(new GraphQLName("NestedPersonInterface"))
            {
                Fields = new GraphQLFieldsDefinition(
                    new List<GraphQLFieldDefinition>
                    {
                        new(
                            new GraphQLName("id"),
                            new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                        ),
                        new(
                            new GraphQLName("name"),
                            new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                        ),
                        new(
                            new GraphQLName("friend"),
                            new GraphQLNamedType(new GraphQLName(personInterface.Name))
                        )
                    }
                )
            };

            var colors = new GraphQLEnumTypeDefinition(new GraphQLName("Color"))
            {
                Values = new GraphQLEnumValuesDefinition(
                    new List<GraphQLEnumValueDefinition>
                    {
                        new(new GraphQLName("RED"), new GraphQLEnumValue(new GraphQLName("RED"))),
                        new(new GraphQLName("BLUE"), new GraphQLEnumValue(new GraphQLName("BLUE"))),
                        new(new GraphQLName("GREEN"), new GraphQLEnumValue(new GraphQLName("GREEN")))
                    }
                )
            };

            var blogPost = new GraphQLObjectTypeDefinition(new GraphQLName("BlogPost"))
            {
                Fields = new GraphQLFieldsDefinition(
                    new List<GraphQLFieldDefinition>
                    {
                        new(
                            new GraphQLName("id"),
                            new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                        ),
                        new(
                            new GraphQLName("author"),
                            new GraphQLNamedType(new GraphQLName("Blogger"))
                        )
                    }
                )
            };

            var blogger = new GraphQLObjectTypeDefinition(new GraphQLName("Blogger"))
            {
                Fields = new GraphQLFieldsDefinition(
                    new List<GraphQLFieldDefinition>
                    {
                        new(
                            new GraphQLName("id"),
                            new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                        ),
                        new(
                            new GraphQLName("name"),
                            new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                        ),
                        new(
                            new GraphQLName("posts"),
                            new GraphQLListType(new GraphQLNamedType(new GraphQLName(blogPost.Name))))
                        {
                            Arguments = new GraphQLArgumentsDefinition(
                                new List<GraphQLInputValueDefinition>
                                {
                                    new(
                                        new GraphQLName("filter"),
                                        new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                                    )
                                }
                            )
                        }
                    }
                )
            };

            var bloggerWhoComments = new GraphQLObjectTypeDefinition(new GraphQLName("BloggerWhoComments"))
            {
                Fields = new GraphQLFieldsDefinition(
                    new List<GraphQLFieldDefinition>
                    {
                        new(
                            new GraphQLName("id"),
                            new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                        ),
                        new(
                            new GraphQLName("name"),
                            new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                        ),
                        new(
                            new GraphQLName("posts"),
                            new GraphQLListType(new GraphQLNamedType(new GraphQLName("BlogPostWithComments")))
                        )
                    }
                )
            };

            var blogComment = new GraphQLObjectTypeDefinition(new GraphQLName("BlogComment"))
            {
                Fields = new GraphQLFieldsDefinition(
                    new List<GraphQLFieldDefinition>
                    {
                        new(new GraphQLName("id"), new GraphQLNamedType(new GraphQLName(GraphQLToken.String))),
                        new(
                            new GraphQLName("content"),
                            new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                        ),
                        new(
                            new GraphQLName("author"),
                            new GraphQLNamedType(new GraphQLName(bloggerWhoComments.Name))
                        )
                    }
                )
            };

            var blogPostWithComments = new GraphQLObjectTypeDefinition(new GraphQLName("BlogPostWithComments"))
            {
                Fields = new GraphQLFieldsDefinition(
                    new List<GraphQLFieldDefinition>
                    {
                        new(
                            new GraphQLName("id"),
                            new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                        ),
                        new(
                            new GraphQLName("content"),
                            new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                        ),
                        new(
                            new GraphQLName("comments"),
                            new GraphQLListType(new GraphQLNamedType(new GraphQLName(blogComment.Name)))
                        )
                    }
                )
            };

            var bestFriend = new GraphQLObjectTypeDefinition(new GraphQLName("BestFriend"))
            {
                Fields = new GraphQLFieldsDefinition(
                    new List<GraphQLFieldDefinition>
                    {
                        new(
                            new GraphQLName("id"),
                            new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                        ),
                        new(
                            new GraphQLName("name"),
                            new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                        ),
                        new(
                            new GraphQLName("bestFriend"),
                            new GraphQLNamedType(new GraphQLName("BestFriend"))
                        )
                    }
                )
            };

            var blogPostWithArguments = new GraphQLObjectTypeDefinition(
                new GraphQLName("BlogPostWithArguments"))
            {
                Fields = new GraphQLFieldsDefinition(
                    new List<GraphQLFieldDefinition>
                    {
                        new(
                            new GraphQLName("id"),
                            new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                        ),
                        new(
                            new GraphQLName("content"),
                            new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                        )
                        {
                            Arguments = new GraphQLArgumentsDefinition(
                                new List<GraphQLInputValueDefinition>
                                {
                                    new(
                                        new GraphQLName("filter"),
                                        new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                                    )
                                }
                            )
                        }
                    }
                )
            };

            var blogUnion = new GraphQLUnionTypeDefinition(new GraphQLName("BlogUnion"))
            {
                Types = new GraphQLUnionMemberTypes(
                    new List<GraphQLNamedType>
                    {
                        new(new GraphQLName(blogPost.Name)),
                        new(new GraphQLName(blogPostWithComments.Name)),
                        new(new GraphQLName(blogPostWithArguments.Name))
                    }
                )
            };

            var bloggerWithUnionPosts = new GraphQLObjectTypeDefinition(new GraphQLName("BloggerWithUnionPosts"))
            {
                Fields = new GraphQLFieldsDefinition(
                    new List<GraphQLFieldDefinition>
                    {
                        new(
                            new GraphQLName("id"),
                            new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                        ),
                        new(
                            new GraphQLName("name"),
                            new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                        ),
                        new(
                            new GraphQLName("posts"),
                            new GraphQLListType(new GraphQLNamedType(new GraphQLName(blogUnion.Name)))
                        )
                    }
                )
            };

            var graphQLDocument = new GraphQLDocument(
                new List<ASTNode>
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
            );

            var graphQLDocumentAdapter = new GraphQLDocumentAdapter(graphQLDocument);

            var personQueryFieldDefinition = new GraphQLFieldDefinition(
                new GraphQLName("person"),
                new GraphQLNamedType(new GraphQLName(person.Name))
            );

            yield return new TestCaseData(
                personQueryFieldDefinition,
                graphQLDocumentAdapter,
                GraphQLOperationType.Query,
                new GraphQLOperation(personQueryFieldDefinition)
                {
                    Type = GraphQLOperationType.Query,
                    OperationString = """
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

            var personWithFriendsQueryFieldDefinition = new GraphQLFieldDefinition(
                new GraphQLName("personWithFriends"),
                new GraphQLNamedType(new GraphQLName(personWithFriends.Name))
            );

            yield return new TestCaseData(
                personWithFriendsQueryFieldDefinition,
                graphQLDocumentAdapter,
                GraphQLOperationType.Query,
                new GraphQLOperation(personWithFriendsQueryFieldDefinition)
                {
                    Type = GraphQLOperationType.Query,
                    OperationString = """
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

            var personWithFriendsWithArgumentsGraphQLQueryFieldDefinition = new GraphQLFieldDefinition(
                new GraphQLName("personById"),
                new GraphQLNamedType(new GraphQLName(person.Name))
            )
            {
                Arguments = new GraphQLArgumentsDefinition(
                    new List<GraphQLInputValueDefinition>
                    {
                        new(
                            new GraphQLName("id"),
                            new GraphQLNamedType(new GraphQLName("Int"))
                        )
                    }
                )
            };

            yield return new TestCaseData(
                personWithFriendsWithArgumentsGraphQLQueryFieldDefinition,
                graphQLDocumentAdapter,
                GraphQLOperationType.Query,
                new GraphQLOperation(personWithFriendsWithArgumentsGraphQLQueryFieldDefinition)
                {
                    Type = GraphQLOperationType.Query,
                    OperationString = """
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
                        new GraphQLArgumentType("id", "id", GraphQLToken.Int, ExampleValue)
                    }
                }
            ).SetName("Converter is able to convert simple query with arguments.");

            var personWithFriendsWithNestedArgumentsGraphQLQueryFieldDefinition = new GraphQLFieldDefinition(
                new GraphQLName("personWithFriendsById"),
                new GraphQLNamedType(new GraphQLName("PersonWithFriendsWithArguments"))
            )
            {
                Arguments = new GraphQLArgumentsDefinition(
                    new List<GraphQLInputValueDefinition>
                    {
                        new(
                            new GraphQLName("id"),
                            new GraphQLNamedType(new GraphQLName("Int"))
                        )
                    }
                )
            };

            var personWithFavoriteColorsGraphQLQueryFieldDefinition = new GraphQLFieldDefinition(
                new GraphQLName("personWithFavoriteColors"),
                new GraphQLNamedType(new GraphQLName(personWithFavoriteColorsWithArguments.Name))
            );

            yield return new TestCaseData(
                personWithFriendsWithNestedArgumentsGraphQLQueryFieldDefinition,
                graphQLDocumentAdapter,
                GraphQLOperationType.Query,
                new GraphQLOperation(personWithFriendsWithNestedArgumentsGraphQLQueryFieldDefinition)
                {
                    Type = GraphQLOperationType.Query,
                    OperationString = """
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
                        new GraphQLArgumentType(
                            "id",
                            "id",
                            GraphQLToken.Int,
                            ExampleValue
                        ),
                        new GraphQLListArgumentType(
                            new GraphQLArgumentType(
                                "ids",
                                "ids",
                                GraphQLToken.String,
                                ExampleValue
                            )
                        ),
                        new GraphQLNonNullArgumentType(
                            new GraphQLArgumentType(
                                "location",
                                "location",
                                GraphQLToken.String,
                                ExampleValue
                            )
                        )
                    }
                }
            ).SetName("Converter is able to convert nested query with nested field arguments.");

            yield return new TestCaseData(
                personWithFavoriteColorsGraphQLQueryFieldDefinition,
                graphQLDocumentAdapter,
                GraphQLOperationType.Query,
                new GraphQLOperation(personWithFavoriteColorsGraphQLQueryFieldDefinition)
                {
                    Type = GraphQLOperationType.Query,
                    OperationString = """
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
                            new GraphQLArgumentType("filter", "filter", GraphQLToken.String, ExampleValue)
                        )
                    }
                }
            ).SetName("Converter is able to convert simple query with scalar field arguments.");

            var personInterfaceQueryGraphQLFieldDefinition = new GraphQLFieldDefinition(
                new GraphQLName("personInterface"),
                new GraphQLNamedType(new GraphQLName(personInterface.Name))
            );

            yield return new TestCaseData(
                personInterfaceQueryGraphQLFieldDefinition,
                graphQLDocumentAdapter,
                GraphQLOperationType.Query,
                new GraphQLOperation(personInterfaceQueryGraphQLFieldDefinition)
                {
                    Type = GraphQLOperationType.Query,
                    OperationString = """
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

            var nestedPersonInterfaceGraphQLQueryFieldDefinition = new GraphQLFieldDefinition(
                new GraphQLName("nestedPersonInterface"),
                new GraphQLNamedType(new GraphQLName(personInterfaceWithFriend.Name))
            );

            yield return new TestCaseData(
                nestedPersonInterfaceGraphQLQueryFieldDefinition,
                graphQLDocumentAdapter,
                GraphQLOperationType.Query,
                new GraphQLOperation(nestedPersonInterfaceGraphQLQueryFieldDefinition)
                {
                    Type = GraphQLOperationType.Query,
                    OperationString = """
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

            var blogPostQueryFieldDefinition = new GraphQLFieldDefinition(
                new GraphQLName("blogPost"),
                new GraphQLNamedType(new GraphQLName(blogPost.Name))
            );

            yield return new TestCaseData(
                blogPostQueryFieldDefinition,
                graphQLDocumentAdapter,
                GraphQLOperationType.Query,
                new GraphQLOperation(blogPostQueryFieldDefinition)
                {
                    Type = GraphQLOperationType.Query,
                    OperationString = """
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

            var blogPostWithCommentsQueryFieldDefinition = new GraphQLFieldDefinition(
                new GraphQLName("blogPostWithComments"),
                new GraphQLNamedType(new GraphQLName(blogPostWithComments.Name))
            );

            yield return new TestCaseData(
                blogPostWithCommentsQueryFieldDefinition,
                graphQLDocumentAdapter,
                GraphQLOperationType.Query,
                new GraphQLOperation(blogPostWithCommentsQueryFieldDefinition)
                {
                    Type = GraphQLOperationType.Query,
                    OperationString = """
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

            var bestFriendQueryFieldDefinition = new GraphQLFieldDefinition(
                new GraphQLName("bestFriend"),
                new GraphQLNamedType(new GraphQLName(bestFriend.Name))
            );

            yield return new TestCaseData(
                bestFriendQueryFieldDefinition,
                graphQLDocumentAdapter,
                GraphQLOperationType.Query,
                new GraphQLOperation(bestFriendQueryFieldDefinition)
                {
                    Type = GraphQLOperationType.Query,
                    OperationString = """
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

            var blogUnionQueryFieldDefinition = new GraphQLFieldDefinition(
                new GraphQLName("blogUnion"),
                new GraphQLNamedType(new GraphQLName(blogUnion.Name))
            );

            yield return new TestCaseData(
                blogUnionQueryFieldDefinition,
                graphQLDocumentAdapter,
                GraphQLOperationType.Query,
                new GraphQLOperation(blogUnionQueryFieldDefinition)
                {
                    Type = GraphQLOperationType.Query,
                    OperationString = """
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
                        new GraphQLArgumentType("filter", "filter", GraphQLToken.String, ExampleValue),
                        new GraphQLArgumentType("filter", "filter1", GraphQLToken.String, ExampleValue)
                    }
                }
            ).SetName("Converter is able to convert query with arguments and union return type.");

            var bloggerWithUnionPostsQueryFieldDefinition = new GraphQLFieldDefinition(
                new GraphQLName("bloggerWithUnionPosts"),
                new GraphQLNamedType(new GraphQLName(bloggerWithUnionPosts.Name))
            );

            yield return new TestCaseData(
                bloggerWithUnionPostsQueryFieldDefinition,
                graphQLDocumentAdapter,
                GraphQLOperationType.Query,
                new GraphQLOperation(bloggerWithUnionPostsQueryFieldDefinition)
                {
                    Type = GraphQLOperationType.Query,
                    OperationString = """
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
                        new GraphQLArgumentType("filter", "filter", GraphQLToken.String, ExampleValue),
                        new GraphQLArgumentType("filter", "filter1", GraphQLToken.String, ExampleValue)
                    }
                }
            ).SetName("Converter is able to convert query with arguments and nested union fields.");
        }
    }
}