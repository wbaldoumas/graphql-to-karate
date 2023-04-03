namespace GraphQLToKarate.Integration.Api.Types;

public sealed class Query
{
    public User GetUserById([GraphQLType(typeof(IdType))][GraphQLNonNullType] string id) => new()
    {
        Id = id,
        Name = "John Doe",
        Role = UserRole.Administrator,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
}