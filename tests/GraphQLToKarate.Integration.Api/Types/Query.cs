namespace GraphQLToKarate.Integration.Api.Types;

public sealed class Query
{
    public User GetUserById([GraphQLType(typeof(IdType))] [GraphQLNonNullType] string id) => new()
    {
        Id = id,
        Name = "John Doe",
        Role = UserRole.Administrator,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    public BlogPost GetBlogPostById([GraphQLType(typeof(IdType))] [GraphQLNonNullType] string id) => new()
    {
        Id = id,
        Title = "GraphQL to Karate",
        Content = "GraphQL to Karate is a tool that converts GraphQL schemas to Karate tests.",
        Author = GetUserById(Guid.NewGuid().ToString()),
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    public Comment GetCommentById([GraphQLType(typeof(IdType))] [GraphQLNonNullType] string id) => new()
    {
        Id = id,
        Content = "GraphQL to Karate is a tool that converts GraphQL schemas to Karate tests.",
        Author = GetUserById(Guid.NewGuid().ToString()),
        BlogPost = GetBlogPostById(Guid.NewGuid().ToString()),
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    public UserConnection GetUsers([GraphQLType(typeof(PageInfoInput))] PageInfoInput? pageInfo) => new()
    {
        Nodes = new[]
        {
            GetUserById(Guid.NewGuid().ToString()),
            GetUserById(Guid.NewGuid().ToString()),
            GetUserById(Guid.NewGuid().ToString()),
            GetUserById(Guid.NewGuid().ToString()),
            GetUserById(Guid.NewGuid().ToString())
        },
        PageInfo = new PageInfo
        {
            HasNextPage = true,
            HasPreviousPage = false,
            TotalCount = 5
        }
    };

    public BlogPostConnection GetBlogPosts([GraphQLType(typeof(PageInfoInput))] PageInfoInput? pageInfo) => new()
    {
        Nodes = new[]
        {
            GetBlogPostById(Guid.NewGuid().ToString()),
            GetBlogPostById(Guid.NewGuid().ToString()),
            GetBlogPostById(Guid.NewGuid().ToString()),
            GetBlogPostById(Guid.NewGuid().ToString()),
            GetBlogPostById(Guid.NewGuid().ToString()),
        },
        PageInfo = new PageInfo
        {
            HasNextPage = true,
            HasPreviousPage = false,
            TotalCount = 5
        }
    };

    public CommentConnection GetComments([GraphQLType(typeof(PageInfoInput))] PageInfoInput? pageInfo) => new()
    {
        Nodes = new[]
        {
            GetCommentById(Guid.NewGuid().ToString()),
            GetCommentById(Guid.NewGuid().ToString()),
            GetCommentById(Guid.NewGuid().ToString()),
            GetCommentById(Guid.NewGuid().ToString()),
            GetCommentById(Guid.NewGuid().ToString())
        },
        PageInfo = new PageInfo
        {
            HasNextPage = true,
            HasPreviousPage = false,
            TotalCount = 5
        }
    };
}