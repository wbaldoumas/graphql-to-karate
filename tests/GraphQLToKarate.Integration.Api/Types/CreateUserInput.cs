namespace GraphQLToKarate.Integration.Api.Types;

public sealed class CreateUserInput
{
    public required string Name { get; init; }

    public required string Password { get; init; }

    public required UserRole Role { get; init; }

    public required CreateBlogPostInput BlogPost { get; init; }
}