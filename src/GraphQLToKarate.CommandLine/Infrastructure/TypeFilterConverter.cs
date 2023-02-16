using System.ComponentModel;
using System.Globalization;

namespace GraphQLToKarate.CommandLine.Infrastructure;

/// <summary>
///     Converts a comma-separated string into an ISet of strings, trimming whitespace.
/// </summary>
internal sealed class TypeFilterConverter : TypeConverter
{
    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is not string stringValue)
        {
            throw new NotSupportedException("Can't convert type filter value to type filter.");
        }

        return stringValue
            .Split(',')
            .Select(type => type.Trim())
            .Where(type => !string.IsNullOrEmpty(type))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}