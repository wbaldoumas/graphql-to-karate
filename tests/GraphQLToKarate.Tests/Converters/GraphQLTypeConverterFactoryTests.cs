using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Converters;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Converters;

[TestFixture]
internal sealed class GraphQLTypeConverterFactoryTests
{
    private GraphQLTypeConverterFactory? _subjectUnderTest;

    [SetUp]
    public void SetUp() => _subjectUnderTest = new GraphQLTypeConverterFactory(new GraphQLTypeConverter());

    [Test]
    [TestCaseSource(nameof(TypeConverterTestCases))]
    public void CreateGraphQLTypeConverter_Returns_Expected_Type_Instance(
        GraphQLType graphQLType,
        Type expectedType)
    {
        // act
        var typeConverter = _subjectUnderTest!.CreateGraphQLTypeConverter(graphQLType);

        // assert
        typeConverter.Should().BeOfType(expectedType);
    }

    private static IEnumerable<TestCaseData> TypeConverterTestCases
    {
        get
        {
            yield return new TestCaseData(
                new GraphQLNonNullType(),
                typeof(GraphQLNonNullTypeConverter)
            ).SetName("Returns GraphQLNonNullTypeConverter for GraphQLNonNullType input");

            yield return new TestCaseData(
                new GraphQLNamedType(),
                typeof(GraphQLNullTypeConverter)
            ).SetName("Returns GraphQLNullTypeConverter for other GraphQLType input");
        }
    }
}