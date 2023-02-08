using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Converters;

[TestFixture]
internal sealed class GraphQLTypeConverterTests
{
    private IGraphQLTypeConverter? _subjectUnderTest;

    [SetUp]
    public void SetUp() => _subjectUnderTest = new GraphQLTypeConverter();

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
                new GraphQLNamedType
                {
                    Name = new GraphQLName(GraphQLToken.Boolean)
                },
                emptyGraphQLDocumentAdapter,
                new KarateType(KarateToken.Boolean, testFieldName)
            ).SetName("Boolean GraphQL type is converted to boolean Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName(GraphQLToken.Float)
                },
                emptyGraphQLDocumentAdapter,
                new KarateType(KarateToken.Number, testFieldName)
            ).SetName("Float GraphQL type is converted to number Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName(GraphQLToken.Int)
                },
                emptyGraphQLDocumentAdapter,
                new KarateType(KarateToken.Number, testFieldName)
            ).SetName("Int GraphQL type is converted to number Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName(GraphQLToken.String)
                },
                emptyGraphQLDocumentAdapter,
                new KarateType(KarateToken.String, testFieldName)
            ).SetName("String GraphQL type is converted to string Karate type.");


            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName(GraphQLToken.Id)
                },
                emptyGraphQLDocumentAdapter,
                new KarateType(KarateToken.String, testFieldName)
            ).SetName("ID GraphQL type is converted to string Karate type.");

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
                new GraphQLNamedType
                {
                    Name = new GraphQLName(enumTypeName)
                },
                new GraphQLDocumentAdapter(graphQLDocumentWithEnumTypeDefinition),
                new KarateType(KarateToken.String, testFieldName)
            ).SetName("Enum GraphQL type is converted to string Karate type.");

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
                new GraphQLNamedType
                {
                    Name = new GraphQLName(customTypeName)
                },
                new GraphQLDocumentAdapter(graphQLDocumentWithEnumAndCustomTypeDefinition),
                new KarateType($"{customTypeName.FirstCharToLower()}Schema", testFieldName)
            ).SetName("Custom GraphQL type is converted to custom Karate type.");

            const string interfaceTypeName = "TodoInterface";

            const string unionTypeName = "TodoUnion";

            var populatedGraphQLDocument = new GraphQLDocument
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
                    },
                    new GraphQLInterfaceTypeDefinition
                    {
                        Name = new GraphQLName(interfaceTypeName)
                    },
                    new GraphQLUnionTypeDefinition
                    {
                        Name = new GraphQLName(unionTypeName)
                    }
                }
            };

            var populatedGraphQLDocumentAdapter = new GraphQLDocumentAdapter(populatedGraphQLDocument);

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName(interfaceTypeName)
                },
                new GraphQLDocumentAdapter(populatedGraphQLDocument),
                new KarateType($"{interfaceTypeName.FirstCharToLower()}Schema", testFieldName)
            ).SetName("Custom GraphQL type is converted to custom Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName("Unknown")
                },
                emptyGraphQLDocumentAdapter,
                new KarateType(KarateToken.Present, testFieldName)
            ).SetName("Unknown GraphQL type is converted to present Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName(unionTypeName)
                },
                populatedGraphQLDocumentAdapter,
                new KarateType(KarateToken.Present, testFieldName)
            ).SetName("Union GraphQL type as a field is converted to present Karate type.");
        }
    }
}