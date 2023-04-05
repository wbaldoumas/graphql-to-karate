namespace GraphQLToKarate.Integration.Api.Types;

public sealed class Mutation
{
    public User CreateUser(CreateUserInput input) => new()
    {
        Id = Guid.NewGuid().ToString(),
        Name = input.Name,
        Role = input.Role,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
    };

    public User UpdateUser(UpdateUserInput input) => new()
    {
        Id = input.Id,
        Name = input.Name,
        Role = input.Role,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
    };

    public bool DeleteUser([GraphQLType(typeof(IdType))][GraphQLNonNullType] string id) => true;
}