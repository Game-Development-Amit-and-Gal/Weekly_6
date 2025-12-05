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

    public TilemapGraph(Tilemap m, TileCost cost)
    {
        map = m;
        costProvider = cost;
    }

    /// <summary>
    /// Returns neighbors of a cell with their movement cost.
    /// </summary>
    public IEnumerable<(Vector3Int node, int weight)> Neighbors(Vector3Int node)
    {
        foreach (var d in DIRS)
        {
            Vector3Int next = node + d;
            TileBase tile = map.GetTile(next);

            // Ignore outside map
            if (tile == null) continue;

            yield return (next, costProvider.GetTileCost(next));
        }
    }

    /// <summary>
    /// Returns all valid walkable tiles on the map.
    /// Needed by Dijkstra initialization.
    /// </summary>
    public IEnumerable<Vector3Int> AllNodes()
    {
        foreach (var pos in map.cellBounds.allPositionsWithin)
        {
            if (map.HasTile(pos))
                yield return pos;
        }
    }
}
