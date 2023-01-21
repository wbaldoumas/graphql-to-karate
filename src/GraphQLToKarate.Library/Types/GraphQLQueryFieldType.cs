using GraphQLParser.AST;
using GraphQLToKarate.Library.Extensions;

namespace GraphQLToKarate.Library.Types;

public sealed class GraphQLQueryFieldType
{
    private readonly GraphQLFieldDefinition _queryField;

    public string Name => _queryField.Name.StringValue;

    public string ReturnTypeName => _queryField.Type.GetTypeName();

    public required string QueryString { get; init; }

    public required ICollection<GraphQLArgumentTypeBase> Arguments { get; init; }

    public GraphQLQueryFieldType(GraphQLFieldDefinition queryField) => _queryField = queryField;
}
