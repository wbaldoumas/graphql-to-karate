using System.ComponentModel;
using FluentAssertions;
using GraphQLToKarate.Library.Enums;
using GraphQLToKarate.Library.Extensions;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Enums;

[TestFixture]
internal sealed class GraphQLOperationTypeTests
{
    [Test]
    [TestCase(GraphQLOperationType.Query, "query")]
    [TestCase(GraphQLOperationType.Mutation, "mutation")]
    public void Name_returns_expected_value(
        GraphQLOperationType graphQLOperationType,
        string expectedValue
    ) => graphQLOperationType.Name().Should().Be(expectedValue);

    [Test]
    public void Name_throws_exception_for_invalid_value()
    {
        // arrange
        var act = () => ((GraphQLOperationType)5).Name();

        // assert
        act.Should().Throw<InvalidEnumArgumentException>();
    }
}