using System.Collections.Generic;

public interface IGraph<NodeType> // Generic interface for a weighted graph
{
    IEnumerable<(NodeType node, int weight)> Neighbors(NodeType node); // Method to get neighboring nodes and their weights
    IEnumerable<NodeType> AllNodes(); // Method to get all valid nodes in the graph
}
