using GraphQLToKarate.CommandLine.Settings;
using Serilog.Core;
using Spectre.Console.Cli;

namespace GraphQLToKarate.CommandLine.Infrastructure;

/// <inheritdoc cref="ICommandInterceptor"/>
internal sealed class LogCommandSettingsInterceptor : ICommandInterceptor
{
    public static readonly LoggingLevelSwitch LoggingLevelSwitch = new();

    public void Intercept(CommandContext context, CommandSettings settings)
    {
        if (settings is LogCommandSettings logCommandSettings)
        {
            LoggingLevelSwitch.MinimumLevel = logCommandSettings.LogLevel;
        }
    }
}