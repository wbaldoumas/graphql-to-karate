using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Features;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Features;

[TestFixture]
internal sealed class KarateScenarioBuilderTests
{
    private IKarateScenarioBuilder? _subjectUnderTest;

    [SetUp]
    public void SetUp() => _subjectUnderTest = new KarateScenarioBuilder();

    [Test]
    [TestCaseSource(nameof(TestCases))]
    public void ScenarioBuilder_builds_expected_scenario_string(
        GraphQLQueryFieldType graphQLQueryFieldType,
        string expectedScenarioString)
    {
        var scenarioString = _subjectUnderTest!.Build(graphQLQueryFieldType);

        scenarioString.Should().Be(expectedScenarioString);
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
                new GraphQLQueryFieldType(graphQLFieldDefinition)
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
            ).SetName("Simple query without arguments is generated as a valid scenario.");

            yield return new TestCaseData(
                new GraphQLQueryFieldType(graphQLFieldDefinition)
                {
                    Arguments = new List<GraphQLArgumentTypeBase>
                    {
                        new GraphQLNonNullArgumentType(new GraphQLArgumentType("id", "id", GraphQLToken.String)),
                        new GraphQLArgumentType("isCompleted", "isCompleted", GraphQLToken.Boolean),
                        new GraphQLNonNullArgumentType(
                            new GraphQLListArgumentType(
                                new GraphQLNonNullArgumentType(
                                    new GraphQLArgumentType("filter", "filter", "Color")
                                )
                            )
                        )
                    },
                    QueryString =
                    """
                    query TodoTest($id: String!, $isCompleted: Boolean, $filter: [Color!]!) {
                      todo(id: $id, isCompleted: $isCompleted) {
                        id
                        name
                        completed
                        color
                        colors(filter: $filter)
                      }
                    }
                    """
                },
                """"
                Scenario: Perform a todo query and validate the response
                  * text query = """
                      query TodoTest($id: String!, $isCompleted: Boolean, $filter: [Color!]!) {
                        todo(id: $id, isCompleted: $isCompleted) {
                          id
                          name
                          completed
                          color
                          colors(filter: $filter)
                        }
                      }
                    """

                  * text variables = """
                      {
                        "id": <some value>
                        "isCompleted": <some value>
                        "filter": <some value>
                      }
                    """

                  Given path "/graphql"
                  And request { query: query, operationName: "TodoTest", variables: variables }
                  When method post
                  Then status 200
                  And match response.data.todo == todoSchema
                """"
            ).SetName("Simple query with arguments is generated as a valid scenario.");

            var graphQLFieldDefinitionWithListReturn = new GraphQLFieldDefinition
            {
                Name = new GraphQLName("todo"),
                Type = new GraphQLListType
                {
                    Type = new GraphQLNamedType
                    {
                        Name = new GraphQLName("Todo")
                    }
                }
            };

            yield return new TestCaseData(
                new GraphQLQueryFieldType(graphQLFieldDefinitionWithListReturn)
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
                  And match each response.data.todo == todoSchema
                """"
            ).SetName("Simple query without arguments and list return is generated as a valid scenario.");

            var graphQLFieldDefinitionWithNonNullListReturn = new GraphQLFieldDefinition
            {
                Name = new GraphQLName("todo"),
                Type = new GraphQLNonNullType
                {
                    Type = new GraphQLListType
                    {
                        Type = new GraphQLNamedType
                        {
                            Name = new GraphQLName("Todo")
                        }
                    }
                }
            };

            yield return new TestCaseData(
                new GraphQLQueryFieldType(graphQLFieldDefinitionWithNonNullListReturn)
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
                  And match each response.data.todo == todoSchema
                """"
            ).SetName("Simple query without arguments and non-null list return is generated as a valid scenario.");
        }
    }
}