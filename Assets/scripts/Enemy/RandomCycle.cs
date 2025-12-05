using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Generates 3 random valid grass positions for spawning enemies.
/// </summary>
public class RandomEnemySpawnPoints : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private List<TileBase> allowed = new List<TileBase>();

    [SerializeField] private int spawnCount = 3;

    /// <summary>
    /// Returns a list of world positions (Vector3) for enemy spawn.
    /// </summary>
    public List<Vector3> GetRandomSpawns()
    {
        List<Vector3Int> validCells = new List<Vector3Int>();
        List<Vector3> result = new List<Vector3>();

        BoundsInt bounds = tilemap.cellBounds;

        // Collect all grass cells
        foreach (var pos in bounds.allPositionsWithin)
        {
            TileBase t = tilemap.GetTile(pos);
            if (allowed.Contains(t))
                validCells.Add(pos);
        }

        // Pick N random unique cells
        for (int i = 0; i < spawnCount; i++)
        {
            if (validCells.Count == 0) break;

            int index = Random.Range(0, validCells.Count);
            Vector3Int cell = validCells[index];

            result.Add(tilemap.GetCellCenterWorld(cell));

            validCells.RemoveAt(index);
        }

        return result;
    }
}
