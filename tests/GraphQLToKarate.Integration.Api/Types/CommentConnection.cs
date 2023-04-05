namespace GraphQLToKarate.Integration.Api.Types;

public sealed class CommentConnection
{
    public required Comment[] Nodes { get; init; }

    public required PageInfo PageInfo { get; init; }
}