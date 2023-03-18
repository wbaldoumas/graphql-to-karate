using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Apollo;
using GraphQLToKarate.Library.Extensions;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Extensions;

[TestFixture]
public class GraphQLInputValueDefinitionExtensionsTests
{
    [Test]
    public void IsInaccessible_returns_false_when_directives_is_null()
    {
        // arrange
        var inputValueDefinition = new GraphQLInputValueDefinition();

        // act
        var result = inputValueDefinition.IsInaccessible();

        // assert
        result.Should().BeFalse();
    }

    [Test]
    [TestCase(Directives.Inaccessible, true)]
    [TestCase(Directives.External, false)]
    [TestCase("someDirective", false)]
    public void IsInaccessible_returns_expected_result(string directiveName, bool expected)
    {
        // arrange
        var inputValueDefinition = new GraphQLInputValueDefinition
        {
            Directives = new GraphQLDirectives
            {
                Items = new List<GraphQLDirective>
                {
                    new()
                    {
                        Name = new GraphQLName(directiveName)
                    }
                }
            }
        };

        // act
        var result = inputValueDefinition.IsInaccessible();

        // assert
        result.Should().Be(expected);
    }

    [Test]
    public void IsExternal_returns_false_when_directives_is_null()
    {
        // arrange
        var inputValueDefinition = new GraphQLInputValueDefinition();

        // act
        var result = inputValueDefinition.IsExternal();

        // assert
        result.Should().BeFalse();
    }

    [Test]
    [TestCase(Directives.Inaccessible, false)]
    [TestCase(Directives.External, true)]
    [TestCase("someDirective", false)]
    public void IsExternal_returns_expected_result(string directiveName, bool expected)
    {
        // arrange
        var inputValueDefinition = new GraphQLInputValueDefinition
        {
            Directives = new GraphQLDirectives
            {
                Items = new List<GraphQLDirective>
                {
                    new()
                    {
                        Name = new GraphQLName(directiveName)
                    }
                }
            }
        };

        // act
        var result = inputValueDefinition.IsExternal();

        // assert
        result.Should().Be(expected);
    }
}