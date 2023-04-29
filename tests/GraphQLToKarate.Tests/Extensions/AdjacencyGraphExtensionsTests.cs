using FluentAssertions;
using GraphQLToKarate.Library.Extensions;
using NUnit.Framework;
using QuikGraph;

namespace GraphQLToKarate.Tests.Extensions;

[TestFixture]
internal sealed class AdjacencyGraphExtensionsTests
{
    [TestCaseSource(nameof(IsCyclicTestCases))]
    public void IsCyclicGraph_should_return_expected_result(AdjacencyGraph<int, Edge<int>> graph, bool expectedResult) =>
        graph.IsCyclicGraph().Should().Be(expectedResult);

    private static IEnumerable<TestCaseData> IsCyclicTestCases
    {
        get
        {
            yield return new TestCaseData(
                new AdjacencyGraph<int, Edge<int>>(),
                false
            ).SetName("Empty graph should be non-cyclic");

            yield return new TestCaseData(
                CreateSingleVertexGraph(),
                false
            ).SetName("Single vertex graph should be non-cyclic");

            yield return new TestCaseData(
                CreateCyclicGraph(),
                true
            ).SetName("Cyclic graph should be cyclic");

            yield return new TestCaseData(
                CreateNonCyclicGraph(),
                false
            ).SetName("Non-cyclic graph should be non-cyclic");
        }
    }

    private static AdjacencyGraph<int, Edge<int>> CreateSingleVertexGraph()
    {
        var graph = new AdjacencyGraph<int, Edge<int>>(true);

        graph.AddVertex(1);

        return graph;
    }

    private static AdjacencyGraph<int, Edge<int>> CreateCyclicGraph()
    {
        var graph = new AdjacencyGraph<int, Edge<int>>(true);

        graph.AddVerticesAndEdgeRange(new[]
        {
            new Edge<int>(1, 2),
            new Edge<int>(2, 3),
            new Edge<int>(3, 1)
        });

        return graph;
    }

    private static AdjacencyGraph<int, Edge<int>> CreateNonCyclicGraph()
    {
        var graph = new AdjacencyGraph<int, Edge<int>>(true);

        graph.AddVerticesAndEdgeRange(new[]
        {
            new Edge<int>(1, 2),
            new Edge<int>(2, 3),
            new Edge<int>(3, 4)
        });

        return graph;
    }
}