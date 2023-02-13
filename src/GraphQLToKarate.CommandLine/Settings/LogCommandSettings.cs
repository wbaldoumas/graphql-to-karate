using Serilog.Events;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace GraphQLToKarate.CommandLine.Settings;

internal class LogCommandSettings : CommandSettings
{
    [CommandOption("--log-level")]
    [Description("Minimum level for logging")]
    [TypeConverter(typeof(LogLevelVerbosityConverter))]
    [DefaultValue(LogEventLevel.Information)]
    public LogEventLevel LogLevel { get; set; }
}