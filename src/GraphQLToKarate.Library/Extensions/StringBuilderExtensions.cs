using System.Text;

namespace GraphQLToKarate.Library.Extensions;

internal static class StringBuilderExtensions
{
    public static void TrimEnd(this StringBuilder source, int length) => source.Remove(source.Length - length, length);
}