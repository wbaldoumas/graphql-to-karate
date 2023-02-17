using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Adapters;

[TestFixture]
internal sealed class GraphQLDocumentAdapterTests
{
    [Test]
    [TestCaseSource(nameof(IsGraphQLEnumTypeDefinitionTestCases))]
    public void IsGraphQLEnumTypeDefinition_returns_expected_result(
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        string graphQLTypeDefinitionName,
        bool expectedResult)
    {
        // act + assert
        graphQLDocumentAdapter
            .IsGraphQLEnumTypeDefinition(graphQLTypeDefinitionName)
            .Should()
            .Be(expectedResult);
    }

    private static IEnumerable<TestCaseData> IsGraphQLEnumTypeDefinitionTestCases
    {
        get
        {
            const string enumTypeDefinitionName = "test";

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(new GraphQLDocument()),
                enumTypeDefinitionName,
                false
            ).SetName("When GraphQL document is empty, enum type definition is not found");

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(
                    new GraphQLDocument
                    {
                        Definitions = new List<ASTNode>
                        {
                            new GraphQLObjectTypeDefinition
                            {
                                Name = new GraphQLName("Goodbye")
                            }
                        }
                    }
                ),
                enumTypeDefinitionName,
                false
            ).SetName(
                "When GraphQL document is not empty but doesn't have enum type definitions, enum type definition is not found"
            );

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(
                    new GraphQLDocument
                    {
                        Definitions = new List<ASTNode>
                        {
                            new GraphQLEnumTypeDefinition
                            {
                                Name = new GraphQLName("Hello")
                            }
                        }
                    }
                ),
                enumTypeDefinitionName,
                false
            ).SetName(
                "When GraphQL document is not empty but doesn't have specific enum type definition, enum type definition is not found"
            );

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(
                    new GraphQLDocument
                    {
                        Definitions = new List<ASTNode>
                        {
                            new GraphQLEnumTypeDefinition
                            {
                                Name = new GraphQLName(enumTypeDefinitionName)
                            }
                        }
                    }
                ),
                enumTypeDefinitionName,
                true
            ).SetName("When GraphQL document has specific enum type definitions, enum type definition is found");
        }
    }

    [Test]
    [TestCaseSource(nameof(IsGraphQLTypeDefinitionWithFieldsTestCases))]
    public void IsGraphQLTypeDefinitionWithFields_returns_expected_result(
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        string graphQLTypeDefinitionName,
        bool expectedResult)
    {
        // act + assert
        graphQLDocumentAdapter
            .IsGraphQLTypeDefinitionWithFields(graphQLTypeDefinitionName)
            .Should()
            .Be(expectedResult);
    }

    private static IEnumerable<TestCaseData> IsGraphQLTypeDefinitionWithFieldsTestCases
    {
        get
        {
            const string hasFieldsTypeDefinitionName = "test";

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(new GraphQLDocument()),
                hasFieldsTypeDefinitionName,
                false
            ).SetName("When GraphQL document is empty, has fields type definition is not found");

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(
                    new GraphQLDocument
                    {
                        Definitions = new List<ASTNode>
                        {
                            new GraphQLEnumTypeDefinition
                            {
                                Name = new GraphQLName("Goodbye")
                            }
                        }
                    }
                ),
                hasFieldsTypeDefinitionName,
                false
            ).SetName(
                "When GraphQL document is not empty but doesn't have has fields type definitions, has fields type definition is not found"
            );

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(
                    new GraphQLDocument
                    {
                        Definitions = new List<ASTNode>
                        {
                            new GraphQLObjectTypeDefinition
                            {
                                Name = new GraphQLName("Hello")
                            }
                        }
                    }
                ),
                hasFieldsTypeDefinitionName,
                false
            ).SetName(
                "When GraphQL document is not empty but doesn't have specific has fields type definition, has fields type definition is not found"
            );

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(
                    new GraphQLDocument
                    {
                        Definitions = new List<ASTNode>
                        {
                            new GraphQLObjectTypeDefinition
                            {
                                Name = new GraphQLName(hasFieldsTypeDefinitionName)
                            }
                        }
                    }
                ),
                hasFieldsTypeDefinitionName,
                true
            ).SetName(
                "When GraphQL document has specific has fields type definitions, has fields type definition is found"
            );
        }
    }

    [Test]
    [TestCaseSource(nameof(IsGraphQLUnionTypeDefinitionTestCases))]
    public void IsGraphQLUnionTypeDefinition_returns_expected_result(
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        string graphQLTypeDefinitionName,
        bool expectedResult)
    {
        // act + assert
        graphQLDocumentAdapter
            .IsGraphQLUnionTypeDefinition(graphQLTypeDefinitionName)
            .Should()
            .Be(expectedResult);
    }

    private static IEnumerable<TestCaseData> IsGraphQLUnionTypeDefinitionTestCases
    {
        get
        {
            const string unionTypeDefinitionName = "test";

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(new GraphQLDocument()),
                unionTypeDefinitionName,
                false
            ).SetName("When GraphQL document is empty, union type definition is not found");

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(
                    new GraphQLDocument
                    {
                        Definitions = new List<ASTNode>
                        {
                            new GraphQLEnumTypeDefinition
                            {
                                Name = new GraphQLName("Goodbye")
                            }
                        }
                    }
                ),
                unionTypeDefinitionName,
                false
            ).SetName(
                "When GraphQL document is not empty but doesn't have union type definition, union type definition is not found"
            );

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(
                    new GraphQLDocument
                    {
                        Definitions = new List<ASTNode>
                        {
                            new GraphQLUnionTypeDefinition
                            {
                                Name = new GraphQLName("Hello")
                            }
                        }
                    }
                ),
                unionTypeDefinitionName,
                false
            ).SetName(
                "When GraphQL document is not empty but doesn't have specific union type definition, union type definition is not found"
            );

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(
                    new GraphQLDocument
                    {
                        Definitions = new List<ASTNode>
                        {
                            new GraphQLUnionTypeDefinition
                            {
                                Name = new GraphQLName(unionTypeDefinitionName)
                            }
                        }
                    }
                ),
                unionTypeDefinitionName,
                true
            ).SetName("When GraphQL document has specific union type definition, union type definition is found");
        }
    }

    [Test]
    [TestCaseSource(nameof(GetGraphQLTypeDefinitionWithFieldsTestCases))]
    public void GetGraphQLTypeDefinitionWithFields_returns_expected_result(
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        string graphQLTypeDefinitionName,
        IHasFieldsDefinitionNode? expectedResult)
    {
        // act + assert
        graphQLDocumentAdapter
            .GetGraphQLTypeDefinitionWithFields(graphQLTypeDefinitionName)
            .Should()
            .BeEquivalentTo(expectedResult);
    }

    private static IEnumerable<TestCaseData> GetGraphQLTypeDefinitionWithFieldsTestCases
    {
        get
        {
            const string hasFieldsTypeDefinitionName = "test";
            const string otherTypeDefinitionName = "other";

            ASTNode definition = new GraphQLObjectTypeDefinition
            {
                Name = new GraphQLName(hasFieldsTypeDefinitionName)
            };

            var graphQLDocumentAdapter = new GraphQLDocumentAdapter(
                new GraphQLDocument
                {
                    Definitions = new List<ASTNode>
                    {
                        definition
                    }
                }
            );

            yield return new TestCaseData(
                graphQLDocumentAdapter,
                otherTypeDefinitionName,
                null
            ).SetName("When non-fields type definition name is passed, null is returned.");

            yield return new TestCaseData(
                graphQLDocumentAdapter,
                hasFieldsTypeDefinitionName,
                definition as IHasFieldsDefinitionNode
            ).SetName("When has fields type definition name is passed, has fields type definition is returned.");
        }
    }

    [Test]
    [TestCaseSource(nameof(GetGraphQLUnionTypeDefinitionTestCases))]
    public void GetGraphQLUnionTypeDefinition_returns_expected_result(
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        string graphQLTypeDefinitionName,
        GraphQLUnionTypeDefinition? expectedResult)
    {
        // act + assert
        graphQLDocumentAdapter
            .GetGraphQLUnionTypeDefinition(graphQLTypeDefinitionName)
            .Should()
            .BeEquivalentTo(expectedResult);
    }

    [Test]
    [TestCaseSource(nameof(GetGraphQLEnumTypeDefinitionTestCases))]
    public void GetGraphQLEnumTypeDefinition_returns_expected_result(
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        string graphQLTypeDefinitionName,
        GraphQLEnumTypeDefinition? expectedResult)
    {
        // act + assert
        graphQLDocumentAdapter
            .GetGraphQLEnumTypeDefinition(graphQLTypeDefinitionName)
            .Should()
            .BeEquivalentTo(expectedResult);
    }

    private static IEnumerable<TestCaseData> GetGraphQLUnionTypeDefinitionTestCases
    {
        get
        {
            const string unionTypeDefinitionName = "test";
            const string otherTypeDefinitionName = "other";

            ASTNode definition = new GraphQLUnionTypeDefinition
            {
                Name = new GraphQLName(unionTypeDefinitionName)
            };

            var graphQLDocumentAdapter = new GraphQLDocumentAdapter(
                new GraphQLDocument
                {
                    Definitions = new List<ASTNode>
                    {
                        definition
                    }
                }
            );

            yield return new TestCaseData(
                graphQLDocumentAdapter,
                otherTypeDefinitionName,
                null
            ).SetName("When non-union type definition name is passed, null is returned.");

            yield return new TestCaseData(
                graphQLDocumentAdapter,
                unionTypeDefinitionName,
                definition as GraphQLUnionTypeDefinition
            ).SetName("When union type definition name is passed, union type definition is returned.");
        }
    }

    private static IEnumerable<TestCaseData> GetGraphQLEnumTypeDefinitionTestCases
    {
        get
        {
            yield return new TestCaseData(
                new GraphQLDocumentAdapter(new GraphQLDocument()),
                "test",
                null
            ).SetName("When GraphQL document is empty, enum type definition is not found");

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(
                    new GraphQLDocument
                    {
                        Definitions = new List<ASTNode>
                        {
                            new GraphQLEnumTypeDefinition
                            {
                                Name = new GraphQLName("Goodbye")
                            }
                        }
                    }
                ),
                "test",
                null
            ).SetName(
                "When GraphQL document is not empty but doesn't have enum type definition, enum type definition is not found"
            );

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(
                    new GraphQLDocument
                    {
                        Definitions = new List<ASTNode>
                        {
                            new GraphQLEnumTypeDefinition
                            {
                                Name = new GraphQLName("Hello")
                            }
                        }
                    }
                ),
                "test",
                null
            ).SetName(
                "When GraphQL document is not empty but doesn't have specific enum type definition, enum type definition is not found"
            );

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(
                    new GraphQLDocument
                    {
                        Definitions = new List<ASTNode>
                        {
                            new GraphQLEnumTypeDefinition
                            {
                                Name = new GraphQLName("test")
                            }
                        }
                    }
                ),
                "test",
                new GraphQLEnumTypeDefinition
                {
                    Name = new GraphQLName("test")
                }
            ).SetName("When GraphQL document has specific enum type definition, enum type definition is found");
        }
    }
}
