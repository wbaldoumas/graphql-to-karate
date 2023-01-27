using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Features;
using GraphQLToKarate.Library.Parsers;

const string graphQLSchema = """
    interface FooInterface {
        id: String!
        name: String!
        completed: Boolean
        color: Color
        colors(filter: [Color!]!): [Color!]!
    }

    type Todo implements FooInterface {
        id: String!
        name: String!
        completed: Boolean
        color: Color

        "A field that requires an argument"
        colors(filter: [Color!]!): [Color!]!
    }

    type SimpleTodo {
      id: String!
      name: String!
      stuff: [String!]
    }

    type NestedTodo {
      id: String!
      todos: [FooInterface!]!
    }

    type NestedNestedTodo {
      id: String!
      todos: [[NestedTodo!]]!
    }

    type MoreNesting {
      id: String!
      name: String!
      colors: [Color!]!
      nestedTodos: [NestedTodo!]!
    }

    union TodoUnion = Todo | SimpleTodo

    input TodoInputType {
        name: String!
        completed: Boolean
        color: Color=RED
    }

    enum Color {
        "Red color"
        RED
        "Green color"
        GREEN
    }

    type Query {
        "A Query with 1 required argument and 1 optional argument"
        todo(
          id: String!,
          "A default value of false"
          isCompleted: Boolean=false
        ): FooInterface!

        "Returns a list (or null) that can contain null values"
        todos(
          "Required argument that is a list that cannot contain null values"
          ids: [String!]!
        ): [FooInterface!]!

        nestedTodos(
          ids: [String!]!
        ): [[NestedTodo]]

        moreNestedTodos(
          ids: [String!]!
        ): [[MoreNesting]]
    }

    type Mutation {
        "A Mutation with 1 required argument"
        create_todo(
          todo: TodoInputType!
        ): Todo!

        "A Mutation with 2 required arguments"
        update_todo(
          id: String!,
          data: TodoInputType!
        ): Todo!

        "Returns a list (or null) that can contain null values"
        update_todos(
          ids: [String!]!
          data: TodoInputType!
        ): [Todo]
    }
    """;

var graphQLToKarateConverter = new GraphQLToKarateConverter(
    new GraphQLSchemaParser(),
    new GraphQLTypeDefinitionConverter(
        new GraphQLTypeConverterFactory()
    ),
    new GraphQLFieldDefinitionConverter(
        new GraphQLInputValueDefinitionConverterFactory()
    ),
    new KarateFeatureBuilder(
        new KarateScenarioBuilder()
    )
);

Console.WriteLine(graphQLToKarateConverter.Convert(graphQLSchema));

Console.WriteLine("Done! Press enter to exit...");
Console.ReadLine();