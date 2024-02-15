using GraphQLParser.AST;
using GraphQLToKarate.Library.Enums;
using GraphQLToKarate.Library.Extensions;

namespace GraphQLToKarate.Library.Types;

public sealed class GraphQLOperation(GraphQLFieldDefinition graphQLFieldDefinition)
{
    public string Name => graphQLFieldDefinition.NameValue();

    public string OperationName => $"{Name.FirstCharToUpper()}Test";

    public GraphQLType ReturnType => graphQLFieldDefinition.Type;

    public string ReturnTypeName => graphQLFieldDefinition.Type.GetUnwrappedTypeName();

    public bool IsListReturnType => graphQLFieldDefinition.Type.IsListType();

    public required string OperationString { get; init; }

    public required GraphQLOperationType Type { get; init; }

    public required ICollection<GraphQLArgumentTypeBase> Arguments { get; init; }
}
