using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library.Features;

/// <summary>
///     Builds a Karate API testing feature in string format.
/// </summary>
public interface IKarateFeatureBuilder
{
    /// <summary>
    ///     Builds a Karate API testing feature in string format.
    /// </summary>
    /// <param name="karateObjects">The Karate schema objects.</param>
    /// <param name="graphQLQueries">The GraphQL query objects.</param>
    /// <returns>The Karate feature as a string.</returns>
    string Build(
        IEnumerable<KarateObject> karateObjects,
        IEnumerable<GraphQLQueryFieldType> graphQLQueries
    );
}