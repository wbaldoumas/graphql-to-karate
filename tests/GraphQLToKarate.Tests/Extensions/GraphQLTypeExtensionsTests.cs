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
                    new GraphQLNamedType
                    {
                        Name = new GraphQLName("NamedType")
                    },
                    "NamedType"
                )
                .SetName("GraphQLNamedType input");

            yield return new TestCaseData(
                    new GraphQLNonNullType
                    {
                        Type = new GraphQLNamedType
                        {
                            Name = new GraphQLName("NonNullType")
                        }
                    },
                    "NonNullType"
                )
                .SetName("GraphQLNonNullType input");

            yield return new TestCaseData(
                    new GraphQLListType
                    {
                        Type = new GraphQLNonNullType
                        {
                            Type = new GraphQLNamedType
                            {
                                Name = new GraphQLName("ListType")
                            }
                        }
                    },
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
        var namedNode = new GraphQLNamedType
        {
            Name = new GraphQLName("NamedType")
        };

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
            yield return new TestCaseData(
                new GraphQLNamedType(),
                false
            );

            yield return new TestCaseData(
                new GraphQLNonNullType
                {
                    Type = new GraphQLNamedType()
                },
                false
            );

            yield return new TestCaseData(
                new GraphQLListType
                {
                    Type = new GraphQLNamedType()
                },
                true
            );

            yield return new TestCaseData(
                new GraphQLNonNullType
                {
                    Type = new GraphQLListType
                    {
                        Type = new GraphQLNamedType()
                    }
                },
                true
            );
        }
    }
}