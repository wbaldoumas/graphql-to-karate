using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Features;
using GraphQLToKarate.Library.Types;
using NSubstitute;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Features;

[TestFixture]
internal sealed class KarateFeatureBuilderTests
{
    private IKarateScenarioBuilder? _mockScenarioBuilder;

    private IKarateFeatureBuilder? _subjectUnderTest;

    [SetUp]
    public void SetUp()
    {
        _mockScenarioBuilder = Substitute.For<IKarateScenarioBuilder>();

        _subjectUnderTest = new KarateFeatureBuilder(_mockScenarioBuilder);
    }

    [Test]
    [TestCaseSource(nameof(TestCases))]
    public void FeatureBuilder_builds_expected_feature(
        IEnumerable<KarateObject> karateObjects,
        ICollection<GraphQLQueryFieldType> graphQLQueryFieldTypes,
        IDictionary<string, string> mockScenarioBuilderReturnByGraphQLQueryName,
        string expectedFeatureString)
    {
        // arrange
        foreach (var graphQLQueryFieldType in graphQLQueryFieldTypes)
        {
            _mockScenarioBuilder!
                .Build(graphQLQueryFieldType)
                .Returns(mockScenarioBuilderReturnByGraphQLQueryName[graphQLQueryFieldType.Name]);
        }

        // act
        var featureString = _subjectUnderTest!.Build(karateObjects, graphQLQueryFieldTypes);

        // assert
        featureString.Should().Be(expectedFeatureString);
    }

