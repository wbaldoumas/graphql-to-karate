namespace GraphQLToKarate.Integration.Api.Types;

public sealed class User : Node
{
    public required string Name { get; init; }

    public required UserRole Role { get; init; }

    public required DateTime CreatedAt { get; init; }

    public required DateTime UpdatedAt { get; init; }
}