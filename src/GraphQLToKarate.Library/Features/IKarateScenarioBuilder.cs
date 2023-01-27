using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library.Features;

/// <summary>
///     Builds a Karate API testing scenario in string format.
/// </summary>
public interface IKarateScenarioBuilder
{
    string Build(GraphQLQueryFieldType graphQLQueryFieldType);
}