using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Wraps a Unity Tilemap and TileCost into a weighted graph usable by Dijkstra.
/// </summary>
public class TilemapGraph : IGraph<Vector3Int>
{
    private Tilemap map;
    private TileCost costProvider;

    // 4 possible movement directions (grid neighbors)
    private static readonly Vector3Int[] DIRS =
    {
        new Vector3Int(1,0,0),
        new Vector3Int(-1,0,0),
        new Vector3Int(0,1,0),
        new Vector3Int(0,-1,0)
    };

    public TilemapGraph(Tilemap m, TileCost cost) // Constructor to initialize the graph with a Tilemap and TileCost
    {
        map = m;
        costProvider = cost;
    }

    /// <summary>
    /// Returns neighbors of a cell with their movement cost.
    /// </summary>
    public IEnumerable<(Vector3Int node, int weight)> Neighbors(Vector3Int node) // IEnumerable method to get neighboring nodes and their weights
    {
        foreach (var d in DIRS) // traverse each possible direction
        {
            Vector3Int next = node + d; // calculate the neighbor position
            TileBase tile = map.GetTile(next); // get the tile at the neighbor position

            // Ignore outside map
            if (tile == null) continue;

            yield return (next, costProvider.GetTileCost(next)); // Using Yeild in order to return each neighbor and its cost (neighbor,weight)
        }
    }

    /// <summary>
    /// Returns all valid walkable tiles on the map.
    /// Needed by Dijkstra initialization.
    /// </summary>
    public IEnumerable<Vector3Int> AllNodes() // IEnumerable method to get all valid nodes in the graph
    {
        foreach (var pos in map.cellBounds.allPositionsWithin) // traverse all positions within the tilemap bounds
        {
            if (map.HasTile(pos))// check if there is a tile at the position
                yield return pos;
        }
    }
}
