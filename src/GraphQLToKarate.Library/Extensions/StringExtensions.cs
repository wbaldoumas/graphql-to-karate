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
}
