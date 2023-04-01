using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Enums;
using GraphQLToKarate.Library.Features;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;
using NSubstitute;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Features;

[TestFixture]
internal sealed class KarateScenarioBuilderTests
{
    private IKarateScenarioBuilder? _subjectUnderTest;
    private IGraphQLDocumentAdapter? _mockGraphQLDocumentAdapter;

    [SetUp]
    public void SetUp()
    {
        _subjectUnderTest = new KarateScenarioBuilder(new GraphQLTypeConverterFactory(new GraphQLTypeConverter()));
        _mockGraphQLDocumentAdapter = Substitute.For<IGraphQLDocumentAdapter>();
    }

    [Test]
    [TestCaseSource(nameof(TestCases))]
    public void ScenarioBuilder_builds_expected_scenario_string(
        GraphQLOperation graphQLOperation,
        GraphQLUnionTypeDefinition? graphQLUnionTypeDefinitionReturn,
        bool isGraphQLTypeDefinitionWithFields,
        string expectedScenarioString)
    {
        // arrange
        _mockGraphQLDocumentAdapter!
            .GetGraphQLUnionTypeDefinition(Arg.Any<string>())
            .Returns(graphQLUnionTypeDefinitionReturn);

        if (graphQLUnionTypeDefinitionReturn is not null)
        {
            _mockGraphQLDocumentAdapter!
                .IsGraphQLUnionTypeDefinition(Arg.Any<string>())
                .Returns(true);
        }

        if (isGraphQLTypeDefinitionWithFields)
        {
            _mockGraphQLDocumentAdapter!
                .IsGraphQLTypeDefinitionWithFields(Arg.Any<string>())
                .Returns(true);
        }

        // act
        var scenarioString = _subjectUnderTest!.Build(graphQLOperation, _mockGraphQLDocumentAdapter!);

        // assert
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
                new GraphQLOperation(graphQLFieldDefinition)
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
                null,
                true,
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
                  And request { query: '#(query)', operationName: "TodoTest" }
                  When method post
                  Then status 200
                  And match response.data.todo == "##(todoSchema)"
                """"
            ).SetName("Simple query without arguments is generated as a valid scenario.");

            yield return new TestCaseData(
                new GraphQLOperation(graphQLFieldDefinition)
                {
                    Type = GraphQLOperationType.Query,
                    Arguments = new List<GraphQLArgumentTypeBase>
                    {
                        new GraphQLNonNullArgumentType(
                            new GraphQLArgumentType(
                                "id",
                                "id",
                                GraphQLToken.String,
                                "\"an example value\""
                            )
                        ),
                        new GraphQLArgumentType(
                            "isCompleted",
                            "isCompleted",
                            GraphQLToken.Boolean,
                            "true"
                        ),
                        new GraphQLNonNullArgumentType(
                            new GraphQLListArgumentType(
                                new GraphQLNonNullArgumentType(
                                    new GraphQLArgumentType(
                                        "filter",
                                        "filter",
                                        "Color",
                                        "[ RED, BLUE ]"
                                    )
                                )
                            )
                        )
                    },
                    OperationString =
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
                null,
                true,
                """"
                Scenario: Perform a todo query and validate the response
                  * text query =
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

                  * def variables =
                    """
                      {
                        id: "an example value",
                        isCompleted: true,
                        filter: [ RED, BLUE ]
                      }
                    """

                  Given path "/graphql"
                  And request { query: '#(query)', operationName: "TodoTest", variables: '#(variables)' }
                  When method post
                  Then status 200
                  And match response.data.todo == "##(todoSchema)"
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
                new GraphQLOperation(graphQLFieldDefinitionWithListReturn)
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
                null,
                true,
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
                  And request { query: '#(query)', operationName: "TodoTest" }
                  When method post
                  Then status 200
                  And match response.data.todo == "##[] ##(todoSchema)"
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
                new GraphQLOperation(graphQLFieldDefinitionWithNonNullListReturn)
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
                null,
                true,
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
                  And request { query: '#(query)', operationName: "TodoTest" }
                  When method post
                  Then status 200
                  And match response.data.todo == "#[] ##(todoSchema)"
                """"
            ).SetName("Simple query without arguments and non-null list return is generated as a valid scenario.");

            var graphQLFieldDefinitionWithUnionReturnType = new GraphQLFieldDefinition
            {
                Name = new GraphQLName("todoUnion"),
                Type = new GraphQLNonNullType
                {
                    Type = new GraphQLNamedType
                    {
                        Name = new GraphQLName("TodoUnion")
                    }
                }
            };

            yield return new TestCaseData(
                new GraphQLOperation(graphQLFieldDefinitionWithUnionReturnType)
                {
                    Type = GraphQLOperationType.Query,
                    Arguments = new List<GraphQLArgumentTypeBase>(),
                    OperationString =
                    """
                    query TodoUnionTest {
                      todoUnion {
                        ... on Todo {
                          id
                          name
                        }
                        ... on TodoError {
                          message
                        }
                      }
                    }
                    """
                },
                new GraphQLUnionTypeDefinition
                {
                    Name = new GraphQLName("TodoUnion"),
                    Types = new GraphQLUnionMemberTypes
                    {
                        Items = new List<GraphQLNamedType>
                        {
                            new()
                            {
                                Name = new GraphQLName("Todo")
                            },
                            new()
                            {
                                Name = new GraphQLName("TodoError")
                            }
                        }
                    }
                },
                false,
                """"
                Scenario: Perform a todoUnion query and validate the response
                  * text query =
                    """
                      query TodoUnionTest {
                        todoUnion {
                          ... on Todo {
                            id
                            name
                          }
                          ... on TodoError {
                            message
                          }
                        }
                      }
                    """

                  * def isValid =
                    """
                    response =>
                      karate.match(response, todoSchema).pass ||
                      karate.match(response, todoErrorSchema).pass
                    """

                  Given path "/graphql"
                  And request { query: '#(query)', operationName: "TodoUnionTest" }
                  When method post
                  Then status 200
                  And match response.data.todoUnion == "#? isValid(_)"
                """"
            ).SetName("Simple query with union return type is validated as expected.");

            var graphQLFieldDefinitionWithListUnionReturnType = new GraphQLFieldDefinition
            {
                Name = new GraphQLName("todoUnion"),
                Type = new GraphQLNonNullType
                {
                    Type = new GraphQLListType
                    {
                        Type = new GraphQLNamedType
                        {
                            Name = new GraphQLName("TodoUnion")
                        }
                    }
                }
            };

            yield return new TestCaseData(
                new GraphQLOperation(graphQLFieldDefinitionWithListUnionReturnType)
                {
                    Type = GraphQLOperationType.Query,
                    Arguments = new List<GraphQLArgumentTypeBase>(),
                    OperationString =
                    """
                    query TodoUnionTest {
                      todoUnion {
                        ... on Todo {
                          id
                          name
                        }
                        ... on TodoError {
                          message
                        }
                      }
                    }
                    """
                },
                new GraphQLUnionTypeDefinition
                {
                    Name = new GraphQLName("TodoUnion"),
                    Types = new GraphQLUnionMemberTypes
                    {
                        Items = new List<GraphQLNamedType>
                        {
                            new()
                            {
                                Name = new GraphQLName("Todo")
                            },
                            new()
                            {
                                Name = new GraphQLName("TodoError")
                            }
                        }
                    }
                },
                false,
                """"
                Scenario: Perform a todoUnion query and validate the response
                  * text query =
                    """
                      query TodoUnionTest {
                        todoUnion {
                          ... on Todo {
                            id
                            name
                          }
                          ... on TodoError {
                            message
                          }
                        }
                      }
                    """

                  * def isValid =
                    """
                    response =>
                      karate.match(response, todoSchema).pass ||
                      karate.match(response, todoErrorSchema).pass
                    """

                  Given path "/graphql"
                  And request { query: '#(query)', operationName: "TodoUnionTest" }
                  When method post
                  Then status 200
                  And match each response.data.todoUnion == "#? isValid(_)"
                """"
            ).SetName("Simple query with list union return type is validated as expected.");

            var graphQLFieldDefinitionWithScalarReturnType = new GraphQLFieldDefinition
            {
                Name = new GraphQLName("todoCount"),
                Type = new GraphQLNamedType
                {
                    Name = new GraphQLName("Int")
                }
            };

            yield return new TestCaseData(
                new GraphQLOperation(graphQLFieldDefinitionWithScalarReturnType)
                {
                    Type = GraphQLOperationType.Query,
                    Arguments = new List<GraphQLArgumentTypeBase>(),
                    OperationString =
                    """
                    query TodoCountTest {
                      todoCount
                    }
                    """
                },
                null,
                false,
                """"
                Scenario: Perform a todoCount query and validate the response
                  * text query =
                    """
                      query TodoCountTest {
                        todoCount
                      }
                    """

                  Given path "/graphql"
                  And request { query: '#(query)', operationName: "TodoCountTest" }
                  When method post
                  Then status 200
                  And match response.data.todoCount == "##number"
                """"
            ).SetName("Simple query with scalar return type is generated as a valid scenario.");
        }
    }
}