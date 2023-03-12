using GraphQLToKarate.CommandLine.Settings;
using Serilog.Core;
using Spectre.Console.Cli;

namespace GraphQLToKarate.CommandLine.Infrastructure;

/// <summary>
///     Intercepts the <see cref="LogCommandSettings"/> and sets the <see cref="LogCommandSettingsInterceptor.LoggingLevelSwitch"/> accordingly.
/// </summary>
internal sealed class LogCommandSettingsInterceptor : ICommandInterceptor
{
    public static readonly LoggingLevelSwitch LoggingLevelSwitch = new();

    /// <summary>
    ///     Intercepts the <see cref="LogCommandSettings"/> and sets the <see cref="LogCommandSettingsInterceptor.LoggingLevelSwitch"/> accordingly.
    /// </summary>
    /// <param name="context">The command context.</param>
    /// <param name="settings">The command settings.</param>
    public void Intercept(CommandContext context, CommandSettings settings)
    {
        if (settings is LogCommandSettings logCommandSettings)
        {
            LoggingLevelSwitch.MinimumLevel = logCommandSettings.LogLevel;
        }
    }
}