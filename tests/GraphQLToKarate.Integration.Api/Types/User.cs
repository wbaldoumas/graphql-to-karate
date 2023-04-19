namespace GraphQLToKarate.Integration.Api.Types;

public sealed class User : Node, ISearchResult
{
    public required string Name { get; init; }

    public required UserRole Role { get; init; }

    public required IEnumerable<BlogPost> BlogPosts { get; init; }

    public required DateTime CreatedAt { get; init; }

    public required DateTime UpdatedAt { get; init; }
}