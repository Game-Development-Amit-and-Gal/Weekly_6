using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Stores movement cost for each tile on a Tilemap,
/// used for weighted pathfinding (Dijkstra / A*).
/// </summary>
public class TileCost : MonoBehaviour
{
    // --- REFERENCES -------------------------

    /// <summary>
    /// Tilemap this script reads tiles from.
    /// Used to convert player/enemy positions into tile coordinates.
    /// </summary>
    [SerializeField] private Tilemap tilemap;

    // --- SERIALIZABLE COST TABLE ------------
    /// <summary>
    /// Helper class required because Unity cannot serialize Dictionary directly.
    /// Each entry pairs a tile sprite with a movement cost.
    /// </summary>
    [System.Serializable]
    public class CostEntry
    {
        public TileBase tile;
        [Tooltip("Higher = slower movement. Very high = almost blocked.")]
        public int cost = 1;
    }

    /// <summary>
    /// List visible in Inspector where designers assign costs to tiles.
    /// </summary>
    [SerializeField] private List<CostEntry> tileCosts = new List<CostEntry>();

    // --- RUNTIME LOOKUP TABLE ----------------
    /// <summary>
    /// Dictionary version of tileCosts. Fast lookup during gameplay.
    /// </summary>
    private Dictionary<TileBase, int> costMap;

    // --- UNITY LIFECYCLE ---------------------
    private void Awake()
    {
        // Initialize dictionary
        costMap = new Dictionary<TileBase, int>();

        // Fill dictionary from inspector list
        foreach (var entry in tileCosts)
        {
            // Skip null tiles and duplicates
            if (entry.tile != null && !costMap.ContainsKey(entry.tile))
            {
                costMap.Add(entry.tile, entry.cost);
            }
        }
    }

    // --- PUBLIC API --------------------------
    /// <summary>
    /// Returns the movement cost of the tile located at a given Tilemap cell.
    /// </summary>
    /// <param name="worldCell">Tile cell in grid coordinates (Tilemap.WorldToCell).</param>
    /// <returns>Movement cost. Returns int.MaxValue if tile is unknown (treated as blocked).</returns>
    public int GetTileCost(Vector3Int worldCell)
    {
        TileBase tile = tilemap.GetTile(worldCell);

        // Try to find cost in dictionary
        if (tile != null && costMap.TryGetValue(tile, out int cost))
            return cost;

        // Unknown tile -> treat as extremely expensive / unreachable
        return int.MaxValue;
    }
}
