using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Enums;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Features;
using GraphQLToKarate.Library.Settings;
using GraphQLToKarate.Library.Types;
using NSubstitute;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Features;

[TestFixture]
internal sealed class KarateFeatureBuilderTests
{
    private IKarateScenarioBuilder? _mockScenarioBuilder;
    private IGraphQLDocumentAdapter? _mockGraphQLDocumentAdapter;

    [SetUp]
    public void SetUp()
    {
        _mockScenarioBuilder = Substitute.For<IKarateScenarioBuilder>();
        _mockGraphQLDocumentAdapter = Substitute.For<IGraphQLDocumentAdapter>();
    }

    [Test]
    [TestCaseSource(nameof(TestCases))]
    public void FeatureBuilder_builds_expected_feature_when_queries_are_included(
        IEnumerable<KarateObject> karateObjects,
        ICollection<GraphQLOperation> graphQLOperations,
        IDictionary<string, string> mockScenarioBuilderReturnByGraphQLOperationName,
        KarateFeatureBuilderSettings karateFeatureBuilderSettings,
        string expectedFeatureString)
    {
        // arrange
        foreach (var graphQLOperation in graphQLOperations)
        {
            _mockScenarioBuilder!
                .Build(graphQLOperation, _mockGraphQLDocumentAdapter!)
                .Returns(mockScenarioBuilderReturnByGraphQLOperationName[graphQLOperation.Name]);
        }

        var subjectUnderTest = new KarateFeatureBuilder(_mockScenarioBuilder!, karateFeatureBuilderSettings);

        // act
        var featureString = subjectUnderTest.Build(karateObjects, graphQLOperations, _mockGraphQLDocumentAdapter!);

        // assert
        featureString.Should().Be(expectedFeatureString);
    }

    private static IEnumerable<TestCaseData> TestCases
    {
        get
        {
            var graphQLFieldDefinition = new GraphQLFieldDefinition(
                new GraphQLName("todo"),
                new GraphQLNamedType(new GraphQLName("Todo"))
            );

            const string baseUrl = "\"https://www.karate-feature-builder-tests/graphql\"";

            var karateFeatureBuilderSettingsWithQueries = new KarateFeatureBuilderSettings
            {
                BaseUrl = baseUrl,
                ExcludeQueries = false
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
                new List<GraphQLOperation>
                {
                    new(graphQLFieldDefinition)
                    {
                        Type = GraphQLOperationType.Query,
                        Arguments = new List<GraphQLArgumentTypeBase>(),
                        OperationString =
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
                        graphQLFieldDefinition.NameValue(),
                        """"
                        Scenario: Perform a todo query and validate the response
                          * text query =
                            """
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
                karateFeatureBuilderSettingsWithQueries,
                """"
                Feature: Test GraphQL Endpoint with Karate

                Background: Base URL and Schemas
                  * url "https://www.karate-feature-builder-tests/graphql"

                  * def todoSchema =
                    """
                      {
                        id: '#number',
                        name: '#string'
                      }
                    """

                Scenario: Perform a todo query and validate the response
                  * text query =
                    """
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

            var otherGraphQLFieldDefinition = new GraphQLFieldDefinition(
                new GraphQLName("user"),
                new GraphQLNamedType(new GraphQLName("User"))
            );

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
                new List<GraphQLOperation>
                {
                    new(graphQLFieldDefinition)
                    {
                        Type = GraphQLOperationType.Query,
                        Arguments = new List<GraphQLArgumentTypeBase>(),
                        OperationString =
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
                        Type = GraphQLOperationType.Query,
                        Arguments = new List<GraphQLArgumentTypeBase>(),
                        OperationString =
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
                        graphQLFieldDefinition.NameValue(),
                        """"
                        Scenario: Perform a todo query and validate the response
                          * text query =
                            """
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
                        otherGraphQLFieldDefinition.NameValue(),
                        """"
                        Scenario: Perform a user query and validate the response
                          * text query =
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

                          Given path "/graphql"
                          And request { query: query, operationName: "UserTest" }
                          When method post
                          Then status 200
                          And match response.data.user == userSchema
                        """"
                    }
                },
                karateFeatureBuilderSettingsWithQueries,
                """"
                Feature: Test GraphQL Endpoint with Karate

                Background: Base URL and Schemas
                  * url "https://www.karate-feature-builder-tests/graphql"

                  * def todoSchema =
                    """
                      {
                        id: '#number',
                        name: '#string'
                      }
                    """

                  * def userSchema =
                    """
                      {
                        id: '#number',
                        name: '#string',
                        todos: '#[] #todoSchema'
                      }
                    """

                Scenario: Perform a todo query and validate the response
                  * text query =
                    """
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
                  * text query =
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
                new List<GraphQLOperation>(),
                new Dictionary<string, string>(),
                karateFeatureBuilderSettingsWithQueries,
                """"
                Feature: Test GraphQL Endpoint with Karate
                
                Background: Base URL and Schemas
                  * url "https://www.karate-feature-builder-tests/graphql"
                
                  * def todoSchema =
                    """
                      {
                        id: '#number',
                        name: '#string'
                      }
                    """
                
                  * def userSchema =
                    """
                      {
                        id: '#number',
                        name: '#string',
                        todos: '#[] #todoSchema'
                      }
                    """
                """"
            ).SetName("When only Karate types are present and no queries are, it is handled as expected.");

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
                new List<GraphQLOperation>
                {
                    new(graphQLFieldDefinition)
                    {
                        Type = GraphQLOperationType.Query,
                        Arguments = new List<GraphQLArgumentTypeBase>(),
                        OperationString =
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
                        graphQLFieldDefinition.NameValue(),
                        """"
                        Scenario: Perform a todo query and validate the response
                          * text query =
                            """
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
                new KarateFeatureBuilderSettings
                {
                    BaseUrl = baseUrl,
                    ExcludeQueries = true
                },
                """"
                Feature: Test GraphQL Endpoint with Karate

                Background: Base URL and Schemas
                  * url "https://www.karate-feature-builder-tests/graphql"

                  * def todoSchema =
                    """
                      {
                        id: '#number',
                        name: '#string'
                      }
                    """
                """"
            ).SetName("Simple case with one Karate object schema and one query is handled as expected when queries are excluded.");
        }
    }
}