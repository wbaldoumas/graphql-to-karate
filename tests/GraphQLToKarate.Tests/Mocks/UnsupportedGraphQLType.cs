using System.Diagnostics.CodeAnalysis;
using GraphQLParser.AST;

namespace GraphQLToKarate.Tests.Mocks;

[ExcludeFromCodeCoverage]
internal sealed class UnsupportedGraphQLType : GraphQLType
{
    public override ASTNodeKind Kind => ASTNodeKind.Alias;
}