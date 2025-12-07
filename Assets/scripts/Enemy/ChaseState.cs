using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Chase behavior for an enemy.
/// Uses BFS to compute the next step toward the player.
/// Enabled when the StateMachine switches into chase mode.
/// </summary>
public class ChaseState : MonoBehaviour
{
    /// <summary>Movement speed during chase.</summary>
    [SerializeField] private float chaseSpeed = 3f;

    /// <summary>Reference to the player's transform used as a chase target.</summary>
    [SerializeField] private Transform player = null;

    /// <summary>BFS component providing the next step toward the player.</summary>
    [SerializeField] private EnemyChaseBFS bfs = null;

    /// <summary>Tilemap used to convert world positions to grid cells.</summary>
    [SerializeField] private Tilemap tilemap = null;

    /// <summary>Scaling factor for delta-time movement.</summary>
    [SerializeField] private float timeFactor = 1f;
    private Vector3 lastStep; // To track the last step taken

    private void Update() 
    {
        if (player == null || bfs == null || tilemap == null) // Safety check
        {
            return;
        }

        Vector3 next = bfs.GetNextStepTowardsPlayer();

        // Only move if BFS returned a NEW TILE
        if (next != transform.position)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                next,
                chaseSpeed * Time.deltaTime
            ); // Move towards the next step

            lastStep = next; // Update last step
        }

        // --- 2. Check if enemy and player are on the SAME TILE ---
        Vector3Int enemyCell = tilemap.WorldToCell(transform.position); // Current cell of the enemy
        Vector3Int playerCell = tilemap.WorldToCell(player.position); // Current cell of the player


        Vector3Int currentCell2 = bfs.getTile.WorldToCell(transform.position); // Current cell of the enemy
        Vector3Int nextCell2 = bfs.getTile.WorldToCell(next); // Next cell from BFS

        if (nextCell2 != currentCell2) // If the next cell is different from the current cell
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                next,
                chaseSpeed * Time.deltaTime
            ); // Move towards the next step
        }

        if (enemyCell == playerCell)
        {
            KillPlayer();
        }
    }

    /// <summary>
    /// Called when the enemy reaches the same tile as the player.
    /// Replace with your own death / game-over logic as needed.
    /// </summary>
    private void KillPlayer() // method to kill the player
    {
        // Example: destroy the player object
        if (player != null)
        {
            Destroy(player.gameObject); // Destroy the player object if it exists.
        }

        Debug.Log("Player caught by enemy!");
    }
}
