using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Exceptions;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;
using GraphQLToKarate.Tests.Mocks;
using NSubstitute;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Converters;

[TestFixture]
internal sealed class GraphQLListTypeConverterTests
{
    private IGraphQLTypeConverter? _subjectUnderTest;
    private IGraphQLDocumentAdapter? _mockGraphQLDocumentAdapter;

    [SetUp]
    public void SetUp()
    {
        _mockGraphQLDocumentAdapter = Substitute.For<IGraphQLDocumentAdapter>();

        var graphQLTypeConverterFactory = new GraphQLTypeConverterFactory(new GraphQLTypeConverter());

        _subjectUnderTest = new GraphQLListTypeConverter(graphQLTypeConverterFactory);
    }

    [Test]
    [TestCaseSource(nameof(TestCases))]
    public void Convert(
        string graphQLFieldName,
        GraphQLType graphQLType,
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        KarateTypeBase expectedKarateType)
    {
        // act
        var karateType = _subjectUnderTest!.Convert(
            graphQLFieldName,
            graphQLType,
            graphQLDocumentAdapter
        );

        // assert
        karateType.Should().BeEquivalentTo(expectedKarateType);
    }

    private static IEnumerable<TestCaseData> TestCases
    {
        get
        {
            const string testFieldName = "Test";

            var emptyGraphQLDocumentAdapter = new GraphQLDocumentAdapter(new GraphQLDocument());

            yield return new TestCaseData(
                testFieldName,
                new GraphQLListType
                {
                    Type = new GraphQLNamedType
                    {
                        Name = new GraphQLName(GraphQLToken.Boolean)
                    }
                },
                emptyGraphQLDocumentAdapter,
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
                    }
                },
                emptyGraphQLDocumentAdapter,
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
                emptyGraphQLDocumentAdapter,
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
                emptyGraphQLDocumentAdapter,
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
                emptyGraphQLDocumentAdapter,
                new KarateListType(
                    new KarateNullType(
                        new KarateType(KarateToken.String, testFieldName)
                    )
                )
            ).SetName("List of nullable ID GraphQL type is converted to list of nullable string Karate type.");

            const string enumTypeName = "Color";

            var graphQLDocumentWithEnumTypeDefinition = new GraphQLDocument
            {
                Definitions = new List<ASTNode>
                {
                    new GraphQLEnumTypeDefinition
                    {
                        Name = new GraphQLName(enumTypeName)
                    }
                }
            };

            yield return new TestCaseData(
                testFieldName,
                new GraphQLListType
                {
                    Type = new GraphQLNamedType
                    {
                        Name = new GraphQLName(enumTypeName)
                    }
                },
                new GraphQLDocumentAdapter(graphQLDocumentWithEnumTypeDefinition),
                new KarateListType(
                    new KarateNullType(
                        new KarateType(KarateToken.String, testFieldName)
                    )
                )
            ).SetName("List of nullable enum GraphQL type is converted to list of nullable string Karate type.");

            const string customTypeName = "ToDo";

            var graphQLDocumentWithEnumAndCustomTypeDefinition = new GraphQLDocument
            {
                Definitions = new List<ASTNode>
                {
                    new GraphQLEnumTypeDefinition
                    {
                        Name = new GraphQLName(enumTypeName)
                    },
                    new GraphQLObjectTypeDefinition
                    {
                        Name = new GraphQLName(customTypeName)
                    }
                }
            };

            yield return new TestCaseData(
                testFieldName,
                new GraphQLListType
                {
                    Type = new GraphQLNamedType
                    {
                        Name = new GraphQLName(customTypeName)
                    }
                },
                new GraphQLDocumentAdapter(graphQLDocumentWithEnumAndCustomTypeDefinition),
                new KarateListType(
                    new KarateNullType(
                        new KarateType($"({customTypeName.FirstCharToLower()}Schema)", testFieldName)
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
                emptyGraphQLDocumentAdapter,
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
                emptyGraphQLDocumentAdapter,
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
                emptyGraphQLDocumentAdapter,
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
                emptyGraphQLDocumentAdapter,
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
                emptyGraphQLDocumentAdapter,
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
                new GraphQLDocumentAdapter(graphQLDocumentWithEnumTypeDefinition),
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
                new GraphQLDocumentAdapter(graphQLDocumentWithEnumAndCustomTypeDefinition),
                new KarateListType(
                    new KarateNonNullType(
                        new KarateType($"({customTypeName.FirstCharToLower()}Schema)", testFieldName)
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
                emptyGraphQLDocumentAdapter,
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

    [Test]
    public void Convert_throws_exception_when_unsupported_graphql_type_is_encountered()
    {
        // arrange
        var unsupportedGraphQLType = new UnsupportedGraphQLType();

        var graphQLListType = new GraphQLListType
        {
            Type = unsupportedGraphQLType
        };

        // act
        var act = () => _subjectUnderTest!.Convert(
            "unsupported",
            graphQLListType,
            _mockGraphQLDocumentAdapter!
        );

        // assert
        act.Should().ThrowExactly<InvalidGraphQLTypeException>();
    }
}