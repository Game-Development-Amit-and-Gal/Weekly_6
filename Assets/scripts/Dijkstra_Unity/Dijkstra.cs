using System.Collections.Generic;
using System;

public static class Dijkstra
{
    /// <summary>
    /// Computes shortest path based on weight (Dijkstra).
    /// </summary>
    public static List<NodeType> GetPath<NodeType>(
        IGraph<NodeType> graph,
        NodeType start, NodeType end
    ) where NodeType : notnull
    {
        var prev = new Dictionary<NodeType, NodeType>();
        var dist = new Dictionary<NodeType, int>();
        var visited = new HashSet<NodeType>();

        // initialize all distances to infinity
        foreach (var node in graph.AllNodes())
            dist[node] = int.MaxValue;

        dist[start] = 0;

        while (true)
        {
            NodeType current = default;
            int bestDist = int.MaxValue;

            // pick lowest-distance unvisited node
            //Instead of Using a priority queue which is not available in Unity, we do a linear scan here.
            foreach (var kv in dist)
            {
                if (!visited.Contains(kv.Key) && kv.Value < bestDist)
                {
                    bestDist = kv.Value;
                    current = kv.Key;
                }
            }

            // No reachable nodes or we reached the target
            //EqualityComparer<NodeType>.Default.Equals is used to handle generic type comparison
            if (current == null || EqualityComparer<NodeType>.Default.Equals(current, end))
                break;

            visited.Add(current);

            // check neighbors
            foreach (var (neighbor, weight) in graph.Neighbors(current))
            {
                if (visited.Contains(neighbor)) continue;

                int newDist = dist[current] + weight;
                if (newDist < dist[neighbor])
                {
                    dist[neighbor] = newDist;
                    prev[neighbor] = current;
                }
            }
        }

        // reconstruct path
        var path = new List<NodeType>();
        if (!prev.ContainsKey(end) && !EqualityComparer<NodeType>.Default.Equals(start, end))
            return path; // unreachable

        for (var at = end; at != null && prev.ContainsKey(at); at = prev[at])
            path.Add(at);

        path.Add(start);
        path.Reverse();
        return path;
    }
}
