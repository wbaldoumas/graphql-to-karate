using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
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
        ICollection<GraphQLQueryFieldType> graphQLQueryFieldTypes,
        IDictionary<string, string> mockScenarioBuilderReturnByGraphQLQueryName,
        KarateFeatureBuilderSettings karateFeatureBuilderSettings,
        string expectedFeatureString)
    {
        // arrange
        foreach (var graphQLQueryFieldType in graphQLQueryFieldTypes)
        {
            _mockScenarioBuilder!
                .Build(graphQLQueryFieldType, _mockGraphQLDocumentAdapter!)
                .Returns(mockScenarioBuilderReturnByGraphQLQueryName[graphQLQueryFieldType.Name]);
        }

        var subjectUnderTest = new KarateFeatureBuilder(_mockScenarioBuilder!, karateFeatureBuilderSettings);

        // act
        var featureString = subjectUnderTest.Build(karateObjects, graphQLQueryFieldTypes, _mockGraphQLDocumentAdapter!);

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
                karateFeatureBuilderSettingsWithQueries,
                """"
                Feature: Test GraphQL Endpoint with Karate

                Background: Base URL and Schemas
                  * url "https://www.karate-feature-builder-tests/graphql"

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

            var otherGraphQLFieldDefinition = new GraphQLFieldDefinition
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
                karateFeatureBuilderSettingsWithQueries,
                """"
                Feature: Test GraphQL Endpoint with Karate

                Background: Base URL and Schemas
                  * url "https://www.karate-feature-builder-tests/graphql"

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
                karateFeatureBuilderSettingsWithQueries,
                """"
                Feature: Test GraphQL Endpoint with Karate
                
                Background: Base URL and Schemas
                  * url "https://www.karate-feature-builder-tests/graphql"
                
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
                new KarateFeatureBuilderSettings
                {
                    BaseUrl = baseUrl,
                    ExcludeQueries = true
                },
                """"
                Feature: Test GraphQL Endpoint with Karate

                Background: Base URL and Schemas
                  * url "https://www.karate-feature-builder-tests/graphql"

                  * text todoSchema = """
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