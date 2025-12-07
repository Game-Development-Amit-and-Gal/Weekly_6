using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Pure BFS helper: computes the next tile toward the player.
/// DOES NOT move the enemy by itself.
/// </summary>
public class EnemyChaseBFS : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private List<TileBase> allowed = new List<TileBase>();

    // Directions (no magic values)
    private static readonly Vector3Int Up = new Vector3Int(0, 1, 0);
    private static readonly Vector3Int Down = new Vector3Int(0, -1, 0);
    private static readonly Vector3Int Left = new Vector3Int(-1, 0, 0);
    private static readonly Vector3Int Right = new Vector3Int(1, 0, 0);
    private static readonly Vector3Int[] Directions = { Up, Down, Left, Right };



    /// <summary>
    /// Returns the next world position (one tile) toward the player.
    /// If no path exists, returns current position.
    /// </summary>
    /// 


    public Tilemap getTile => tilemap; // Getter for the tilemap
    public Vector3 GetNextStepTowardsPlayer() // Method to get the next step towards the player using BFS
    {
        Vector3Int start = tilemap.WorldToCell(transform.position); // Convert enemy's world position to tilemap cell
        Vector3Int target = tilemap.WorldToCell(player.position); // Convert player's world position to tilemap cell

        if (start == target) // Already at player
            return transform.position; // Return current position

        var queue = new Queue<Vector3Int>(); // Queue for BFS
        var cameFrom = new Dictionary<Vector3Int, Vector3Int>(); // To reconstruct the path

        queue.Enqueue(start); // Enqueue starting position
        cameFrom[start] = start; // Mark start as visited

        while (queue.Count > 0) // BFS loop
        {
            Vector3Int current = queue.Dequeue(); // Dequeue the next position

            foreach (Vector3Int dir in Directions) // Explore each direction
            {
                Vector3Int next = current + dir; // Calculate the next position

                if (!cameFrom.ContainsKey(next) && IsWalkable(next)) // If not visited and walkable
                {
                    cameFrom[next] = current; // Mark as visited
                    queue.Enqueue(next); // Enqueue the next position

                    if (next == target) // Reached the target
                        return ReconstructFirstStep(start, target, cameFrom); //    Reconstruct and return the first step
                }
            }
        }

        // Player unreachable
        return transform.position;
    }

    private Vector3 ReconstructFirstStep(
        Vector3Int start,
        Vector3Int target,
        Dictionary<Vector3Int, Vector3Int> cameFrom) // Method to reconstruct the first step from start to target using the cameFrom dictionary
    {
        Vector3Int current = target; // Start from the target

        while (cameFrom[current] != start) // Backtrack until reaching the start position  
            current = cameFrom[current]; // Move to the previous position

        return tilemap.GetCellCenterWorld(current); // Return the world position of the first step
    }

    private bool IsWalkable(Vector3Int cell) // Method to check if a tile at a given cell is walkable
    {
        TileBase t = tilemap.GetTile(cell); // Get the tile at the specified cell
        Debug.Log("Checking tile: " + t + " at " + cell); // Debug log to check the tile being evaluated

        return allowed.Contains(t); // Return true if the tile is in the allowed list
    }

}
