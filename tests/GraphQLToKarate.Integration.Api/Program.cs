using GraphQLToKarate.Integration.Api.Types;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddType<DateTime>()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddType<UserRole>()
    .AddType<Node>()
    .AddType<User>()
    .AddType<BlogPost>()
    .AddType<Comment>()
    .AddType<PageInfo>()
    .AddType<UserConnection>()
    .AddType<BlogPostConnection>()
    .AddType<CommentConnection>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGraphQL();

app.Run();