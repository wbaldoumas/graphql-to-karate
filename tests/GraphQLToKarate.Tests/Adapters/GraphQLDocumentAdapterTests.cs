using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Tokens;
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
    [TestCaseSource(nameof(IsGraphQLInputObjectTypeDefinitionTestCases))]
    public void IsGraphQLInputObjectTypeDefinition_returns_expected_result(
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        string graphQLTypeDefinitionName,
        bool expectedResult)
    {
        // act + assert
        graphQLDocumentAdapter
            .IsGraphQLInputObjectTypeDefinition(graphQLTypeDefinitionName)
            .Should()
            .Be(expectedResult);
    }

    private static IEnumerable<TestCaseData> IsGraphQLInputObjectTypeDefinitionTestCases
    {
        get
        {
            yield return new TestCaseData(
                new GraphQLDocumentAdapter(new GraphQLDocument()),
                "test",
                false
            ).SetName("When GraphQL document is empty, input object type definition is not found");

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
                false
            ).SetName(
                "When GraphQL document is not empty but doesn't have input object type definition, input object type definition is not found"
            );

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(
                    new GraphQLDocument
                    {
                        Definitions = new List<ASTNode>
                        {
                            new GraphQLInputObjectTypeDefinition
                            {
                                Name = new GraphQLName("Hello")
                            }
                        }
                    }
                ),
                "test",
                false
            ).SetName(
                "When GraphQL document is not empty but doesn't have specific input object type definition, input object type definition is not found"
            );

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(
                    new GraphQLDocument
                    {
                        Definitions = new List<ASTNode>
                        {
                            new GraphQLInputObjectTypeDefinition
                            {
                                Name = new GraphQLName("test")
                            }
                        }
                    }
                ),
                "test",
                true
            ).SetName(
                "When GraphQL document has specific input object type definition, input object type definition is found"
            );
        }
    }

    [Test]
    [TestCaseSource(nameof(GetGraphQLTypeDefinitionWithFieldsTestCases))]
    public void GetGraphQLTypeDefinitionWithFields_returns_expected_result(
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        string graphQLTypeDefinitionName,
        IHasFieldsDefinitionNode? expectedResult)
    {
        // act
        var graphQLTypeDefinitionWithFields = graphQLDocumentAdapter.GetGraphQLTypeDefinitionWithFields(graphQLTypeDefinitionName);

        // assert
        graphQLTypeDefinitionWithFields.Should().BeEquivalentTo(expectedResult);
    }

    private static IEnumerable<TestCaseData> GetGraphQLTypeDefinitionWithFieldsTestCases
    {
        get
        {
            const string graphQLObjectTypeDefinitionName = "object";
            const string graphQLInterfaceTypeDefinitionName = "interface";
            const string otherTypeDefinitionName = "other";

            var graphQLObjectTypeDefinition = new GraphQLObjectTypeDefinition
            {
                Name = new GraphQLName(graphQLObjectTypeDefinitionName),
                Fields = new GraphQLFieldsDefinition
                {
                    Items = new List<GraphQLFieldDefinition>
                    {
                        new()
                        {
                            Name = new GraphQLName("mainField")
                        }
                    }
                }
            };

            var graphQLInterfaceTypeDefinition = new GraphQLInterfaceTypeDefinition
            {
                Name = new GraphQLName(graphQLInterfaceTypeDefinitionName),
                Fields = new GraphQLFieldsDefinition
                {
                    Items = new List<GraphQLFieldDefinition>
                    {
                        new()
                        {
                            Name = new GraphQLName("mainField")
                        }
                    }
                }
            };

            var otherGraphQLObjectTypeExtension = new GraphQLObjectTypeExtension
            {
                Name = new GraphQLName("fakeType"),
                Fields = new GraphQLFieldsDefinition
                {
                    Items = new List<GraphQLFieldDefinition>
                    {
                        new()
                        {
                            Name = new GraphQLName("huh")
                        }
                    }
                }
            };

            var graphQLObjectTypeExtension = new GraphQLObjectTypeExtension
            {
                Name = new GraphQLName(graphQLObjectTypeDefinitionName),
                Fields = new GraphQLFieldsDefinition
                {
                    Items = new List<GraphQLFieldDefinition>
                    {
                        new()
                        {
                            Name = new GraphQLName("extendedField")
                        }
                    }
                }
            };

            var graphQLInterfaceTypeExtension = new GraphQLInterfaceTypeExtension
            {
                Name = graphQLInterfaceTypeDefinition.Name,
                Fields = new GraphQLFieldsDefinition
                {
                    Items = new List<GraphQLFieldDefinition>
                    {
                        new()
                        {
                            Name = new GraphQLName("extendedField")
                        }
                    }
                }
            };

            var graphQLDocumentAdapter = new GraphQLDocumentAdapter(
                new GraphQLDocument
                {
                    Definitions = new List<ASTNode>
                    {
                        graphQLObjectTypeDefinition,
                        graphQLObjectTypeExtension,
                        otherGraphQLObjectTypeExtension,
                        graphQLInterfaceTypeExtension,
                        graphQLInterfaceTypeDefinition
                    }
                }
            );

            var expectedGraphQLObjectTypeDefinition = new GraphQLObjectTypeDefinition
            {
                Name = graphQLObjectTypeDefinition.Name,
                Fields = new GraphQLFieldsDefinition
                {
                    Items = new List<GraphQLFieldDefinition>
                    {
                        new()
                        {
                            Name = new GraphQLName("mainField")
                        },
                        new()
                        {
                            Name = new GraphQLName("extendedField")
                        }
                    }
                }
            };

            yield return new TestCaseData(
                graphQLDocumentAdapter,
                otherTypeDefinitionName,
                null
            ).SetName("When non-fields type definition name is passed, null is returned.");

            yield return new TestCaseData(
                graphQLDocumentAdapter,
                graphQLObjectTypeDefinitionName,
                expectedGraphQLObjectTypeDefinition
            ).SetName("When has fields type definition name is passed, has fields type definition is returned.");

            var expectedGraphQLInterfaceTypeDefinition = new GraphQLObjectTypeDefinition
            {
                Name = graphQLInterfaceTypeDefinition.Name,
                Fields = new GraphQLFieldsDefinition
                {
                    Items = new List<GraphQLFieldDefinition>
                    {
                        new()
                        {
                            Name = new GraphQLName("mainField")
                        },
                        new()
                        {
                            Name = new GraphQLName("extendedField")
                        }
                    }
                }
            };

            yield return new TestCaseData(
                graphQLDocumentAdapter,
                graphQLInterfaceTypeDefinitionName,
                expectedGraphQLInterfaceTypeDefinition
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

    private static IEnumerable<TestCaseData> GetGraphQLUnionTypeDefinitionTestCases
    {
        get
        {
            const string unionTypeDefinitionName = "test";
            const string otherTypeDefinitionName = "other";

            var graphQLUnionTypeDefinition = new GraphQLUnionTypeDefinition
            {
                Name = new GraphQLName(unionTypeDefinitionName),
                Types = new GraphQLUnionMemberTypes
                {
                    Items = new List<GraphQLNamedType>
                    {
                        new()
                        {
                            Name = new GraphQLName("mainType")
                        },
                        new()
                        {
                            Name = new GraphQLName("secondType")
                        }
                    }
                }
            };

            var graphQLUnionTypeExtension = new GraphQLUnionTypeExtension
            {
                Name = graphQLUnionTypeDefinition.Name,
                Types = new GraphQLUnionMemberTypes
                {
                    Items = new List<GraphQLNamedType>
                    {
                        new()
                        {
                            Name = new GraphQLName("thirdType")
                        }
                    }
                }
            };

            var otherGraphQLUnionTypeExtension = new GraphQLUnionTypeExtension
            {
                Name = new GraphQLName("somethingElse"),
                Types = new GraphQLUnionMemberTypes
                {
                    Items = new List<GraphQLNamedType>
                    {
                        new()
                        {
                            Name = new GraphQLName("otherType")
                        }
                    }
                }
            };

            var graphQLDocumentAdapter = new GraphQLDocumentAdapter(
                new GraphQLDocument
                {
                    Definitions = new List<ASTNode>
                    {
                        graphQLUnionTypeDefinition,
                        graphQLUnionTypeExtension,
                        otherGraphQLUnionTypeExtension
                    }
                }
            );

            var expectedGraphQLUnionTypeDefinition = new GraphQLUnionTypeDefinition
            {
                Name = new GraphQLName(unionTypeDefinitionName),
                Types = new GraphQLUnionMemberTypes
                {
                    Items = new List<GraphQLNamedType>
                    {
                        new()
                        {
                            Name = new GraphQLName("mainType")
                        },
                        new()
                        {
                            Name = new GraphQLName("secondType")
                        },
                        new()
                        {
                            Name = new GraphQLName("thirdType")
                        }
                    }
                }
            };

            yield return new TestCaseData(
                graphQLDocumentAdapter,
                otherTypeDefinitionName,
                null
            ).SetName("When non-union type definition name is passed, null is returned.");

            yield return new TestCaseData(
                graphQLDocumentAdapter,
                unionTypeDefinitionName,
                expectedGraphQLUnionTypeDefinition
            ).SetName("When union type definition name is passed, union type definition is returned.");
        }
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

    private static IEnumerable<TestCaseData> GetGraphQLEnumTypeDefinitionTestCases
    {
        get
        {
            yield return new TestCaseData(
                new GraphQLDocumentAdapter(new GraphQLDocument()),
                "test",
                null
            ).SetName("When GraphQL document is empty, enum type definition is not found");

            var graphQLEnumTypeDefinitionName = "Color";

            var graphQLEnumTypeDefinition = new GraphQLEnumTypeDefinition
            {
                Name = new GraphQLName(graphQLEnumTypeDefinitionName),
                Values = new GraphQLEnumValuesDefinition
                {
                    Items = new List<GraphQLEnumValueDefinition>
                    {
                        new()
                        {
                            Name = new GraphQLName("RED")
                        },
                        new()
                        {
                            Name = new GraphQLName("BLUE")
                        }
                    }
                }
            };

            var graphQLEnumTypeExtension = new GraphQLEnumTypeExtension
            {
                Name = graphQLEnumTypeDefinition.Name,
                Values = new GraphQLEnumValuesDefinition
                {
                    Items = new List<GraphQLEnumValueDefinition>
                    {
                        new()
                        {
                            Name = new GraphQLName("GREEN")
                        },
                        new()
                        {
                            Name = new GraphQLName("YELLOW")
                        }
                    }
                }
            };

            var otherGraphQLEnumTypeExtension = new GraphQLEnumTypeExtension
            {
                Name = new GraphQLName("somethingElse"),
                Values = new GraphQLEnumValuesDefinition
                {
                    Items = new List<GraphQLEnumValueDefinition>
                    {
                        new()
                        {
                            Name = new GraphQLName("SMALL")
                        },
                        new()
                        {
                            Name = new GraphQLName("LARGE")
                        }
                    }
                }
            };

            var graphQLDocumentAdapter = new GraphQLDocumentAdapter(
                new GraphQLDocument
                {
                    Definitions = new List<ASTNode>
                    {
                        graphQLEnumTypeDefinition,
                        graphQLEnumTypeExtension,
                        otherGraphQLEnumTypeExtension
                    }
                }
            );

            yield return new TestCaseData(
                graphQLDocumentAdapter,
                graphQLEnumTypeDefinition.NameValue(),
                new GraphQLEnumTypeDefinition
                {
                    Name = new GraphQLName(graphQLEnumTypeDefinitionName),
                    Values = new GraphQLEnumValuesDefinition
                    {
                        Items = new List<GraphQLEnumValueDefinition>
                        {
                            new()
                            {
                                Name = new GraphQLName("RED")
                            },
                            new()
                            {
                                Name = new GraphQLName("BLUE")
                            },
                            new()
                            {
                                Name = new GraphQLName("GREEN")
                            },
                            new()
                            {
                                Name = new GraphQLName("YELLOW")
                            }
                        }
                    }
                }
            ).SetName("When GraphQL document has specific enum type definition, enum type definition is found");
        }
    }

    [Test]
    [TestCaseSource(nameof(GetGraphQLInputObjectTypeDefinitionTestCases))]
    public void GetGraphQLInputObjectTypeDefinition_returns_expected_result(
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        string graphQLTypeDefinitionName,
        GraphQLInputObjectTypeDefinition? expectedResult)
    {
        // act + assert
        graphQLDocumentAdapter
            .GetGraphQLInputObjectTypeDefinition(graphQLTypeDefinitionName)
            .Should()
            .BeEquivalentTo(expectedResult);
    }

    private static IEnumerable<TestCaseData> GetGraphQLInputObjectTypeDefinitionTestCases
    {
        get
        {
            yield return new TestCaseData(
                new GraphQLDocumentAdapter(new GraphQLDocument()),
                "test",
                null
            ).SetName("When GraphQL document is empty, input object type definition is not found");

            var graphQLInputObjectTypeDefinition = new GraphQLInputObjectTypeDefinition
            {
                Name = new GraphQLName("Hello"),
                Fields = new GraphQLInputFieldsDefinition
                {
                    Items = new List<GraphQLInputValueDefinition>
                    {
                        new()
                        {
                            Name = new GraphQLName("mainField")
                        }
                    }
                }
            };

            var graphQLInputObjectTypeExtension = new GraphQLInputObjectTypeExtension
            {
                Name = graphQLInputObjectTypeDefinition.Name,
                Fields = new GraphQLInputFieldsDefinition
                {
                    Items = new List<GraphQLInputValueDefinition>
                    {
                        new()
                        {
                            Name = new GraphQLName("otherField")
                        }
                    }
                }
            };

            var otherGraphQLInputObjectTypeExtension = new GraphQLInputObjectTypeExtension
            {
                Name = new GraphQLName("somethingElse"),
                Fields = new GraphQLInputFieldsDefinition
                {
                    Items = new List<GraphQLInputValueDefinition>
                    {
                        new()
                        {
                            Name = new GraphQLName("yetAnotherField")
                        }
                    }
                }
            };

            var graphQLDocumentAdapter = new GraphQLDocumentAdapter(
                new GraphQLDocument
                {
                    Definitions = new List<ASTNode>
                    {
                        graphQLInputObjectTypeDefinition,
                        graphQLInputObjectTypeExtension,
                        otherGraphQLInputObjectTypeExtension
                    }
                }
            );

            yield return new TestCaseData(
                graphQLDocumentAdapter,
                "test",
                null
            ).SetName(
                "When GraphQL document is not empty but doesn't have specific input object type definition, input object type definition is not found"
            );

            yield return new TestCaseData(
                graphQLDocumentAdapter,
                graphQLInputObjectTypeDefinition.NameValue(),
                new GraphQLInputObjectTypeDefinition
                {
                    Name = new GraphQLName("Hello"),
                    Fields = new GraphQLInputFieldsDefinition
                    {
                        Items = new List<GraphQLInputValueDefinition>
                        {
                            new()
                            {
                                Name = new GraphQLName("mainField")
                            },
                            new()
                            {
                                Name = new GraphQLName("otherField")
                            }
                        }
                    }
                }
            ).SetName(
                "When GraphQL document has specific input object type definition, input object type definition is found"
            );
        }
    }

    [Test]
    [TestCaseSource(nameof(GraphQLObjectTypeDefinitionsTestCases))]
    public void GraphQLObjectTypeDefinitions_returns_expected_result(
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        IEnumerable<GraphQLObjectTypeDefinition> expectedResult
    ) => graphQLDocumentAdapter
        .GraphQLObjectTypeDefinitions
        .Should()
        .BeEquivalentTo(expectedResult);

    private static IEnumerable<TestCaseData> GraphQLObjectTypeDefinitionsTestCases
    {
        get
        {
            yield return new TestCaseData(
                new GraphQLDocumentAdapter(new GraphQLDocument()),
                new List<GraphQLObjectTypeDefinition>()
            ).SetName("GraphQL document with no definitions generates expected result");

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(new GraphQLDocument
                {
                    Definitions = new List<ASTNode>
                    {
                        new GraphQLInterfaceTypeDefinition
                        {
                            Name = new GraphQLName("SomeInterfaceType")
                        }
                    }
                }),
                new List<GraphQLObjectTypeDefinition>()
            ).SetName("GraphQL document with no object type definitions generates expected result");

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(new GraphQLDocument
                {
                    Definitions = new List<ASTNode>
                    {
                        new GraphQLInterfaceTypeDefinition
                        {
                            Name = new GraphQLName("SomeInterfaceType")
                        },
                        new GraphQLObjectTypeDefinition
                        {
                            Name = new GraphQLName("SomeObjectType")
                        },
                        new GraphQLObjectTypeDefinition
                        {
                            Name = new GraphQLName(GraphQLToken.Query)
                        }
                    }
                }),
                new List<GraphQLObjectTypeDefinition>
                {
                    new()
                    {
                        Name = new GraphQLName("SomeObjectType")
                    }
                }
            ).SetName("GraphQL document with one object type definitions generates expected result");
        }
    }

    [Test]
    [TestCaseSource(nameof(GraphQLInterfaceTypeDefinitionsTestCases))]
    public void GraphQLInterfaceTypeDefinitions_returns_expected_result(
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        IEnumerable<GraphQLInterfaceTypeDefinition> expectedResult
    ) => graphQLDocumentAdapter
        .GraphQLInterfaceTypeDefinitions
        .Should()
        .BeEquivalentTo(expectedResult);

    private static IEnumerable<TestCaseData> GraphQLInterfaceTypeDefinitionsTestCases
    {
        get
        {
            yield return new TestCaseData(
                new GraphQLDocumentAdapter(new GraphQLDocument()),
                new List<GraphQLInterfaceTypeDefinition>()
            ).SetName("GraphQL document with no definitions generates expected result");

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(new GraphQLDocument
                {
                    Definitions = new List<ASTNode>
                    {
                        new GraphQLObjectTypeDefinition
                        {
                            Name = new GraphQLName("SomeObjectType")
                        }
                    }
                }),
                new List<GraphQLInterfaceTypeDefinition>()
            ).SetName("GraphQL document with no interface type definitions generates expected result");

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(new GraphQLDocument
                {
                    Definitions = new List<ASTNode>
                    {
                        new GraphQLInterfaceTypeDefinition
                        {
                            Name = new GraphQLName("SomeInterfaceType")
                        },
                        new GraphQLObjectTypeDefinition
                        {
                            Name = new GraphQLName("SomeObjectType")
                        },
                        new GraphQLObjectTypeDefinition
                        {
                            Name = new GraphQLName(GraphQLToken.Query)
                        }
                    }
                }),
                new List<GraphQLInterfaceTypeDefinition>
                {
                    new()
                    {
                        Name = new GraphQLName("SomeInterfaceType")
                    }
                }
            ).SetName("GraphQL document with one interface type definition generates expected result");
        }
    }

    [Test]
    [TestCaseSource(nameof(GraphQLQueryTypeDefinitionTestCases))]
    public void GraphQLQueryTypeDefinition_returns_expected_result(
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        GraphQLObjectTypeDefinition? expectedResult
    ) => graphQLDocumentAdapter
        .GraphQLQueryTypeDefinition
        .Should()
        .BeEquivalentTo(expectedResult);

    private static IEnumerable<TestCaseData> GraphQLQueryTypeDefinitionTestCases
    {
        get
        {
            yield return new TestCaseData(
                new GraphQLDocumentAdapter(new GraphQLDocument()),
                null
            ).SetName("GraphQL document with no definitions returns expected result.");

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(new GraphQLDocument
                {
                    Definitions = new List<ASTNode>
                    {
                        new GraphQLInterfaceTypeDefinition
                        {
                            Name = new GraphQLName("SomeInterfaceType")
                        }
                    }
                }),
                null
            ).SetName("GraphQL document with no query definition returns expected result.");

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(new GraphQLDocument
                {
                    Definitions = new List<ASTNode>
                    {
                        new GraphQLObjectTypeDefinition
                        {
                            Name = new GraphQLName(GraphQLToken.Query)
                        }
                    }
                }),
                new GraphQLObjectTypeDefinition
                {
                    Name = new GraphQLName(GraphQLToken.Query)
                }
            ).SetName("GraphQL document with query definition returns expected result.");
        }
    }

    [Test]
    [TestCaseSource(nameof(GraphQLMutationTypeDefinitionTestCases))]
    public void GraphQLMutationTypeDefinition_returns_expected_result(
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        GraphQLObjectTypeDefinition? expectedResult
    ) => graphQLDocumentAdapter
        .GraphQLMutationTypeDefinition
        .Should()
        .BeEquivalentTo(expectedResult);

    private static IEnumerable<TestCaseData> GraphQLMutationTypeDefinitionTestCases
    {
        get
        {
            yield return new TestCaseData(
                new GraphQLDocumentAdapter(new GraphQLDocument()),
                null
            ).SetName("GraphQL document with no definitions returns expected result.");

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(new GraphQLDocument
                {
                    Definitions = new List<ASTNode>
                    {
                        new GraphQLInterfaceTypeDefinition
                        {
                            Name = new GraphQLName("SomeInterfaceType")
                        }
                    }
                }),
                null
            ).SetName("GraphQL document with no mutation definition returns expected result.");

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(new GraphQLDocument
                {
                    Definitions = new List<ASTNode>
                    {
                        new GraphQLObjectTypeDefinition
                        {
                            Name = new GraphQLName(GraphQLToken.Mutation)
                        }
                    }
                }),
                new GraphQLObjectTypeDefinition
                {
                    Name = new GraphQLName(GraphQLToken.Mutation)
                }
            ).SetName("GraphQL document with mutation definition returns expected result.");
        }
    }
}