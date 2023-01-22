using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library.Features;

/// <summary>
///     Builds a Karate API testing scenario in string format.
/// </summary>
public interface IScenarioBuilder
{
    string Build(GraphQLQueryFieldType graphQLQueryFieldType);
}