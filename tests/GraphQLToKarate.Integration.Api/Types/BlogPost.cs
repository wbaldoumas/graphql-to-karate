namespace GraphQLToKarate.Integration.Api.Types;

public sealed class BlogPost : Node, ISearchResult
{
    public required string Title { get; init; }

    public required string Content { get; init; }

    public required User Author { get; init; }

    public required DateTime CreatedAt { get; init; }

    public required DateTime UpdatedAt { get; init; }
}