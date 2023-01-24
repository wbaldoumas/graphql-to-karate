using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Exceptions;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Tests.Mocks;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Extensions;

[TestFixture]
internal class GraphQLTypeExtensionsTests
{
    [TestCaseSource(nameof(TestCases))]
    public void GetTypeName_should_return_correct_type_name(
        GraphQLType graphQLType,
        string expectedTypeName
    ) => graphQLType.GetTypeName().Should().Be(expectedTypeName);

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
    public void GetTypeName_should_throw_an_exception_when_unsupported_graphql_type_is_encountered()
    {
        // arrange
        var graphQLType = new UnsupportedGraphQLType();

        // act
        var act = () => graphQLType.GetTypeName();

        // assert
        act.Should().ThrowExactly<InvalidGraphQLTypeException>();
    }
}