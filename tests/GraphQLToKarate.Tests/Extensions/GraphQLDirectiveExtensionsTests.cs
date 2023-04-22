using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Apollo;
using GraphQLToKarate.Library.Extensions;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Extensions;

[TestFixture]
internal sealed class GraphQLDirectiveExtensionsTests
{
    [TestCase(Directives.Inaccessible, true)]
    [TestCase(Directives.External, false)]
    [TestCase("otherDirective", false)]
    public void IsInaccessible_returns_expected_result(string directiveName, bool expectedResult)
    {
        var directive = new GraphQLDirective(new GraphQLName(directiveName));

        var result = directive.IsInaccessible();

        result.Should().Be(expectedResult);
    }

    [TestCase(Directives.Inaccessible, false)]
    [TestCase(Directives.External, true)]
    [TestCase("otherDirective", false)]
    public void IsExternal_returns_expected_result(string directiveName, bool expectedResult)
    {
        var directive = new GraphQLDirective(new GraphQLName(directiveName));

        var result = directive.IsExternal();

        result.Should().Be(expectedResult);
    }
}