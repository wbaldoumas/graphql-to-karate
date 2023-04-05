namespace GraphQLToKarate.Integration.Api.Types;

public sealed class BlogPostConnection
{
    public required BlogPost[] Nodes { get; init; }

    public required PageInfo PageInfo { get; init; }
}