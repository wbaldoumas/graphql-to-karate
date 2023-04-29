namespace GraphQLToKarate.Integration.Api.Types;

[ExtendObjectType(typeof(BlogPost))]
public sealed class BlogPostExtensions
{
    public required Uri Uri { get; set;  } = new ("https://my-awesome-api.com");
}