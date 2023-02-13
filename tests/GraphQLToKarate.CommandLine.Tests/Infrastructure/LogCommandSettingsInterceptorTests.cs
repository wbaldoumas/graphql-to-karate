using FluentAssertions;
using GraphQLToKarate.CommandLine.Infrastructure;
using GraphQLToKarate.CommandLine.Settings;
using NSubstitute;
using NUnit.Framework;
using Serilog.Events;
using Spectre.Console.Cli;

namespace GraphQLToKarate.CommandLine.Tests.Infrastructure;

[TestFixture]
internal sealed class LogCommandSettingsInterceptorTests
{
    private LogCommandSettingsInterceptor? _subjectUnderTest;

    [SetUp]
    public void SetUp() => _subjectUnderTest = new LogCommandSettingsInterceptor();

    [Test]
    public void LogLevelInterceptor_sets_log_level_as_expected()
    {
        // arrange
        const LogEventLevel expectedLogLevel = LogEventLevel.Verbose;

        var command = new LogCommandSettings
        {
            LogLevel = expectedLogLevel
        };

        // act
        _subjectUnderTest!.Intercept(
            new CommandContext(Substitute.For<IRemainingArguments>(), string.Empty, null),
            command
        );

        // assert
        LogCommandSettingsInterceptor.LoggingLevelSwitch.MinimumLevel
            .Should()
            .Be(expectedLogLevel);
    }

    [Test]
    public void LogLevelInterceptor_does_not_set_log_level_when_settings_is_not_LogCommandSettings()
    {
        // arrange
        LogCommandSettingsInterceptor.LoggingLevelSwitch.MinimumLevel = LogEventLevel.Fatal;

        var command = Substitute.For<CommandSettings>();

        // act
        _subjectUnderTest!.Intercept(
            new CommandContext(Substitute.For<IRemainingArguments>(), string.Empty, null),
            command
        );

        // assert
        LogCommandSettingsInterceptor.LoggingLevelSwitch.MinimumLevel
            .Should()
            .Be(LogEventLevel.Fatal);
    }
}