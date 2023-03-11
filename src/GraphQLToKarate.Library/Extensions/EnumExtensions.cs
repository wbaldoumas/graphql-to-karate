using System.ComponentModel;

namespace GraphQLToKarate.Library.Extensions;

/// <summary>
///     A home for extensions on the <see cref="Enum"/> type.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    ///     Retrieve the string representation of the enum.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="value">The enum value.</param>
    /// <returns>The string representation of the enum.</returns>
    /// <exception cref="InvalidEnumArgumentException">Thrown when the enum name is unidentifiable.</exception>
    public static string Name<TEnum>(this TEnum value) where TEnum : struct, Enum
    {
        var enumName = Enum.GetName(value);

        if (enumName is null)
        {
            throw new InvalidEnumArgumentException($"Unable to identify enum name for {value}!");
        }

        return enumName.FirstCharToLower();
    }
}