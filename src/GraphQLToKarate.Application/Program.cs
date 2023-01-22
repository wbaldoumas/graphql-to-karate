using GraphQLToKarate.Library;
using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Features;

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

var converter = new Converter(
    new GraphQLTypeDefinitionConverter(new GraphQLTypeConverterFactory()),
    new GraphQLFieldDefinitionConverter()
);

var (karateObjects, graphQLQueryFields) = converter.Convert(graphQLSchema);

var scenarioBuilder = new ScenarioBuilder();
var featureBuilder = new FeatureBuilder(scenarioBuilder);

Console.WriteLine(featureBuilder.Build(karateObjects, graphQLQueryFields));

Console.WriteLine("Done! Press enter to exit...");
Console.ReadLine();