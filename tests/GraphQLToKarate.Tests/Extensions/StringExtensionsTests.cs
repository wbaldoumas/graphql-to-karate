﻿using FluentAssertions;
using GraphQLToKarate.Library.Extensions;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Extensions;

[TestFixture]
internal sealed class StringExtensionsTests
{
    [Test]
    [TestCase("all lowercase", "all lowercase")]
    [TestCase("ALL UPPERCASE", "aLL UPPERCASE")]
    [TestCase("Mixed Case", "mixed Case")]
    [TestCase("1 has numbers", "1 has numbers")]
    [TestCase("% has symbols", "% has symbols")]
    [TestCase(null, "")]
    [TestCase("", "")]
    public void FirstCharToLowerTest(string input, string expectedOutput) =>
        input.FirstCharToLower().Should().Be(expectedOutput);

    [Test]
    [TestCase("all lowercase", "All lowercase")]
    [TestCase("ALL UPPERCASE", "ALL UPPERCASE")]
    [TestCase("mixed Case", "Mixed Case")]
    [TestCase("1 has numbers", "1 has numbers")]
    [TestCase("% has symbols", "% has symbols")]
    [TestCase(null, "")]
    [TestCase("", "")]
    public void FirstCharToUpperTest(string input, string expectedOutput) =>
        input.FirstCharToUpper().Should().Be(expectedOutput);
}