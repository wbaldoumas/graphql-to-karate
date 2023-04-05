using GraphQLToKarate.Integration.Api.Types;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddType<DateTime>()
    .AddQueryType<Query>()
    .AddType<UserRole>()
    .AddType<Node>()
    .AddType<User>()
    .AddType<BlogPost>()
    .AddType<Comment>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGraphQL();

app.Run();