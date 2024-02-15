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
                new GraphQLDocumentAdapter(new GraphQLDocument([])),
                enumTypeDefinitionName,
                false
            ).SetName("When GraphQL document is empty, enum type definition is not found");

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(
                    new GraphQLDocument([new GraphQLObjectTypeDefinition(new GraphQLName("Goodbye"))])
                ),
                enumTypeDefinitionName,
                false
            ).SetName(
                "When GraphQL document is not empty but doesn't have enum type definitions, enum type definition is not found"
            );

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(
                    new GraphQLDocument([new GraphQLEnumTypeDefinition(new GraphQLName("Hello"))])
                ),
                enumTypeDefinitionName,
                false
            ).SetName(
                "When GraphQL document is not empty but doesn't have specific enum type definition, enum type definition is not found"
            );

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(
                    new GraphQLDocument([new GraphQLEnumTypeDefinition(new GraphQLName(enumTypeDefinitionName))]
                    )
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
                new GraphQLDocumentAdapter(new GraphQLDocument([])),
                hasFieldsTypeDefinitionName,
                false
            ).SetName("When GraphQL document is empty, has fields type definition is not found");

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(
                    new GraphQLDocument([new GraphQLEnumTypeDefinition(new GraphQLName("Goodbye"))])
                ),
                hasFieldsTypeDefinitionName,
                false
            ).SetName(
                "When GraphQL document is not empty but doesn't have has fields type definitions, has fields type definition is not found"
            );

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(
                    new GraphQLDocument([new GraphQLObjectTypeDefinition(new GraphQLName("Hello"))])
                ),
                hasFieldsTypeDefinitionName,
                false
            ).SetName(
                "When GraphQL document is not empty but doesn't have specific has fields type definition, has fields type definition is not found"
            );

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(
                    new GraphQLDocument([new GraphQLObjectTypeDefinition(new GraphQLName(hasFieldsTypeDefinitionName))])
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
                new GraphQLDocumentAdapter(new GraphQLDocument([])),
                unionTypeDefinitionName,
                false
            ).SetName("When GraphQL document is empty, union type definition is not found");

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(
                    new GraphQLDocument([new GraphQLEnumTypeDefinition(new GraphQLName("Goodbye"))])
                ),
                unionTypeDefinitionName,
                false
            ).SetName(
                "When GraphQL document is not empty but doesn't have union type definition, union type definition is not found"
            );

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(
                    new GraphQLDocument([new GraphQLUnionTypeDefinition(new GraphQLName("Hello"))])
                ),
                unionTypeDefinitionName,
                false
            ).SetName(
                "When GraphQL document is not empty but doesn't have specific union type definition, union type definition is not found"
            );

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(
                    new GraphQLDocument([new GraphQLUnionTypeDefinition(new GraphQLName(unionTypeDefinitionName))])
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
                new GraphQLDocumentAdapter(new GraphQLDocument([])),
                "test",
                false
            ).SetName("When GraphQL document is empty, input object type definition is not found");

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(
                    new GraphQLDocument([new GraphQLEnumTypeDefinition(new GraphQLName("Goodbye"))])
                ),
                "test",
                false
            ).SetName(
                "When GraphQL document is not empty but doesn't have input object type definition, input object type definition is not found"
            );

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(
                    new GraphQLDocument([new GraphQLInputObjectTypeDefinition(new GraphQLName("Hello"))])
                ),
                "test",
                false
            ).SetName(
                "When GraphQL document is not empty but doesn't have specific input object type definition, input object type definition is not found"
            );

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(
                    new GraphQLDocument(
                        [new GraphQLInputObjectTypeDefinition(new GraphQLName("test"))]
                    )
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
        var graphQLTypeDefinitionWithFields =
            graphQLDocumentAdapter.GetGraphQLTypeDefinitionWithFields(graphQLTypeDefinitionName);

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

            var mainField = new GraphQLFieldDefinition(
                new GraphQLName("mainField"),
                new GraphQLNamedType(new GraphQLName("String"))
            );

            var inaccessibleField = new GraphQLFieldDefinition(
                new GraphQLName("inaccessibleField"),
                new GraphQLNamedType(new GraphQLName("String"))
            )
            {
                Directives = new GraphQLDirectives([new GraphQLDirective(new GraphQLName("inaccessible"))])
            };

            var externalField = new GraphQLFieldDefinition(
                new GraphQLName("externalField"),
                new GraphQLNamedType(new GraphQLName("String"))
            )
            {
                Directives = new GraphQLDirectives([new GraphQLDirective(new GraphQLName("external"))])
            };

            var extendedField = new GraphQLFieldDefinition(
                new GraphQLName("extendedField"),
                new GraphQLNamedType(new GraphQLName("String"))
            );

            var graphQLObjectTypeDefinition = new GraphQLObjectTypeDefinition(
                new GraphQLName(graphQLObjectTypeDefinitionName)
            )
            {
                Fields = new GraphQLFieldsDefinition([
                    mainField,
                    inaccessibleField,
                    externalField
                ])
            };

            var graphQLInterfaceTypeDefinition = new GraphQLInterfaceTypeDefinition(
                new GraphQLName(graphQLInterfaceTypeDefinitionName))
            {
                Fields = new GraphQLFieldsDefinition([
                    mainField,
                    inaccessibleField,
                    externalField
                ])
            };

            var otherGraphQLObjectTypeExtension = new GraphQLObjectTypeExtension(
                new GraphQLName("fakeType")
            )
            {
                Fields = new GraphQLFieldsDefinition(
                    [
                        new GraphQLFieldDefinition(
                            new GraphQLName("huh"),
                            new GraphQLNamedType(new GraphQLName("String"))
                        )
                    ]
                )
            };

            var graphQLObjectTypeExtension =
                new GraphQLObjectTypeExtension(new GraphQLName(graphQLObjectTypeDefinitionName))
                {
                    Fields = new GraphQLFieldsDefinition([
                        extendedField,
                        inaccessibleField,
                        externalField
                    ])
                };

            var graphQLInterfaceTypeExtension = new GraphQLInterfaceTypeExtension(graphQLInterfaceTypeDefinition.Name)
            {
                Fields = new GraphQLFieldsDefinition([
                    extendedField,
                    inaccessibleField,
                    externalField
                ])
            };

            var graphQLDocumentAdapter = new GraphQLDocumentAdapter(
                new GraphQLDocument([
                    graphQLObjectTypeDefinition,
                    graphQLObjectTypeExtension,
                    otherGraphQLObjectTypeExtension,
                    graphQLInterfaceTypeExtension,
                    graphQLInterfaceTypeDefinition
                ])
            );

            var expectedGraphQLObjectTypeDefinition = new GraphQLObjectTypeDefinition(graphQLObjectTypeDefinition.Name)
            {
                Fields = new GraphQLFieldsDefinition([
                    mainField,
                    extendedField
                ])
            };

            var expectedGraphQLInterfaceTypeDefinition =
                new GraphQLObjectTypeDefinition(graphQLInterfaceTypeDefinition.Name)
                {
                    Fields = new GraphQLFieldsDefinition([
                        mainField,
                        extendedField
                    ])
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

            var mainType = new GraphQLNamedType(new GraphQLName("mainType"));
            var secondType = new GraphQLNamedType(new GraphQLName("secondType"));
            var thirdType = new GraphQLNamedType(new GraphQLName("thirdType"));

            var graphQLUnionTypeDefinition = new GraphQLUnionTypeDefinition(new GraphQLName(unionTypeDefinitionName))
            {
                Types = new GraphQLUnionMemberTypes([mainType, secondType])
            };

            var graphQLUnionTypeExtension = new GraphQLUnionTypeExtension(graphQLUnionTypeDefinition.Name)
            {
                Types = new GraphQLUnionMemberTypes([thirdType])
            };

            var otherGraphQLUnionTypeExtension = new GraphQLUnionTypeExtension(new GraphQLName("somethingElse"))
            {
                Types = new GraphQLUnionMemberTypes([new GraphQLNamedType(new GraphQLName("otherType"))])
            };

            var graphQLDocumentAdapter = new GraphQLDocumentAdapter(
                new GraphQLDocument([
                    graphQLUnionTypeDefinition,
                    graphQLUnionTypeExtension,
                    otherGraphQLUnionTypeExtension
                ])
            );

            var expectedGraphQLUnionTypeDefinition =
                new GraphQLUnionTypeDefinition(new GraphQLName(unionTypeDefinitionName))
                {
                    Types = new GraphQLUnionMemberTypes([mainType, secondType, thirdType])
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
                new GraphQLDocumentAdapter(new GraphQLDocument([])),
                "test",
                null
            ).SetName("When GraphQL document is empty, enum type definition is not found");

            const string graphQLEnumTypeDefinitionName = "Color";

            var redEnumValueDefinition = new GraphQLEnumValueDefinition(
                new GraphQLName("RED"),
                new GraphQLEnumValue(new GraphQLName("RED"))
            );

            var blueEnumValueDefinition = new GraphQLEnumValueDefinition(
                new GraphQLName("BLUE"),
                new GraphQLEnumValue(new GraphQLName("BLUE"))
            );

            var greenEnumValueDefinition = new GraphQLEnumValueDefinition(
                new GraphQLName("GREEN"),
                new GraphQLEnumValue(new GraphQLName("GREEN"))
            );

            var yellowEnumValueDefinition = new GraphQLEnumValueDefinition(
                new GraphQLName("YELLOW"),
                new GraphQLEnumValue(new GraphQLName("YELLOW"))
            );

            var graphQLEnumTypeDefinition =
                new GraphQLEnumTypeDefinition(new GraphQLName(graphQLEnumTypeDefinitionName))
                {
                    Values = new GraphQLEnumValuesDefinition(
                        [
                            redEnumValueDefinition,
                            blueEnumValueDefinition
                        ]
                    )
                };

            var graphQLEnumTypeExtension = new GraphQLEnumTypeExtension(graphQLEnumTypeDefinition.Name)
            {
                Values = new GraphQLEnumValuesDefinition(
                    [
                        greenEnumValueDefinition,
                        yellowEnumValueDefinition
                    ]
                )
            };

            var otherGraphQLEnumTypeExtension = new GraphQLEnumTypeExtension(new GraphQLName("somethingElse"))
            {
                Values = new GraphQLEnumValuesDefinition(
                    [
                        new GraphQLEnumValueDefinition(
                            new GraphQLName("SMALL"),
                            new GraphQLEnumValue(new GraphQLName("SMALL"))
                        ),

                        new GraphQLEnumValueDefinition(
                            new GraphQLName("LARGE"),
                            new GraphQLEnumValue(new GraphQLName("LARGE"))
                        )
                    ]
                )
            };

            var graphQLDocumentAdapter = new GraphQLDocumentAdapter(
                new GraphQLDocument([
                    graphQLEnumTypeDefinition,
                    graphQLEnumTypeExtension,
                    otherGraphQLEnumTypeExtension
                ])
            );

            var expectedGraphQLEnumTypeDefinition =
                new GraphQLEnumTypeDefinition(new GraphQLName(graphQLEnumTypeDefinitionName))
                {
                    Values = new GraphQLEnumValuesDefinition([
                        redEnumValueDefinition, blueEnumValueDefinition, greenEnumValueDefinition,
                        yellowEnumValueDefinition
                    ])
                };

            yield return new TestCaseData(
                graphQLDocumentAdapter,
                graphQLEnumTypeDefinition.NameValue(),
                expectedGraphQLEnumTypeDefinition
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
                new GraphQLDocumentAdapter(new GraphQLDocument([])),
                "test",
                null
            ).SetName("When GraphQL document is empty, input object type definition is not found");

            const string inputObjectTypeDefinitionName = "Hello";

            var mainField = new GraphQLInputValueDefinition(
                new GraphQLName("mainField"),
                new GraphQLNamedType(new GraphQLName("String"))
            );

            var otherField = new GraphQLInputValueDefinition(
                new GraphQLName("otherField"),
                new GraphQLNamedType(new GraphQLName("String"))
            );

            var inaccessibleField = new GraphQLInputValueDefinition(
                new GraphQLName("inaccessibleField"),
                new GraphQLNamedType(new GraphQLName("String"))
            )
            {
                Directives = new GraphQLDirectives([new GraphQLDirective(new GraphQLName("inaccessible"))])
            };

            var externalField = new GraphQLInputValueDefinition(
                new GraphQLName("externalField"),
                new GraphQLNamedType(new GraphQLName("String"))
            )
            {
                Directives = new GraphQLDirectives([new GraphQLDirective(new GraphQLName("external"))])
            };

            var graphQLInputObjectTypeDefinition = new GraphQLInputObjectTypeDefinition(
                new GraphQLName(inputObjectTypeDefinitionName))
            {
                Fields = new GraphQLInputFieldsDefinition(
                    [
                        mainField,
                        inaccessibleField
                    ]
                )
            };

            var graphQLInputObjectTypeExtension =
                new GraphQLInputObjectTypeExtension(graphQLInputObjectTypeDefinition.Name)
                {
                    Fields = new GraphQLInputFieldsDefinition([
                        otherField,
                        externalField
                    ])
                };

            var otherGraphQLInputObjectTypeExtension = new GraphQLInputObjectTypeExtension(
                new GraphQLName("somethingElse"))
            {
                Fields = new GraphQLInputFieldsDefinition([
                    new GraphQLInputValueDefinition(
                        new GraphQLName("yetAnotherField"),
                        new GraphQLNamedType(new GraphQLName("String"))
                    )
                ])
            };

            var graphQLDocumentAdapter = new GraphQLDocumentAdapter(
                new GraphQLDocument([
                    graphQLInputObjectTypeDefinition,
                    graphQLInputObjectTypeExtension,
                    otherGraphQLInputObjectTypeExtension
                ])
            );

            var expectedGraphQLInputObjectTypeDefinition = new GraphQLInputObjectTypeDefinition(
                new GraphQLName(inputObjectTypeDefinitionName))
            {
                Fields = new GraphQLInputFieldsDefinition([
                    mainField,
                    otherField
                ])
            };

            yield return new TestCaseData(
                graphQLDocumentAdapter,
                "test",
                null
            ).SetName(
                "When GraphQL document is not empty but doesn't have specific input object type definition, input object type definition is not found"
            );

            yield return new TestCaseData(
                graphQLDocumentAdapter,
                inputObjectTypeDefinitionName,
                expectedGraphQLInputObjectTypeDefinition
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
            const string objectTypeDefinitionName = "SomeObjectType";
            const string interfaceTypeDefinitionName = "SomeInterfaceType";
            const string queryName = GraphQLToken.Query;

            var graphQLObjectTypeDefinition = new GraphQLObjectTypeDefinition(new GraphQLName(objectTypeDefinitionName))
            {
                Fields = new GraphQLFieldsDefinition([])
            };

            var graphQLInterfaceTypeDefinition = new GraphQLInterfaceTypeDefinition(
                new GraphQLName(interfaceTypeDefinitionName)
            );

            var graphQLDocumentAdapter1 = new GraphQLDocumentAdapter(
                new GraphQLDocument([graphQLInterfaceTypeDefinition])
            );

            var graphQLDocumentAdapter2 = new GraphQLDocumentAdapter(
                new GraphQLDocument([
                    graphQLInterfaceTypeDefinition,
                    graphQLObjectTypeDefinition,
                    new GraphQLObjectTypeDefinition(new GraphQLName(queryName))
                ])
            );

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(new GraphQLDocument([])),
                new List<GraphQLObjectTypeDefinition>()
            ).SetName("GraphQL document with no definitions generates expected result");

            yield return new TestCaseData(
                graphQLDocumentAdapter1,
                new List<GraphQLObjectTypeDefinition>()
            ).SetName("GraphQL document with no object type definitions generates expected result");

            yield return new TestCaseData(
                graphQLDocumentAdapter2,
                new List<GraphQLObjectTypeDefinition>
                {
                    new(new GraphQLName(objectTypeDefinitionName))
                    {
                        Fields = new GraphQLFieldsDefinition([])
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
            const string interfaceTypeName = "SomeInterfaceType";
            const string objectTypeName = "SomeObjectType";
            const string queryTypeName = GraphQLToken.Query;

            var interfaceTypeDefinition = new GraphQLInterfaceTypeDefinition(new GraphQLName(interfaceTypeName));

            var objectTypeDefinition = new GraphQLObjectTypeDefinition(new GraphQLName(objectTypeName));

            var queryObjectTypeDefinition = new GraphQLObjectTypeDefinition(new GraphQLName(queryTypeName));

            var graphQLDocumentAdapter1 =
                new GraphQLDocumentAdapter(new GraphQLDocument([objectTypeDefinition]));

            var graphQLDocumentAdapter2 = new GraphQLDocumentAdapter(new GraphQLDocument([interfaceTypeDefinition, objectTypeDefinition, queryObjectTypeDefinition]));

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(new GraphQLDocument([])),
                new List<GraphQLInterfaceTypeDefinition>()
            ).SetName("GraphQL document with no definitions generates expected result");

            yield return new TestCaseData(
                graphQLDocumentAdapter1,
                new List<GraphQLInterfaceTypeDefinition>()
            ).SetName("GraphQL document with no interface type definitions generates expected result");

            yield return new TestCaseData(
                graphQLDocumentAdapter2,
                new List<GraphQLInterfaceTypeDefinition>
                {
                    new(new GraphQLName(interfaceTypeName))
                    {
                        Fields = new GraphQLFieldsDefinition([])
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
                new GraphQLDocumentAdapter(new GraphQLDocument([])),
                null
            ).SetName("GraphQL document with no definitions returns expected result.");

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(new GraphQLDocument(
                        [new GraphQLInterfaceTypeDefinition(new GraphQLName("SomeInterfaceType"))]
                    )
                ),
                null
            ).SetName("GraphQL document with no query definition returns expected result.");

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(new GraphQLDocument([new GraphQLObjectTypeDefinition(new GraphQLName(GraphQLToken.Query))])),
                new GraphQLObjectTypeDefinition(new GraphQLName(GraphQLToken.Query))
                {
                    Fields = new GraphQLFieldsDefinition([])
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
                new GraphQLDocumentAdapter(new GraphQLDocument([])),
                null
            ).SetName("GraphQL document with no definitions returns expected result.");

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(new GraphQLDocument([new GraphQLInterfaceTypeDefinition(new GraphQLName("SomeInterfaceType"))])),
                null
            ).SetName("GraphQL document with no mutation definition returns expected result.");

            yield return new TestCaseData(
                new GraphQLDocumentAdapter(new GraphQLDocument([new GraphQLObjectTypeDefinition(new GraphQLName(GraphQLToken.Mutation))])),
                new GraphQLObjectTypeDefinition(new GraphQLName(GraphQLToken.Mutation))
                {
                    Fields = new GraphQLFieldsDefinition([])
                }
            ).SetName("GraphQL document with mutation definition returns expected result.");
        }
    }
}