using GraphQLParser.AST;
using GraphQLToKarate.Library.Extensions;

namespace GraphQLToKarate.Library.Types;

public sealed class GraphQLQueryFieldType
{
    private readonly GraphQLFieldDefinition _queryField;

    public string Name => _queryField.NameValue();

    public string OperationName => $"{Name.FirstCharToUpper()}Test";

    public string ReturnTypeName => _queryField.Type.GetUnwrappedTypeName();

    public bool IsNullableReturnType => _queryField.Type is not GraphQLNonNullType;

    public bool IsListReturnType => _queryField.Type.IsListType();

    public required string QueryString { get; init; }

    public required ICollection<GraphQLArgumentTypeBase> Arguments { get; init; }

    public GraphQLQueryFieldType(GraphQLFieldDefinition queryField) => _queryField = queryField;
}
