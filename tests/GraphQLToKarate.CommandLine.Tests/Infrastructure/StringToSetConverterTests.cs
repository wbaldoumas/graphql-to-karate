using System.ComponentModel;
using System.Globalization;
using FluentAssertions;
using GraphQLToKarate.CommandLine.Infrastructure;
using NUnit.Framework;

namespace GraphQLToKarate.CommandLine.Tests.Infrastructure;

[TestFixture]
internal sealed class StringToSetConverterTests
{
    private TypeConverter? _subjectUnderTest;

    [SetUp]
    public void SetUp() => _subjectUnderTest = new StringToSetConverter();

    [Test]
    public void ConvertFrom_should_return_expected_ISet_of_strings_when_given_valid_input()
    {
        // arrange
        const string input = "  Foo, Bar, Baz  ";

        // act
        var result = _subjectUnderTest!.ConvertFrom(null, CultureInfo.InvariantCulture, input);

        // assert
        result.Should()
            .BeEquivalentTo(new HashSet<string>(new[] { "Foo", "Bar", "Baz" }, StringComparer.OrdinalIgnoreCase));
    }

    [Test]
    public void ConvertFrom_should_return_empty_ISet_of_strings_when_given_empty_input()
    {
        // arrange
        var input = string.Empty;

        // act
        var result = _subjectUnderTest!.ConvertFrom(null, CultureInfo.InvariantCulture, input);

        // assert
        result.Should()
            .BeEquivalentTo(new HashSet<string>(StringComparer.OrdinalIgnoreCase));
    }

    [Test]
    public void ConvertFrom_should_not_return_set_containing_empty_strings_when_they_are_in_input()
    {
        // arrange
        const string input = "  Foo, , Bar, Baz  ";

        // act

        var result = _subjectUnderTest!.ConvertFrom(null, CultureInfo.InvariantCulture, input);

        // assert
        result.Should()
            .BeEquivalentTo(new HashSet<string>(new[] { "Foo", "Bar", "Baz" }, StringComparer.OrdinalIgnoreCase));
    }

    [Test]
    public void ConvertFrom_should_throw_NotSupportedException_when_given_non_string_input()
    {
        // arrange + act
        var act = () => _subjectUnderTest!.ConvertFrom(null, CultureInfo.InvariantCulture, 123);

        // assert
        act.Should()
            .Throw<NotSupportedException>()
            .WithMessage("Can't convert type filter value to type filter.");
    }
}