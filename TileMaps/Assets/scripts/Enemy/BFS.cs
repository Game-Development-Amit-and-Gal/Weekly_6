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


    public Tilemap getTile => tilemap;
    public Vector3 GetNextStepTowardsPlayer()
    {
        Vector3Int start = tilemap.WorldToCell(transform.position);
        Vector3Int target = tilemap.WorldToCell(player.position);

        if (start == target)
            return transform.position;

        var queue = new Queue<Vector3Int>();
        var cameFrom = new Dictionary<Vector3Int, Vector3Int>();

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

        // Player unreachable
        return transform.position;
    }

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

    private bool IsWalkable(Vector3Int cell)
    {
        TileBase t = tilemap.GetTile(cell);
        Debug.Log("Checking tile: " + t + " at " + cell);

        return allowed.Contains(t);
    }

}
