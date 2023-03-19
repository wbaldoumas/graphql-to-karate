using GraphQLToKarate.CommandLine.Settings;

namespace GraphQLToKarate.CommandLine.Mappers;

/// <summary>
///   A mapper that maps a <see cref="GraphQLToKarateUserConfiguration"/> to a <see cref="LoadedConvertCommandSettings"/>.
/// </summary>
internal interface IGraphQLToKarateUserConfigurationMapper
{
    /// <summary>
    ///     Maps a <see cref="GraphQLToKarateUserConfiguration"/> to a <see cref="LoadedConvertCommandSettings"/>.
    /// </summary>
    /// <param name="graphQLToKarateUserConfiguration">The <see cref="GraphQLToKarateUserConfiguration"/> to map.</param>
    /// <returns>A <see cref="LoadedConvertCommandSettings"/>.</returns>
    LoadedConvertCommandSettings Map(GraphQLToKarateUserConfiguration graphQLToKarateUserConfiguration);
}