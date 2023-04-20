namespace GraphQLToKarate.Integration.Api.Types;

public class CreateBlogPostInput
{
    public required string Title { get; init; }

    public required string Content { get; init; }

    public required CreateUserInput? Author { get; init; }
}