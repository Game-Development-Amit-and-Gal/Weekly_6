using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Generates 3 random valid grass positions for spawning enemies.
/// </summary>
public class RandomEnemySpawnPoints : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;           // Reference to the Tilemap component
    [SerializeField] private List<TileBase> allowed = new List<TileBase>();     // List of allowed grass tiles

    [SerializeField] private int spawnCount = 3;            // Number of spawn points to generate

    /// <summary>
    /// Returns a list of world positions (Vector3) for enemy spawn.
    /// </summary>
    public List<Vector3> GetRandomSpawns()          // Method to get random spawn positions
    {
        List<Vector3Int> validCells = new List<Vector3Int>();       // List to hold valid grass cell positions
        List<Vector3> result = new List<Vector3>();                 // List to hold final spawn positions

        BoundsInt bounds = tilemap.cellBounds;                      // Get the bounds of the tilemap

        // Collect all grass cells
        foreach (var pos in bounds.allPositionsWithin)              // Iterate through all positions within the bounds
        {
            TileBase t = tilemap.GetTile(pos);                      // Get the tile at the current position
            if (allowed.Contains(t))                                // Check if the tile is in the allowed list
                validCells.Add(pos);                                // Add valid grass cell position to the list
        }

        // Pick N random unique cells
        for (int i = 0; i < spawnCount; i++)
        {
            int zero = 0;                       // To avoid magic number
            if (validCells.Count == zero) break;           // Break if no more valid cells are available

            int index = Random.Range(zero, validCells.Count);              // Pick a random index
            Vector3Int cell = validCells[index];                        // Get the cell at the random index

            result.Add(tilemap.GetCellCenterWorld(cell));

            validCells.RemoveAt(index);
        }

        return result;
    }
}
