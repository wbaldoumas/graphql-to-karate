namespace GraphQLToKarate.Library.Extensions;

/// <summary>
///     A home for extension methods on <see cref="ICollection{T}"/>.
/// </summary>
internal static class CollectionExtensions
{
    /// <summary>
    ///    Returns true if the collection is empty or contains the given item. This method is useful
    ///    for optional "filter" type operations, where collection to filter with would be empty if
    ///    the user didn't specify any filters.
    /// </summary>
    /// <typeparam name="T">The type of the collection.</typeparam>
    /// <param name="collection">The collection to check.</param>
    /// <param name="item">The item to check for.</param>
    /// <returns>Whether the collection is empty or contains the given item.</returns>
    public static bool NoneOrContains<T>(this ICollection<T> collection, T item) =>
        !collection.Any() || collection.Contains(item);
}