    private static IEnumerable<TestCaseData> TestCases
    {
        get
        {
            var graphQLFieldDefinition = new GraphQLFieldDefinition
            {
                Name = new GraphQLName("todo"),
                Type = new GraphQLNamedType
                {
                    Name = new GraphQLName("Todo")
                }
            };

            yield return new TestCaseData(
                new List<KarateObject>
                {
                    new(
                        "Todo",
                        new List<KarateTypeBase>
                        {
                            new KarateNonNullType(new KarateType("number", "id")),
                            new KarateNonNullType(new KarateType("string", "name"))
                        }
                    )
                },
                new List<GraphQLQueryFieldType>
                {
                    new(graphQLFieldDefinition)
                    {
                        Arguments = new List<GraphQLArgumentTypeBase>(),
                        QueryString =
                        """
                        query TodoTest {
                          todo {
                            id
                            name
                          }
                        }
                        """
                    }
                },
                new Dictionary<string, string>
                {
                    {
                        graphQLFieldDefinition.Name.StringValue,
                        """"
                        Scenario: Perform a todo query and validate the response
                          * text query = """
                              query TodoTest {
                                todo {
                                  id
                                  name
                                }
                              }
                            """

                          Given path "/graphql"
                          And request { query: query, operationName: "TodoTest" }
                          When method post
                          Then status 200
                          And match response.data.todo == todoSchema
                        """"
                    }
                },
                """"
                Feature: Test GraphQL Endpoint with Karate

                Background: Base URL and Schemas
                  * url baseUrl

                  * text todoSchema = """
                      {
                        id: '#number',
                        name: '#string'
                      }
                    """

                Scenario: Perform a todo query and validate the response
                  * text query = """
                      query TodoTest {
                        todo {
                          id
                          name
                        }
                      }
                    """
                
                  Given path "/graphql"
                  And request { query: query, operationName: "TodoTest" }
                  When method post
                  Then status 200
                  And match response.data.todo == todoSchema
                """"
            ).SetName("Simple case with one Karate object schema and one query is handled as expected.");

            var otherGraphQLFieldDefinition = new GraphQLFieldDefinition()
            {
                Name = new GraphQLName("user"),
                Type = new GraphQLNamedType
                {
                    Name = new GraphQLName("User")
                }
            };

            yield return new TestCaseData(
                new List<KarateObject>
                {
                    new(
                        "Todo",
                        new List<KarateTypeBase>
                        {
                            new KarateNonNullType(new KarateType("number", "id")),
                            new KarateNonNullType(new KarateType("string", "name"))
                        }
                    ),
                    new(
                        "User",
                        new List<KarateTypeBase>
                        {
                            new KarateNonNullType(new KarateType("number", "id")),
                            new KarateNonNullType(new KarateType("string", "name")),
                            new KarateNonNullType(
                                new KarateListType(
                                    new KarateNonNullType(
                                        new KarateType("todoSchema", "todos")
                                    )
                                )
                            )
                        }
                    )
                },
                new List<GraphQLQueryFieldType>
                {
                    new(graphQLFieldDefinition)
                    {
                        Arguments = new List<GraphQLArgumentTypeBase>(),
                        QueryString =
                        """
                        query TodoTest {
                          todo {
                            id
                            name
                          }
                        }
                        """
                    },
                    new(otherGraphQLFieldDefinition)
                    {
                        Arguments = new List<GraphQLArgumentTypeBase>(),
                        QueryString =
                        """
                        query UserTest {
                          user {
                            id
                            name
                            todos {
                              id
                              name
                            }
                          }
                        }
                        """
                    }
                },
                new Dictionary<string, string>
                {
                    {
                        graphQLFieldDefinition.Name.StringValue,
                        """"
                        Scenario: Perform a todo query and validate the response
                          * text query = """
                              query TodoTest {
                                todo {
                                  id
                                  name
                                }
                              }
                            """

                          Given path "/graphql"
                          And request { query: query, operationName: "TodoTest" }
                          When method post
                          Then status 200
                          And match response.data.todo == todoSchema
                        """"
                    },
                    {
                        otherGraphQLFieldDefinition.Name.StringValue,
                        """"
                        Scenario: Perform a user query and validate the response
                          * text query = """
                              query UserTest {
                                user {
                                  id
                                  name
                                  todos {
                                    id
                                    name
                                  }
                                }
                              }
                            """

                          Given path "/graphql"
                          And request { query: query, operationName: "UserTest" }
                          When method post
                          Then status 200
                          And match response.data.user == userSchema
                        """"
                    }
                },
                """"
                Feature: Test GraphQL Endpoint with Karate

                Background: Base URL and Schemas
                  * url baseUrl

                  * text todoSchema = """
                      {
                        id: '#number',
                        name: '#string'
                      }
                    """

                  * text userSchema = """
                      {
                        id: '#number',
                        name: '#string',
                        todos: '#[] #todoSchema'
                      }
                    """

                Scenario: Perform a todo query and validate the response
                  * text query = """
                      query TodoTest {
                        todo {
                          id
                          name
                        }
                      }
                    """
                
                  Given path "/graphql"
                  And request { query: query, operationName: "TodoTest" }
                  When method post
                  Then status 200
                  And match response.data.todo == todoSchema

                Scenario: Perform a user query and validate the response
                  * text query = """
                      query UserTest {
                        user {
                          id
                          name
                          todos {
                            id
                            name
                          }
                        }
                      }
                    """
                
                  Given path "/graphql"
                  And request { query: query, operationName: "UserTest" }
                  When method post
                  Then status 200
                  And match response.data.user == userSchema
                """"
            ).SetName("Complex case with multiple karate objects and multiple queries is handled as expected.");

            yield return new TestCaseData(
                new List<KarateObject>
                {
                    new(
                        "Todo",
                        new List<KarateTypeBase>
                        {
                            new KarateNonNullType(new KarateType("number", "id")),
                            new KarateNonNullType(new KarateType("string", "name"))
                        }
                    ),
                    new(
                        "User",
                        new List<KarateTypeBase>
                        {
                            new KarateNonNullType(new KarateType("number", "id")),
                            new KarateNonNullType(new KarateType("string", "name")),
                            new KarateNonNullType(
                                new KarateListType(
                                    new KarateNonNullType(
                                        new KarateType("todoSchema", "todos")
                                    )
                                )
                            )
                        }
                    )
                },
                new List<GraphQLQueryFieldType>(),
                new Dictionary<string, string>(),
                """"
                Feature: Test GraphQL Endpoint with Karate
                
                Background: Base URL and Schemas
                  * url baseUrl
                
                  * text todoSchema = """
                      {
                        id: '#number',
                        name: '#string'
                      }
                    """
                
                  * text userSchema = """
                      {
                        id: '#number',
                        name: '#string',
                        todos: '#[] #todoSchema'
                      }
                    """
                """"
            ).SetName("When only Karate types are present and no queries are, it is handled as expected.");
        }
    }
}