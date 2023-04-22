using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Exceptions;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Tests.Mocks;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Extensions;

[TestFixture]
internal sealed class GraphQLTypeExtensionsTests
{
    [TestCaseSource(nameof(TestCases))]
    public void GetUnwrappedTypeName_should_return_correct_type_name(
        GraphQLType graphQLType,
        string expectedTypeName
    ) => graphQLType.GetUnwrappedTypeName().Should().Be(expectedTypeName);

    private static IEnumerable<TestCaseData> TestCases
    {
        get
        {
            yield return new TestCaseData(
                    new GraphQLNamedType(new GraphQLName("NamedType")),
                    "NamedType"
                )
                .SetName("GraphQLNamedType input");

            yield return new TestCaseData(
                    new GraphQLNonNullType(new GraphQLNamedType(new GraphQLName("NonNullType"))),
                    "NonNullType"
                )
                .SetName("GraphQLNonNullType input");

            yield return new TestCaseData(
                    new GraphQLListType(new GraphQLNonNullType(new GraphQLNamedType(new GraphQLName("ListType")))),
                    "ListType"
                )
                .SetName("GraphQLListType input");
        }
    }

    [Test]
    public void GetUnwrappedTypeName_should_throw_an_exception_when_unsupported_graphql_type_is_encountered()
    {
        // arrange
        var graphQLType = new UnsupportedGraphQLType();

        // act
        var act = () => graphQLType.GetUnwrappedTypeName();

        // assert
        act.Should().ThrowExactly<InvalidGraphQLTypeException>();
    }

    [Test]
    public void NameValue_should_return_correct_name_value()
    {
        // arrange
        var namedNode = new GraphQLNamedType(new GraphQLName("NamedType"));

        // act
        var result = namedNode.NameValue();

        // assert
        result.Should().Be("NamedType");
    }

    [Test]
    [TestCaseSource(nameof(IsListTypeTestCases))]
    public void IsListType_returns_expected_value(GraphQLType graphQLType, bool expectedIsListType) =>
        graphQLType.IsListType().Should().Be(expectedIsListType);

    private static IEnumerable<TestCaseData> IsListTypeTestCases
    {
        get
        {
            var graphQLName = new GraphQLName("foo");

            yield return new TestCaseData(
                new GraphQLNamedType(graphQLName),
                false
            );

            yield return new TestCaseData(
                new GraphQLNonNullType(new GraphQLNamedType(graphQLName)),
                false
            );

            yield return new TestCaseData(
                new GraphQLListType(new GraphQLNamedType(graphQLName)),
                true
            );

            yield return new TestCaseData(
                new GraphQLNonNullType(new GraphQLListType(new GraphQLNamedType(graphQLName))),
                true
            );
        }
    }

    [Test]
    [TestCaseSource(nameof(IsNullTypeTestCases))]
    public void IsNullType_returns_expected_value(GraphQLType graphQLType, bool expectedIsNullType) =>
        graphQLType.IsNullType().Should().Be(expectedIsNullType);

    private static IEnumerable<TestCaseData> IsNullTypeTestCases
    {
        get
        {
            var graphQLName = new GraphQLName("foo");

            yield return new TestCaseData(
                new GraphQLNamedType(graphQLName),
                true
            );

            yield return new TestCaseData(
                new GraphQLNonNullType(new GraphQLNamedType(graphQLName)),
                false
            );
        }
    }
}