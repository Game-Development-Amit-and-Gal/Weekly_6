using System.Collections.Generic;

public interface IGraph<NodeType>
{
    IEnumerable<(NodeType node, int weight)> Neighbors(NodeType node);
    IEnumerable<NodeType> AllNodes();
}
