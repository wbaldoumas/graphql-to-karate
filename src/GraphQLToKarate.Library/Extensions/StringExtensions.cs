namespace GraphQLToKarate.Library.Extensions;

/// <summary>
///     Extension methods on the <see cref="string"/> type.
/// </summary>
internal static class StringExtensions
{
    /// <summary>
    ///     Convert the first character of the given <paramref name="source"/> <see cref="string"/> to lowercase.
    /// </summary>
    /// <param name="source">The source <see cref="string"/> to manipulate.</param>
    /// <returns>The <paramref name="source"/> <see cref="string"/> with its first character converted to lowercase.</returns>
    public static string FirstCharToLower(this string source)
    {
        if (string.IsNullOrEmpty(source))
        {
            return string.Empty;
        }

        return string.Create(source.Length, source, static (characters, str) =>
        {
            characters[0] = char.ToLowerInvariant(str[0]);
            str.AsSpan(1).CopyTo(characters[1..]);
        });
    }

    /// <summary>
    ///     Convert the first character of the given <paramref name="source"/> <see cref="string"/> to uppercase.
    /// </summary>
    /// <param name="source">The source <see cref="string"/> to manipulate.</param>
    /// <returns>The <paramref name="source"/> <see cref="string"/> with its first character converted to uppercase.</returns>
    public static string FirstCharToUpper(this string source)
    {
        if (string.IsNullOrEmpty(source))
        {
            return string.Empty;
        }

        return string.Create(source.Length, source, static (characters, str) =>
        {
            characters[0] = char.ToUpperInvariant(str[0]);
            str.AsSpan(1).CopyTo(characters[1..]);
        });
    }

    /// <summary>
    ///     Indents a <paramref name="source"/> string (including multi-line strings) by the specified indent amount.
    ///
    ///     This is different from <see cref="string.PadLeft(int)"/> in that it properly indents strings that span multiple lines.
    /// </summary>
    /// <param name="source">The string to manipulate.</param>
    /// <param name="indent">The amount to indent the string.</param>
    /// <returns>The source string, indented by the specified <paramref name="indent"/> amount.</returns>
    public static string Indent(this string source, int indent)
    {
        var indentation = new string(' ', indent);

        return indentation + source.Replace(Environment.NewLine, Environment.NewLine + indentation, StringComparison.OrdinalIgnoreCase);
    }
}
