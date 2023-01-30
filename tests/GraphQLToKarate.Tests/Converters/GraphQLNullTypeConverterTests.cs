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
internal sealed class GraphQLNullTypeConverterTests
{
    private IGraphQLTypeConverter? _subjectUnderTest;

    private IGraphQLDocumentAdapter? _mockGraphQLDocumentAdapter;

    [SetUp]
    public void SetUp()
    {
        _mockGraphQLDocumentAdapter = Substitute.For<IGraphQLDocumentAdapter>();

        var graphQLTypeConverterFactory = new GraphQLTypeConverterFactory();

        _subjectUnderTest = new GraphQLNullTypeConverter(graphQLTypeConverterFactory);
    }

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
                new KarateNullType(new KarateType(KarateToken.Boolean, testFieldName))
            ).SetName("Nullable Boolean GraphQL type is converted to nullable boolean Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName(GraphQLToken.Float)
                },
                emptyGraphQLDocumentAdapter,
                new KarateNullType(new KarateType(KarateToken.Number, testFieldName))
            ).SetName("Nullable Float GraphQL type is converted to nullable number Karate type.");


            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName(GraphQLToken.Int)
                },
                emptyGraphQLDocumentAdapter,
                new KarateNullType(new KarateType(KarateToken.Number, testFieldName))
            ).SetName("Nullable Int GraphQL type is converted to nullable number Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName(GraphQLToken.String)
                },
                emptyGraphQLDocumentAdapter,
                new KarateNullType(new KarateType(KarateToken.String, testFieldName))
            ).SetName("Nullable String GraphQL type is converted to nullable string Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName(GraphQLToken.Id)
                },
                emptyGraphQLDocumentAdapter,
                new KarateNullType(new KarateType(KarateToken.String, testFieldName))
            ).SetName("Nullable ID GraphQL type is converted to nullable string Karate type.");

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
                new KarateNullType(new KarateType(KarateToken.String, testFieldName))
            ).SetName("Nullable enum GraphQL type is converted to nullable string Karate type.");

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
                new KarateNullType(new KarateType($"{customTypeName.FirstCharToLower()}Schema", testFieldName))
            ).SetName("Nullable custom GraphQL type is converted to nullable custom Karate type.");

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
                new KarateNullType(
                    new KarateListType(
                        new KarateNullType(
                            new KarateType(KarateToken.Boolean, testFieldName)
                        )
                    )
                )
            ).SetName("Nullable list of nullable booleans is converted to nullable list karate type.");
        }
    }

    [Test]
    public void Convert_throws_exception_when_unsupported_graphql_type_is_encountered()
    {
        // arrange
        var unsupportedGraphQLType = new UnsupportedGraphQLType();

        // act
        var act = () => _subjectUnderTest!.Convert(
            "unsupported",
            unsupportedGraphQLType,
            _mockGraphQLDocumentAdapter!
        );

        // assert
        act.Should().ThrowExactly<InvalidGraphQLTypeException>();
    }
}