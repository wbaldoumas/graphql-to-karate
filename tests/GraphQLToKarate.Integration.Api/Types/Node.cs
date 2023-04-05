namespace GraphQLToKarate.Integration.Api.Types;

[InterfaceType("Node")]
public abstract class Node
{
    [GraphQLType(typeof(IdType))]
    [GraphQLNonNullType]
    public required string Id { get; init; }
}