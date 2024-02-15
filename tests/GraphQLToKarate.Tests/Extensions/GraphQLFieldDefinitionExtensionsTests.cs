using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Apollo;
using GraphQLToKarate.Library.Extensions;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Extensions;

[TestFixture]
public class GraphQLFieldDefinitionExtensionsTests
{
    [Test]
    [TestCase(Directives.Inaccessible, true)]
    [TestCase(Directives.External, false)]
    [TestCase("someDirective", false)]
    public void IsInaccessible_returns_expected_result(string directiveName, bool expected)
    {
        // arrange
        var fieldDefinition = new GraphQLFieldDefinition(
            new GraphQLName("someName"),
            new GraphQLNamedType(new GraphQLName("someType"))
        )
        {
            Directives = new GraphQLDirectives(
                [new GraphQLDirective(new GraphQLName(directiveName))]
            )
        };

        // act
        var result = fieldDefinition.IsInaccessible();

        // assert
        result.Should().Be(expected);
    }

    [Test]
    [TestCase(Directives.External, true)]
    [TestCase(Directives.Inaccessible, false)]
    [TestCase("someDirective", false)]
    public void IsExternal_returns_expected_result(string directiveName, bool expected)
    {
        // arrange
        var fieldDefinition = new GraphQLFieldDefinition(
            new GraphQLName("someName"),
            new GraphQLNamedType(new GraphQLName("someType"))
        )
        {
            Directives = new GraphQLDirectives(
                [new GraphQLDirective(new GraphQLName(directiveName))]
            )
        };

        // act
        var result = fieldDefinition.IsExternal();

        // assert
        result.Should().Be(expected);
    }

    [Test]
    public void IsInaccessible_with_null_directives_returns_false()
    {
        // arrange
        var fieldDefinition = new GraphQLFieldDefinition(
            new GraphQLName("someName"),
            new GraphQLNamedType(new GraphQLName("someType"))
        );

        // act
        var result = fieldDefinition.IsInaccessible();

        // assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsExternal_with_null_directives_returns_false()
    {
        // arrange
        var fieldDefinition = new GraphQLFieldDefinition(
            new GraphQLName("someName"),
            new GraphQLNamedType(new GraphQLName("someType"))
        );

        // act
        var result = fieldDefinition.IsExternal();

        // assert
        result.Should().BeFalse();
    }
}