using GraphQLToKarate.Library;
using GraphQLToKarate.Library.Converters;

var graphql = """
    type Todo {
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
      todos: [Todo!]!
    }


    type NestedNestedTodo {
      id: String!
      todos: [[Todo!]]!
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
        ): Todo!

        "Returns a list (or null) that can contain null values"
        todos(
          "Required argument that is a list that cannot contain null values"
          ids: [String!]!
        ): [Todo]

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
    new GraphQLObjectTypeDefinitionConverter(new GraphQLTypeConverterFactory()),
    new GraphQLQueryFieldConverter()
);

var (karateObjects, graphQLQueryFields) = converter.Convert(graphql);

foreach (var graphQLQueryField in graphQLQueryFields)
{
    Console.WriteLine($"{graphQLQueryField.Name}: {graphQLQueryField.ReturnTypeName}");
    Console.WriteLine(graphQLQueryField.QueryString);
    Console.WriteLine();
}

foreach (var karateObject in karateObjects)
{
    Console.WriteLine(karateObject.Name);
    Console.WriteLine(karateObject);
    Console.WriteLine();
}

Console.WriteLine("Done! Press enter to exit...");
Console.ReadLine();