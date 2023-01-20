using System.Diagnostics.CodeAnalysis;

namespace GraphQLToKarate.Library.Tokens;

/// <summary>
///     Contains tokens for each of the available Karate types.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class KarateToken
{
    public const string String = "string";

    public const string Number = "number";

    public const string Boolean = "boolean";

    public const string Object = "object";

    public const string Present = "present";

    public const string Null = "##";

    public const string NonNull = "#";

    public const string Array = "[]";
}