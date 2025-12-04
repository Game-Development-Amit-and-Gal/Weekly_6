using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Enemy AI that uses BFS to move one tile closer to the player.
/// Assumes movement only on valid walkable tiles.
/// </summary>
public class EnemyChaseBFS : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase grassTile;

    [SerializeField] private float moveSpeed = 3f;

    private Vector3 targetPos;

    // Directions (no magic values)
    private static readonly Vector3Int Up = new Vector3Int(0, 1, 0);
    private static readonly Vector3Int Down = new Vector3Int(0, -1, 0);
    private static readonly Vector3Int Left = new Vector3Int(-1, 0, 0);
    private static readonly Vector3Int Right = new Vector3Int(1, 0, 0);
    private static readonly Vector3Int[] Directions = { Up, Down, Left, Right };

    private void Start()
    {
        targetPos = transform.position;
    }

    private void Update()
    {
        MoveEnemy();
    }

    /// <summary>
    /// Moves toward the next BFS tile.
    /// </summary>
    private void MoveEnemy()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) > 0.01f)
            return;

        Vector3 next = GetNextStepTowardsPlayer();

        // If BFS found a next move
        if (next != transform.position)
            targetPos = next;
    }

    /// <summary>
    /// BFS pathfinding toward the player.
    /// Returns the next world position toward the player.
    /// </summary>
    public Vector3 GetNextStepTowardsPlayer()
    {
        Vector3Int start = tilemap.WorldToCell(transform.position);
        Vector3Int target = tilemap.WorldToCell(player.position);

        if (start == target)
            return transform.position;

        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();

        queue.Enqueue(start);
        cameFrom[start] = start;

        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();

            foreach (Vector3Int dir in Directions)
            {
                Vector3Int next = current + dir;

                if (!cameFrom.ContainsKey(next) && IsWalkable(next))
                {
                    cameFrom[next] = current;
                    queue.Enqueue(next);

                    if (next == target)
                        return ReconstructFirstStep(start, target, cameFrom);
                }
            }
        }

        return transform.position; // Player unreachable
    }

    /// <summary>
    /// BFS backtracking: find the first step from start toward target.
    /// </summary>
    private Vector3 ReconstructFirstStep(
        Vector3Int start,
        Vector3Int target,
        Dictionary<Vector3Int, Vector3Int> cameFrom)
    {
        Vector3Int current = target;

        while (cameFrom[current] != start)
            current = cameFrom[current];

        return tilemap.GetCellCenterWorld(current);
    }

    /// <summary>
    /// Checks if a tile is walkable (grass only for now).
    /// </summary>
    private bool IsWalkable(Vector3Int cell)
    {
        TileBase t = tilemap.GetTile(cell);
        return t == grassTile;
    }
}
