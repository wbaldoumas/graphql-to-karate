namespace GraphQLToKarate.Integration.Api.Types;

public sealed class UpdateUserInput
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public required UserRole Role { get; init; }
}