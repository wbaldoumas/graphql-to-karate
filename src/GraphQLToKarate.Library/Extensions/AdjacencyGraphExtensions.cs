using QuikGraph;
using QuikGraph.Algorithms;

namespace GraphQLToKarate.Library.Extensions;

/// <summary>
///     Extension methods for <see cref="AdjacencyGraph{TVertex,TEdge}"/>.
/// </summary>
internal static class AdjacencyGraphExtensions
{
    /// <summary>
    ///    Determines whether the given <see cref="AdjacencyGraph{TVertex,TEdge}"/> is cyclic.
    /// </summary>
    /// <typeparam name="TVertex">The vertex type.</typeparam>
    /// <typeparam name="TEdge">The edge type.</typeparam>
    /// <param name="adjacencyGraph">The <see cref="AdjacencyGraph{TVertex,TEdge}"/> to check.</param>
    /// <returns><c>true</c> if the given <see cref="AdjacencyGraph{TVertex,TEdge}"/> is cyclic; otherwise, <c>false</c>.</returns>
    public static bool IsCyclic<TVertex, TEdge>(this AdjacencyGraph<TVertex, TEdge> adjacencyGraph)
        where TEdge : IEdge<TVertex> => !adjacencyGraph.IsDirectedAcyclicGraph();
}