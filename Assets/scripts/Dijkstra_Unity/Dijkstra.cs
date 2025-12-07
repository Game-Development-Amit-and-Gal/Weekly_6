using System.Collections.Generic;
using System;

public static class Dijkstra // Static class implementing Dijkstra's algorithm for shortest pathfinding
{
    /// <summary>
    /// Computes shortest path based on weight (Dijkstra).
    /// </summary>
    public static List<NodeType> GetPath<NodeType>( // Generic method to get the shortest path between (start, end) nodes in a weighted graph
        IGraph<NodeType> graph,
        NodeType start, NodeType end
    ) where NodeType : notnull
    {
        var prev = new Dictionary<NodeType, NodeType>(); // to reconstruct path
        var dist = new Dictionary<NodeType, int>(); // distances from start
        var visited = new HashSet<NodeType>(); // visited nodes

        // initialize all distances to infinity
        foreach (var node in graph.AllNodes())
            dist[node] = int.MaxValue;

        dist[start] = 0; // delta((start,start)) = 0

        while (true) // main loop
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

                int newDist = dist[current] + weight; // relax edge
                if (newDist < dist[neighbor]) // found a better path
                {
                    dist[neighbor] = newDist; // update distance
                    prev[neighbor] = current; // update path PI[neighbor] = current related to Algo 1 Course.
                }
            }
        }

        // reconstruct path
        var path = new List<NodeType>();
        if (!prev.ContainsKey(end) && !EqualityComparer<NodeType>.Default.Equals(start, end)) // no path found
            return path; // unreachable

        for (var at = end; at != null && prev.ContainsKey(at); at = prev[at]) // backtrack from end to start Using PI
            path.Add(at);

        path.Add(start); // add start node
        path.Reverse(); // reverse to get correct order from start to end
        return path;
    }
}
