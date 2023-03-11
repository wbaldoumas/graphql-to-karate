using GraphQLParser.AST;
using GraphQLToKarate.Library.Enums;
using GraphQLToKarate.Library.Extensions;

namespace GraphQLToKarate.Library.Types;

public sealed class GraphQLOperation
{
    private readonly GraphQLFieldDefinition _graphQLFieldDefinition;

    public string Name => _graphQLFieldDefinition.NameValue();

    public string OperationName => $"{Name.FirstCharToUpper()}Test";

    public string ReturnTypeName => _graphQLFieldDefinition.Type.GetUnwrappedTypeName();

    public bool IsNullableReturnType => _graphQLFieldDefinition.Type is not GraphQLNonNullType;

    public bool IsListReturnType => _graphQLFieldDefinition.Type.IsListType();

    public required string OperationString { get; init; }

    public required GraphQLOperationType Type { get; init; }

    public required ICollection<GraphQLArgumentTypeBase> Arguments { get; init; }

    public GraphQLOperation(GraphQLFieldDefinition graphQLFieldDefinition) => _graphQLFieldDefinition = graphQLFieldDefinition;
}
