using System.Text;

namespace GraphQLToKarate.Library.Extensions;

internal static class StringBuilderExtensions
{
    /// <summary>
    ///     Trims the last <paramref name="count"/> characters from the end of the <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="source">The <see cref="StringBuilder"/> to manipulate.</param>
    /// <param name="count">The count of characters to trim from the end of the <see cref="StringBuilder"/>.</param>
    public static void TrimEnd(this StringBuilder source, int count) => source.Remove(source.Length - count, count);
}