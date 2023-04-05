namespace GraphQLToKarate.Integration.Api.Types;

public sealed class UserConnection
{
    public required User[] Nodes { get; init; }

    public required PageInfo PageInfo { get; init; }
}