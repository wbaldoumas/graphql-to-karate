using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library.Features;

/// <summary>
///     Builds a Karate API testing scenario in string format.
/// </summary>
public interface IKarateScenarioBuilder
{
    /// <summary>
    ///     Build a Karate API testing scenario in string format.
    /// </summary>
    /// <param name="graphQLOperation">The GraphQL query field type to build a Karate scenario for.</param>
    /// <param name="graphQLDocumentAdapter">The GraphQL document adapter to use.</param>
    /// <returns>A Karate API testing scenario in string format.</returns>
    string Build(GraphQLOperation graphQLOperation, IGraphQLDocumentAdapter graphQLDocumentAdapter);
}