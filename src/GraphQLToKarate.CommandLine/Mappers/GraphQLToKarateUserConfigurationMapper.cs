using GraphQLToKarate.CommandLine.Settings;
using GraphQLToKarate.Library.Mappings;

namespace GraphQLToKarate.CommandLine.Mappers;

/// <inheritdoc cref="IGraphQLToKarateUserConfigurationMapper"/>>
internal sealed class GraphQLToKarateUserConfigurationMapper : IGraphQLToKarateUserConfigurationMapper
{
    public LoadedConvertCommandSettings Map(GraphQLToKarateUserConfiguration graphQLToKarateUserConfiguration) => new()
    {
        GraphQLSchema = graphQLToKarateUserConfiguration.GraphQLSchema,
        InputFile = graphQLToKarateUserConfiguration.InputFile,
        OutputFile = graphQLToKarateUserConfiguration.OutputFile,
        BaseUrl = graphQLToKarateUserConfiguration.BaseUrl,
        QueryName = graphQLToKarateUserConfiguration.QueryName,
        MutationName = graphQLToKarateUserConfiguration.MutationName,
        ExcludeQueries = graphQLToKarateUserConfiguration.ExcludeQueries,
        CustomScalarMapping = new CustomScalarMapping(graphQLToKarateUserConfiguration.CustomScalarMapping),
        IncludeMutations = graphQLToKarateUserConfiguration.IncludeMutations,
        QueryOperationFilter = graphQLToKarateUserConfiguration.QueryOperationFilter,
        MutationOperationFilter = graphQLToKarateUserConfiguration.MutationOperationFilter,
        TypeFilter = graphQLToKarateUserConfiguration.TypeFilter
    };
}