using GraphQLParser.AST;
using System.Diagnostics.CodeAnalysis;

namespace GraphQLToKarate.Tests.Mocks;

[ExcludeFromCodeCoverage]
internal sealed class UnsupportedGraphQLType : GraphQLType
{
    public override ASTNodeKind Kind => ASTNodeKind.Alias;
}