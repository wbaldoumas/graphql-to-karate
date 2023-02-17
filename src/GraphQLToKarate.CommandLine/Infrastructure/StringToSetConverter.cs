using System.ComponentModel;
using System.Globalization;

namespace GraphQLToKarate.CommandLine.Infrastructure;

/// <summary>
///     Converts a comma-separated string into an ISet of strings, trimming whitespace.
/// </summary>
internal sealed class StringToSetConverter : TypeConverter
{
    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is not string stringValue)
        {
            throw new NotSupportedException("Can't convert type filter value to type filter.");
        }

        return stringValue
            .Split(',')
            .Select(item => item.Trim())
            .Where(item => !string.IsNullOrEmpty(item))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}