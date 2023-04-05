namespace GraphQLToKarate.Integration.Api.Types;

public sealed class Comment : Node, ISearchResult
{
    public required string Content { get; init; }

    public required User Author { get; init; }

    public required BlogPost BlogPost { get; init; }

    public required DateTime CreatedAt { get; init; }

    public required DateTime UpdatedAt { get; init; }
}