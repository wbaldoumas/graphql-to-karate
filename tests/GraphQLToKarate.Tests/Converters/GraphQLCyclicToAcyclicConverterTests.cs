using FluentAssertions;
using GraphQLParser;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Extensions;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Converters;

[TestFixture]
internal sealed class GraphQLCyclicToAcyclicConverterTests
{
    private IGraphQLCyclicToAcyclicConverter? _subjectUnderTest;

    [SetUp]
    public void SetUp() => _subjectUnderTest = new GraphQLCyclicToAcyclicConverter();

    private const string GraphQLSchema =
        """
        schema {
          query: Query
        }

        type Query {
          user(id: ID!): User
          post(id: ID!): Post
          comment(id: ID!): Comment
          category(id: ID!): Category
        }

        type User {
          id: ID!
          name: String!
          posts: [Post!]!
          friend: User # Direct cycle: User => User
        }

        type Post {
          id: ID!
          title: String!
          content: String!
          author: User! # Transitive cycle: Post => User => Post
          comments: [Comment!]!
          category: Category
        }

        type Comment {
          id: ID!
          content: String!
          author: User! # Transitive cycle: Comment => User => Post => Comment
          post: Post! # Transitive cycle: Comment => Post => Comment
        }

        # No cycles
        type Category {
          id: ID!
          name: String!
          description: String             
        }
        """;

    private const string GraphQLSchemaWithUnionTypes =
        """
        schema {
          query: Query
        }

        union Entity = User | Post | Comment | Category

        type Query {
          entity(id: ID!, entityType: EntityType!): Entity
        }

        enum EntityType {
          USER
          POST
          COMMENT
          CATEGORY
        }

        type User {
          id: ID!
          name: String!
          posts: [Post!]!
          friend: User # Direct cycle: User => User
        }

        type Post {
          id: ID!
          title: String!
          content: String!
          author: User! # Transitive cycle: Post => User => Post
          comments: [Comment!]!
          category: Category
        }

        type Comment {
          id: ID!
          content: String!
          author: User! # Transitive cycle: Comment => User => Post => Comment
          post: Post! # Transitive cycle: Comment => Post => Comment
        }

        # No cycles
        type Category {
          id: ID!
          name: String!
          description: String             
        }
        """;

    private const string GraphQLSchemaWithNestedUnionTypes =
        """
        schema {
          query: Query
        }

        union Entity = User | Post | Comment | Category

        type Query {        
          nestedEntity(id: ID!): NestedEntity
        }

        enum EntityType {
          USER
          POST
          COMMENT
          CATEGORY
        }

        type User {
          id: ID!
          name: String!
          posts: [Post!]!
          friend: User # Direct cycle: User => User
        }

        type Post {
          id: ID!
          title: String!
          content: String!
          author: User! # Transitive cycle: Post => User => Post
          comments: [Comment!]!
          category: Category
        }

        type Comment {
          id: ID!
          content: String!
          author: User! # Transitive cycle: Comment => User => Post => Comment
          post: Post! # Transitive cycle: Comment => Post => Comment
        }

        # No cycles
        type Category {
          id: ID!
          name: String!
          description: String             
        }

        type NestedEntity {
          id: ID!
          entity: Entity
          relatedEntity: NestedEntity # Direct cycle: NestedEntity => NestedEntity
        }
        """;

    [Test]
    [TestCase(GraphQLSchema)]
    [TestCase(GraphQLSchemaWithUnionTypes)]
    [TestCase(GraphQLSchemaWithNestedUnionTypes)]
    public void Convert_removes_cycles_in_cyclical_field_definition_types(string graphQLSchema)
    {
        // arrange
        var graphQLDocumentAdapter = new GraphQLDocumentAdapter(Parser.Parse(graphQLSchema));

        // act
        foreach (var graphQLFieldDefinition in graphQLDocumentAdapter.GraphQLQueryTypeDefinition!.Fields!)
        {
            _subjectUnderTest!.Convert(graphQLFieldDefinition, graphQLDocumentAdapter);
        }

        // assert
        var user = graphQLDocumentAdapter.GetGraphQLTypeDefinitionWithFields("User");

        AssertContainsField(user!, "id");
        AssertContainsField(user!, "name");
        AssertContainsField(user!, "posts");
        AssertDoesNotContainField(user!, "friend");

        var post = graphQLDocumentAdapter.GetGraphQLTypeDefinitionWithFields("Post");

        AssertContainsField(post!, "id");
        AssertContainsField(post!, "title");
        AssertContainsField(post!, "content");
        AssertContainsField(post!, "comments");
        AssertContainsField(post!, "category");
        AssertDoesNotContainField(post!, "author");

        var comment = graphQLDocumentAdapter.GetGraphQLTypeDefinitionWithFields("Comment");

        AssertContainsField(comment!, "id");
        AssertContainsField(comment!, "content");
        AssertDoesNotContainField(comment!, "author");
        AssertDoesNotContainField(comment!, "post");

        var category = graphQLDocumentAdapter.GetGraphQLTypeDefinitionWithFields("Category");

        AssertContainsField(category!, "id");
        AssertContainsField(category!, "name");
        AssertContainsField(category!, "description");
    }

    private static void AssertContainsField(IHasFieldsDefinitionNode hasFieldsDefinitionNode, string fieldName) =>
        hasFieldsDefinitionNode.Fields!.Items
            .Should()
            .Contain(graphQLFieldDefinition =>
                graphQLFieldDefinition.NameValue().Equals(fieldName, StringComparison.OrdinalIgnoreCase)
            );

    private static void AssertDoesNotContainField(IHasFieldsDefinitionNode hasFieldsDefinitionNode, string fieldName) =>
        hasFieldsDefinitionNode.Fields!.Items
            .Should()
            .NotContain(graphQLFieldDefinition =>
                graphQLFieldDefinition.NameValue().Equals(fieldName, StringComparison.OrdinalIgnoreCase)
            );
}