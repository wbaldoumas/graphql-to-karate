﻿using GraphQLToKarate.Library.Tokens;
using System.Diagnostics.CodeAnalysis;

namespace GraphQLToKarate.Library.Settings;

[ExcludeFromCodeCoverage]
public sealed class GraphQLToKarateSettings
{
    public bool ExcludeQueries { get; init; } = false;

    public bool IncludeMutations { get; init; } = false;

    public string QueryName { get; init; } = GraphQLToken.Query;

    public string MutationName { get; init; } = GraphQLToken.Mutation;

    public ISet<string> TypeFilter { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public ISet<string> QueryOperationFilter { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public ISet<string> MutationOperationFilter { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
}