using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Apollo;
using GraphQLToKarate.Library.Extensions;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Extensions;

[TestFixture]
public class GraphQLEnumValueDefinitionExtensionsTests
{
    [Test]
    [TestCase(Directives.Inaccessible, true)]
    [TestCase(Directives.External, false)]
    [TestCase("someDirective", false)]
    public void IsInaccessible_returns_expected_result(string directiveName, bool expected)
    {
        // arrange
        var graphQLEnumValueDefinition = new GraphQLEnumValueDefinition(
            new GraphQLName("someName"),
            new GraphQLEnumValue(new GraphQLName("someValue"))
        )
        {
            Directives = new GraphQLDirectives(new List<GraphQLDirective>
            {
                new(new GraphQLName(directiveName))
            })
        };

        // act
        var result = graphQLEnumValueDefinition.IsInaccessible();

        // assert
        result.Should().Be(expected);
    }

    [Test]
    [TestCase(Directives.Inaccessible, false)]
    [TestCase(Directives.External, true)]
    [TestCase("someDirective", false)]
    public void IsExternal_returns_expected_result(string directiveName, bool expected)
    {
        // arrange
        var graphQLEnumValueDefinition = new GraphQLEnumValueDefinition(
            new GraphQLName("someName"),
            new GraphQLEnumValue(new GraphQLName("someValue"))
        )
        {
            Directives = new GraphQLDirectives(new List<GraphQLDirective>
            {
                new(new GraphQLName(directiveName))
            })
        };

        // act
        var result = graphQLEnumValueDefinition.IsExternal();

        // assert
        result.Should().Be(expected);
    }

    [Test]
    public void IsInaccessible_returns_false_for_null_directives()
    {
        // arrange
        var graphQLEnumValueDefinition = new GraphQLEnumValueDefinition(
            new GraphQLName("someName"),
            new GraphQLEnumValue(new GraphQLName("someValue"))
        )
        {
            Directives = null
        };

        // act
        var result = graphQLEnumValueDefinition.IsInaccessible();

        // assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsExternal_returns_false_for_null_directives()
    {
        // arrange
        var graphQLEnumValueDefinition = new GraphQLEnumValueDefinition(
            new GraphQLName("someName"),
            new GraphQLEnumValue(new GraphQLName("someValue"))
        )
        {
            Directives = null
        };

        // act
        var result = graphQLEnumValueDefinition.IsExternal();

        // assert
        result.Should().BeFalse();
    }
}