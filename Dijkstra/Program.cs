using System.Collections.Generic;
using System;

public class SimpleGraph : IGraph<string>
{
    private readonly Dictionary<string, List<(string node, int weight)>> edges =
        new Dictionary<string, List<(string node, int weight)>>()
        {
            ["A"] = new() { ("B", 1), ("C", 4) },
            ["B"] = new() { ("C", 2), ("D", 5) },
            ["C"] = new() { ("D", 1) },
            ["D"] = new()
        };

    public IEnumerable<(string node, int weight)> Neighbors(string node)
    {
        return edges[node];
    }

    public IEnumerable<string> AllNodes()
    {
        return edges.Keys;
    }

    public int GetWeight(string from, string to)
{
    if (!edges.ContainsKey(from))
        return int.MaxValue;

    foreach (var (node, weight) in edges[from])
    {
        if (node == to)
            return weight;
    }

    return int.MaxValue; // unreachable
}

}

public class DijkstraTest
{
    public static void Main()
    {
        // Create test graph
        var graph = new SimpleGraph();

        // Run Dijkstra from A → D
 var path = Dijkstra.GetPath(graph, "A", "D");

// Print nodes with weights
Console.WriteLine("Shortest path A → D:");
int totalWeight = 0;
for (int i = 0; i < path.Count; i++)
{
    Console.Write(path[i]);

    if (i < path.Count - 1)
    {
        int w = graph.GetWeight(path[i], path[i + 1]);
        Console.Write($" --({w})--> ");
        totalWeight += w;
    }
}
Console.WriteLine($"\nTotal Weight = {totalWeight}\n");

    }
}
