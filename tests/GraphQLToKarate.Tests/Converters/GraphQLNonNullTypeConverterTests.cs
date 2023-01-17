using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Converters;

[TestFixture]
internal sealed class GraphQLNonNullTypeConverterTests
{
    private IGraphQLTypeConverter? _subjectUnderTest;

    [SetUp]
    public void SetUp()
    {
        var graphQLTypeConverterFactory = new GraphQLTypeConverterFactory();

        _subjectUnderTest = new GraphQLNonNullTypeConverter(graphQLTypeConverterFactory);
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
                GraphQLObjectTypeDefinitionsByName = new Dictionary<string, GraphQLObjectTypeDefinition>(),
                GraphQLInterfaceTypeDefinitionsByName = new Dictionary<string, GraphQLInterfaceTypeDefinition>()
            };

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNonNullType
                {
                    Type = new GraphQLNamedType
                    {
                        Name = new GraphQLName(GraphQLToken.Boolean)
                    }
                },
                emptyGraphQLUserDefinedTypes,
                new KarateNonNullType(new KarateType(KarateToken.Boolean, testFieldName))
            ).SetName("Non-nullable Boolean GraphQL type is converted to non-nullable boolean Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNonNullType
                {
                    Type = new GraphQLNamedType
                    {
                        Name = new GraphQLName(GraphQLToken.Float)
                    },
                },
                emptyGraphQLUserDefinedTypes,
                new KarateNonNullType(new KarateType(KarateToken.Number, testFieldName))
            ).SetName("Non-nullable Float GraphQL type is converted to non-nullable number Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNonNullType
                {
                    Type = new GraphQLNamedType
                    {
                        Name = new GraphQLName(GraphQLToken.Int)
                    }
                },
                emptyGraphQLUserDefinedTypes,
                new KarateNonNullType(new KarateType(KarateToken.Number, testFieldName))
            ).SetName("Non-nullable Int GraphQL type is converted to non-nullable number Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNonNullType
                {
                    Type = new GraphQLNamedType
                    {
                        Name = new GraphQLName(GraphQLToken.String)
                    }
                },
                emptyGraphQLUserDefinedTypes,
                new KarateNonNullType(new KarateType(KarateToken.String, testFieldName))
            ).SetName("Non-nullable String GraphQL type is converted to non-nullable string Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNonNullType
                {
                    Type = new GraphQLNamedType
                    {
                        Name = new GraphQLName(GraphQLToken.Id)
                    }
                },
                emptyGraphQLUserDefinedTypes,
                new KarateNonNullType(new KarateType(KarateToken.String, testFieldName))
            ).SetName("Non-nullable ID GraphQL type is converted to non-nullable string Karate type.");

            const string enumTypeName = "Color";

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNonNullType
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
                    GraphQLObjectTypeDefinitionsByName = new Dictionary<string, GraphQLObjectTypeDefinition>(),
                    GraphQLInterfaceTypeDefinitionsByName = new Dictionary<string, GraphQLInterfaceTypeDefinition>()
                },
                new KarateNonNullType(new KarateType(KarateToken.String, testFieldName))
            ).SetName("Non-nullable enum GraphQL type is converted to non-nullable string Karate type.");

            const string customTypeName = "ToDo";

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNonNullType
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
                    },
                    GraphQLInterfaceTypeDefinitionsByName = new Dictionary<string, GraphQLInterfaceTypeDefinition>()
                },
                new KarateNonNullType(
                    new KarateType(
                        $"{customTypeName.FirstCharToLower()}Schema",
                        testFieldName
                    )
                )
            ).SetName("Non-nullable custom GraphQL type is converted to non-nullable custom Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNonNullType
                {
                    Type = new GraphQLListType
                    {
                        Type = new GraphQLNamedType
                        {
                            Name = new GraphQLName(GraphQLToken.Boolean)
                        }
                    }
                },
                emptyGraphQLUserDefinedTypes,
                new KarateNonNullType(
                    new KarateListType(
                        new KarateNullType(
                            new KarateType(
                                KarateToken.Boolean,
                                testFieldName
                            )
                        )
                    )
                )
            ).SetName("Non-nullable list of nullable Boolean GraphQL type is converted to non-nullable list of nullable boolean Karate type.");
        }
    }
}