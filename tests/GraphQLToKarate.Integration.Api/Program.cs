using GraphQLToKarate.Integration.Api.Types;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddType<Node>()
    .AddType<User>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGraphQL();

app.Run();