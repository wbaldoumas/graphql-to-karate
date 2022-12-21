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

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName(GraphQLToken.Boolean)
                },
                new GraphQLUserDefinedTypes(),
                new KarateType(KarateToken.Boolean, testFieldName)
            ).SetName("Boolean GraphQL type is converted to boolean Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName(GraphQLToken.Float)
                },
                new GraphQLUserDefinedTypes(),
                new KarateType(KarateToken.Number, testFieldName)
            ).SetName("Float GraphQL type is converted to number Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName(GraphQLToken.Int)
                },
                new GraphQLUserDefinedTypes(),
                new KarateType(KarateToken.Number, testFieldName)
            ).SetName("Int GraphQL type is converted to number Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName(GraphQLToken.String)
                },
                new GraphQLUserDefinedTypes(),
                new KarateType(KarateToken.String, testFieldName)
            ).SetName("String GraphQL type is converted to string Karate type.");


            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName(GraphQLToken.Id)
                },
                new GraphQLUserDefinedTypes(),
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
                    EnumTypes = new HashSet<string> { enumTypeName }
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
                    EnumTypes = new HashSet<string> { enumTypeName },
                    CustomTypes = new HashSet<string> { customTypeName }
                },
                new KarateType($"{customTypeName.FirstCharToLower()}Schema", testFieldName)
            ).SetName("Custom GraphQL type is converted to custom Karate type.");
        }
    }
}