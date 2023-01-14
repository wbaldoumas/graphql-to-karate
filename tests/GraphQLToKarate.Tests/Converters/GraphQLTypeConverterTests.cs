using FluentAssertions;
using GraphQLParser.AST;
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
                new GraphQLNamedType
                {
                    Name = new GraphQLName(GraphQLToken.Boolean)
                },
                emptyGraphQLUserDefinedTypes,
                new KarateType(KarateToken.Boolean, testFieldName)
            ).SetName("Boolean GraphQL type is converted to boolean Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName(GraphQLToken.Float)
                },
                emptyGraphQLUserDefinedTypes,
                new KarateType(KarateToken.Number, testFieldName)
            ).SetName("Float GraphQL type is converted to number Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName(GraphQLToken.Int)
                },
                emptyGraphQLUserDefinedTypes,
                new KarateType(KarateToken.Number, testFieldName)
            ).SetName("Int GraphQL type is converted to number Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName(GraphQLToken.String)
                },
                emptyGraphQLUserDefinedTypes,
                new KarateType(KarateToken.String, testFieldName)
            ).SetName("String GraphQL type is converted to string Karate type.");


            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName(GraphQLToken.Id)
                },
                emptyGraphQLUserDefinedTypes,
                new KarateType(KarateToken.String, testFieldName)
            ).SetName("ID GraphQL type is converted to string Karate type.");

            const string enumTypeName = "Color";

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName(enumTypeName)
                },
                new GraphQLUserDefinedTypes
                {
                    GraphQLEnumTypeDefinitionsByName = new Dictionary<string, GraphQLEnumTypeDefinition>
                    {
                        { enumTypeName, new GraphQLEnumTypeDefinition() }
                    },
                    GraphQLObjectTypeDefinitionsByName = new Dictionary<string, GraphQLObjectTypeDefinition>()
                },
                new KarateType(KarateToken.String, testFieldName)
            ).SetName("Enum GraphQL type is converted to string Karate type.");

            const string customTypeName = "ToDo";

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName(customTypeName)
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
                new KarateType($"{customTypeName.FirstCharToLower()}Schema", testFieldName)
            ).SetName("Custom GraphQL type is converted to custom Karate type.");
        }
    }
}