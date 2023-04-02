namespace GraphQLToKarate.Integration.Api.Types;

[InterfaceType("Node")]
public abstract class Node
{
    [GraphQLType(typeof(IdType))]
    public required string Id { get; init; }
}