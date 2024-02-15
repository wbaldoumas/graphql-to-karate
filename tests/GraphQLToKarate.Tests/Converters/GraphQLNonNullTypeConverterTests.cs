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
internal sealed class GraphQLNonNullTypeConverterTests
{
    private IGraphQLTypeConverter? _subjectUnderTest;

    private IGraphQLDocumentAdapter? _mockGraphQLDocumentAdapter;

    [SetUp]
    public void SetUp()
    {
        _mockGraphQLDocumentAdapter = Substitute.For<IGraphQLDocumentAdapter>();

        var graphQLTypeConverterFactory = new GraphQLTypeConverterFactory(new GraphQLTypeConverter());

        _subjectUnderTest = new GraphQLNonNullTypeConverter(graphQLTypeConverterFactory);
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

            var emptyGraphQLDocumentAdapter = new GraphQLDocumentAdapter(new GraphQLDocument([]));

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNonNullType(new GraphQLNamedType(new GraphQLName(GraphQLToken.Boolean))),
                emptyGraphQLDocumentAdapter,
                new KarateNonNullType(new KarateType(KarateToken.Boolean, testFieldName))
            ).SetName("Non-nullable Boolean GraphQL type is converted to non-nullable boolean Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNonNullType(new GraphQLNamedType(new GraphQLName(GraphQLToken.Float))),
                emptyGraphQLDocumentAdapter,
                new KarateNonNullType(new KarateType(KarateToken.Number, testFieldName))
            ).SetName("Non-nullable Float GraphQL type is converted to non-nullable number Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNonNullType(new GraphQLNamedType(new GraphQLName(GraphQLToken.Int))),
                emptyGraphQLDocumentAdapter,
                new KarateNonNullType(new KarateType(KarateToken.Number, testFieldName))
            ).SetName("Non-nullable Int GraphQL type is converted to non-nullable number Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNonNullType(new GraphQLNamedType(new GraphQLName(GraphQLToken.String))),
                emptyGraphQLDocumentAdapter,
                new KarateNonNullType(new KarateType(KarateToken.String, testFieldName))
            ).SetName("Non-nullable String GraphQL type is converted to non-nullable string Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNonNullType(new GraphQLNamedType(new GraphQLName(GraphQLToken.Id))),
                emptyGraphQLDocumentAdapter,
                new KarateNonNullType(new KarateType(KarateToken.String, testFieldName))
            ).SetName("Non-nullable ID GraphQL type is converted to non-nullable string Karate type.");

            const string enumTypeName = "Color";

            var graphQLDocumentWithEnumTypeDefinition = new GraphQLDocument(
                [new GraphQLEnumTypeDefinition(new GraphQLName(enumTypeName))]
            );

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNonNullType(new GraphQLNamedType(new GraphQLName(enumTypeName))),
                new GraphQLDocumentAdapter(graphQLDocumentWithEnumTypeDefinition),
                new KarateNonNullType(new KarateType(KarateToken.String, testFieldName))
            ).SetName("Non-nullable enum GraphQL type is converted to non-nullable string Karate type.");

            const string customTypeName = "ToDo";

            var graphQLDocumentWithEnumAndCustomTypeDefinition = new GraphQLDocument([
                    new GraphQLEnumTypeDefinition(new GraphQLName(enumTypeName)),
                    new GraphQLObjectTypeDefinition(new GraphQLName(customTypeName))
                ]
            );

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNonNullType(new GraphQLNamedType(new GraphQLName(customTypeName))),
                new GraphQLDocumentAdapter(graphQLDocumentWithEnumAndCustomTypeDefinition),
                new KarateNonNullType(
                    new KarateType(
                        $"({customTypeName.FirstCharToLower()}Schema)",
                        testFieldName
                    )
                )
            ).SetName("Non-nullable custom GraphQL type is converted to non-nullable custom Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNonNullType(
                    new GraphQLListType(new GraphQLNamedType(new GraphQLName(GraphQLToken.Boolean)))
                ),
                emptyGraphQLDocumentAdapter,
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
            ).SetName(
                "Non-nullable list of nullable Boolean GraphQL type is converted to non-nullable list of nullable boolean Karate type.");
        }
    }

    [Test]
    public void Convert_throws_exception_when_unsupported_graphql_type_is_encountered()
    {
        // arrange
        var unsupportedGraphQLType = new UnsupportedGraphQLType();

        var graphQLNonNullType = new GraphQLNonNullType(unsupportedGraphQLType);

        // act
        var act = () => _subjectUnderTest!.Convert(
            "unsupported",
            graphQLNonNullType,
            _mockGraphQLDocumentAdapter!
        );

        // assert
        act.Should().ThrowExactly<InvalidGraphQLTypeException>();
    }
}