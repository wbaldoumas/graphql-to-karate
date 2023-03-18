using FluentAssertions;
using GraphQLToKarate.CommandLine.Infrastructure;
using NUnit.Framework;
using Serilog.Events;
using System.ComponentModel;
using System.Globalization;

namespace GraphQLToKarate.CommandLine.Tests.Infrastructure;

[TestFixture]
internal sealed class LogLevelVerbosityConverterTests
{
    private TypeConverter? _subjectUnderTest;

    [SetUp]
    public void SetUp() => _subjectUnderTest = new LogLevelVerbosityConverter();

    [Test]
    [TestCase("verbose", LogEventLevel.Verbose)]
    [TestCase("debug", LogEventLevel.Debug)]
    [TestCase("information", LogEventLevel.Information)]
    [TestCase("warning", LogEventLevel.Warning)]
    [TestCase("error", LogEventLevel.Error)]
    [TestCase("fatal", LogEventLevel.Fatal)]
    public void ConvertFrom_should_return_expected_LogEventLevel_when_given_valid_input(
        string input,
        LogEventLevel expectedLogEventLevel
    ) => _subjectUnderTest!
        .ConvertFrom(null, CultureInfo.InvariantCulture, input)
        .Should()
        .Be(expectedLogEventLevel);

    [Test]
    public void ConvertFrom_should_throw_InvalidOperationException_when_given_invalid_input()
    {
        // arrange + act
        var act = () => _subjectUnderTest!.ConvertFrom(null, CultureInfo.InvariantCulture, "invalid");

        // assert
        act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("The value 'invalid' is not a valid log level verbosity.");
    }

    [Test]
    public void ConvertFrom_should_throw_NotSupportedException_when_given_non_string_input()
    {
        // arrange + act
        var act = () => _subjectUnderTest!.ConvertFrom(null, CultureInfo.InvariantCulture, 123);

        // assert
        act.Should()
            .Throw<NotSupportedException>()
            .WithMessage("Can't convert log level value to log level verbosity.");
    }
}