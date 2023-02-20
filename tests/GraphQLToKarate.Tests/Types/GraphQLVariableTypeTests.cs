using FluentAssertions;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Types;

[TestFixture]
internal sealed class GraphQLVariableTypeTests
{
    [Test]
    [TestCaseSource(nameof(TestCases))]
    public void GraphQLVariableType_Schema_produces_expected_output(
        GraphQLArgumentTypeBase graphQLArgumentTypeBase,
        string expectedSchema
    ) => graphQLArgumentTypeBase.VariableTypeName.Should().Be(expectedSchema);

    private static IEnumerable<TestCaseData> TestCases
    {
        get
        {
            var graphQLVariableType = new GraphQLArgumentType("id", "id", GraphQLToken.String, "\"an example value\"");

            yield return new TestCaseData(
                graphQLVariableType,
                GraphQLToken.String
            );

            yield return new TestCaseData(
                new GraphQLNonNullArgumentType(graphQLVariableType),
                $"{GraphQLToken.String}{GraphQLToken.NonNull}"
            );

            yield return new TestCaseData(
                new GraphQLListArgumentType(graphQLVariableType),
                $"{SchemaToken.OpenBracket}{GraphQLToken.String}{SchemaToken.CloseBracket}"
            );

            yield return new TestCaseData(
                new GraphQLNonNullArgumentType(
                    new GraphQLListArgumentType(
                        new GraphQLNonNullArgumentType(
                            graphQLVariableType
                        )
                    )
                ),
                $"{SchemaToken.OpenBracket}{GraphQLToken.String}{GraphQLToken.NonNull}{SchemaToken.CloseBracket}{GraphQLToken.NonNull}"
            );
        }
    }
}