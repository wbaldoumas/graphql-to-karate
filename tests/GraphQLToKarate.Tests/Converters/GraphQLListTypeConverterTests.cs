using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Converters;

[TestFixture]
internal sealed class GraphQLListTypeConverterTests
{
    private IGraphQLTypeConverter? _subjectUnderTest;

    [SetUp]
    public void SetUp()
    {
        var graphQLTypeConverterFactory = new GraphQLTypeConverterFactory();

        _subjectUnderTest = new GraphQLListTypeConverter(graphQLTypeConverterFactory);
    }

    [Test]
    [TestCaseSource(nameof(TestCases))]
    public void Convert(
        string graphQLFieldName,
        GraphQLType graphQLType,
        GraphQLUserDefinedTypes graphQLUserDefinedTypes,
        KarateTypeBase expectedKarateType)
    {
        // act
        var karateType = _subjectUnderTest!.Convert(
            graphQLFieldName,
            graphQLType,
            graphQLUserDefinedTypes
        );

        // assert
        karateType.Should().BeEquivalentTo(expectedKarateType);
    }

    private static IEnumerable<TestCaseData> TestCases
    {
        get
        {
            const string testFieldName = "Test";

            var emptyGraphQLUserDefinedTypes = new GraphQLUserDefinedTypes
            {
                GraphQLEnumTypeDefinitionsByName = new Dictionary<string, GraphQLEnumTypeDefinition>(),
                GraphQLObjectTypeDefinitionsByName = new Dictionary<string, GraphQLObjectTypeDefinition>()
            };

            yield return new TestCaseData(
                testFieldName,
                new GraphQLListType
                {
                    Type = new GraphQLNamedType
                    {
                        Name = new GraphQLName(GraphQLToken.Boolean)
                    }
                },
                emptyGraphQLUserDefinedTypes,
                new KarateListType(
                    new KarateNullType(
                        new KarateType(KarateToken.Boolean, testFieldName)
                    )
                )
            ).SetName("List of nullable Boolean GraphQL type is converted to list of nullable boolean Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLListType
                {
                    Type = new GraphQLNamedType
                    {
                        Name = new GraphQLName(GraphQLToken.Float)
                    },
                },
                emptyGraphQLUserDefinedTypes,
                new KarateListType(
                    new KarateNullType(
                        new KarateType(KarateToken.Number, testFieldName)
                    )
                )
            ).SetName("List of nullable Float GraphQL type is converted to list of nullable number Karate type.");


            yield return new TestCaseData(
                testFieldName,
                new GraphQLListType
                {
                    Type = new GraphQLNamedType
                    {
                        Name = new GraphQLName(GraphQLToken.Int)
                    }
                },
                emptyGraphQLUserDefinedTypes,
                new KarateListType(
                    new KarateNullType(
                        new KarateType(KarateToken.Number, testFieldName)
                    )
                )
            ).SetName("List of nullable Int GraphQL type is converted to list of nullable number Karate type.");


            yield return new TestCaseData(
                testFieldName,
                new GraphQLListType
                {
                    Type = new GraphQLNamedType
                    {
                        Name = new GraphQLName(GraphQLToken.String)
                    }
                },
                emptyGraphQLUserDefinedTypes,
                new KarateListType(
                    new KarateNullType(
                        new KarateType(KarateToken.String, testFieldName)
                    )
                )
            ).SetName("List of nullable String GraphQL type is converted to list of nullable string Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLListType
                {
                    Type = new GraphQLNamedType
                    {
                        Name = new GraphQLName(GraphQLToken.Id)
                    }
                },
                emptyGraphQLUserDefinedTypes,
                new KarateListType(
                    new KarateNullType(
                        new KarateType(KarateToken.String, testFieldName)
                    )
                )
            ).SetName("List of nullable ID GraphQL type is converted to list of nullable string Karate type.");

            const string enumTypeName = "Color";

            yield return new TestCaseData(
                testFieldName,
                new GraphQLListType
                {
                    Type = new GraphQLNamedType
                    {
                        Name = new GraphQLName(enumTypeName)
                    }
                },
                new GraphQLUserDefinedTypes
                {
                    GraphQLEnumTypeDefinitionsByName = new Dictionary<string, GraphQLEnumTypeDefinition>
                    {
                        { enumTypeName, new GraphQLEnumTypeDefinition() }
                    },
                    GraphQLObjectTypeDefinitionsByName = new Dictionary<string, GraphQLObjectTypeDefinition>()
                },
                new KarateListType(
                    new KarateNullType(
                        new KarateType(KarateToken.String, testFieldName)
                    )
                )
            ).SetName("List of nullable enum GraphQL type is converted to list of nullable string Karate type.");

            const string customTypeName = "ToDo";

            yield return new TestCaseData(
                testFieldName,
                new GraphQLListType
                {
                    Type = new GraphQLNamedType
                    {
                        Name = new GraphQLName(customTypeName)
                    },
                },
                new GraphQLUserDefinedTypes
                {
                    GraphQLEnumTypeDefinitionsByName = new Dictionary<string, GraphQLEnumTypeDefinition>
                    {
                        { enumTypeName, new GraphQLEnumTypeDefinition() }
                    },
                    GraphQLObjectTypeDefinitionsByName = new Dictionary<string, GraphQLObjectTypeDefinition>
                    {
                        { customTypeName, new GraphQLObjectTypeDefinition() }
                    }
                },
                new KarateListType(
                    new KarateNullType(
                        new KarateType($"{customTypeName.FirstCharToLower()}Schema", testFieldName)
                    )
                )
            ).SetName("List of nullable custom GraphQL type is converted to list of nullable custom Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLListType
                {
                    Type = new GraphQLNonNullType
                    {
                        Type = new GraphQLNamedType
                        {
                            Name = new GraphQLName(GraphQLToken.Boolean)
                        }
                    }
                },
                emptyGraphQLUserDefinedTypes,
                new KarateListType(
                    new KarateNonNullType(
                        new KarateType(KarateToken.Boolean, testFieldName)
                    )
                )
            ).SetName("List of non-nullable Boolean GraphQL type is converted to list of non-nullable boolean Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLListType
                {
                    Type = new GraphQLNonNullType
                    {
                        Type = new GraphQLNamedType
                        {
                            Name = new GraphQLName(GraphQLToken.Float)
                        }
                    }
                },
                emptyGraphQLUserDefinedTypes,
                new KarateListType(
                    new KarateNonNullType(
                        new KarateType(KarateToken.Number, testFieldName)
                    )
                )
            ).SetName("List of non-nullable Float GraphQL type is converted to list of non-nullable number Karate type.");


            yield return new TestCaseData(
                testFieldName,
                new GraphQLListType
                {
                    Type = new GraphQLNonNullType
                    {
                        Type = new GraphQLNamedType
                        {
                            Name = new GraphQLName(GraphQLToken.Int)
                        }
                    }
                }, 
                emptyGraphQLUserDefinedTypes,
                new KarateListType(
                    new KarateNonNullType(
                        new KarateType(KarateToken.Number, testFieldName)
                    )
                )
            ).SetName("List of non-nullable Int GraphQL type is converted to list of non-nullable number Karate type.");


            yield return new TestCaseData(
                testFieldName,
                new GraphQLListType
                {
                    Type = new GraphQLNonNullType
                    {
                        Type = new GraphQLNamedType
                        {
                            Name = new GraphQLName(GraphQLToken.String)
                        }
                    }
                },
                emptyGraphQLUserDefinedTypes,
                new KarateListType(
                    new KarateNonNullType(
                        new KarateType(KarateToken.String, testFieldName)
                    )
                )
            ).SetName("List of non-nullable String GraphQL type is converted to list of non-nullable string Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLListType
                {
                    Type = new GraphQLNonNullType
                    {
                        Type = new GraphQLNamedType
                        {
                            Name = new GraphQLName(GraphQLToken.Id)
                        }
                    }
                },
                emptyGraphQLUserDefinedTypes,
                new KarateListType(
                    new KarateNonNullType(
                        new KarateType(KarateToken.String, testFieldName)
                    )
                )
            ).SetName("List of non-nullable ID GraphQL type is converted to list of non-nullable string Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLListType
                {
                    Type = new GraphQLNonNullType
                    {
                        Type = new GraphQLNamedType
                        {
                            Name = new GraphQLName(enumTypeName)
                        }
                    }
                },
                new GraphQLUserDefinedTypes
                {
                    GraphQLEnumTypeDefinitionsByName = new Dictionary<string, GraphQLEnumTypeDefinition>
                    {
                        { enumTypeName, new GraphQLEnumTypeDefinition() }
                    },
                    GraphQLObjectTypeDefinitionsByName = new Dictionary<string, GraphQLObjectTypeDefinition>()
                },
                new KarateListType(
                    new KarateNonNullType(
                        new KarateType(KarateToken.String, testFieldName)
                    )
                )
            ).SetName("List of non-nullable enum GraphQL type is converted to list of non-nullable string Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLListType
                {
                    Type = new GraphQLNonNullType
                    {
                        Type = new GraphQLNamedType
                        {
                            Name = new GraphQLName(customTypeName)
                        }
                    }
                },
                new GraphQLUserDefinedTypes
                {
                    GraphQLEnumTypeDefinitionsByName = new Dictionary<string, GraphQLEnumTypeDefinition>
                    {
                        { enumTypeName, new GraphQLEnumTypeDefinition() }
                    },
                    GraphQLObjectTypeDefinitionsByName = new Dictionary<string, GraphQLObjectTypeDefinition>
                    {
                        { customTypeName, new GraphQLObjectTypeDefinition() }
                    }
                },
                new KarateListType(
                    new KarateNonNullType(
                        new KarateType($"{customTypeName.FirstCharToLower()}Schema", testFieldName)
                    )
                )
            ).SetName("List of non-nullable custom GraphQL type is converted to list of non-nullable custom Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLListType
                {
                    Type = new GraphQLListType
                    {
                        Type = new GraphQLNonNullType
                        {
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName(GraphQLToken.Id)
                            }
                        }
                    }
                },
                emptyGraphQLUserDefinedTypes,
                new KarateListType(
                    new KarateNullType(
                        new KarateListType(
                            new KarateNonNullType(
                                new KarateType(KarateToken.String, testFieldName)
                            )
                        )
                    )
                )
            ).SetName("List of nullable lists of non-nullable ID GraphQL type is converted to list of nullable lists of non-nullable string Karate type.");
        }
    }
}