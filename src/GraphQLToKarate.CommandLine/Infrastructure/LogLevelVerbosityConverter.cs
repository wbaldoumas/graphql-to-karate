using Serilog.Events;
using System.ComponentModel;
using System.Globalization;

namespace GraphQLToKarate.CommandLine.Infrastructure;

/// <summary>
///     Converts a log-level string into a <see cref="LogEventLevel"/>.
/// </summary>
internal sealed class LogLevelVerbosityConverter : TypeConverter
{
    private readonly Dictionary<string, LogEventLevel> _logLevelLookup = new(StringComparer.OrdinalIgnoreCase)
    {
        { "verbose", LogEventLevel.Verbose },
        { "debug", LogEventLevel.Debug },
        { "information", LogEventLevel.Information },
        { "warning", LogEventLevel.Warning },
        { "error", LogEventLevel.Error },
        { "fatal", LogEventLevel.Fatal }
    };

    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is not string stringValue)
        {
            throw new NotSupportedException("Can't convert log level value to log level verbosity.");
        }

        if (_logLevelLookup.TryGetValue(stringValue, out var verbosity))
        {
            return verbosity;
        }

        const string format = "The value '{0}' is not a valid log level verbosity.";

        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, format, value));
    }
}