using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerPathfinder : MonoBehaviour // Handles click-to-move pathfinding on a Tilemap
{
    [SerializeField] private Tilemap tilemap;   // The Tilemap to navigate
    [SerializeField] private TileCost cost;     // Tile cost data for pathfinding
    [SerializeField] private float moveSpeed;   // Current movement speed
    [SerializeField] private float defaultSpeed = 5f;       // Default movement speed
    [SerializeField] private List<TileBase> allowedTiles = new List<TileBase>();        // Tiles that can be traversed   
    [SerializeField] private Mover mover;      // Reference to Mover component for sync
    [SerializeField] private float mountaintSpeed = 2f;     // Speed on mountain tiles
    [SerializeField] private float waterSpeed = 3f;  // Speed on water tiles


    private const float epsilon = 0.05f; // Threshold to consider "arrived" at a tile 
    private List<Vector3Int> path;   // Current path as a list of tile positions
    private int pathIndex = 0;       // Current index in the path
    private TilemapGraph graph;      // Graph representation of the Tilemap for pathfinding


    private Camera cam;     // Main camera reference in order to convert screen to world coordinates




    private void Start() // Initialization
    {
        moveSpeed = defaultSpeed; // Set initial movement speed
        cam = Camera.main;       // Get main camera 
        graph = new TilemapGraph(tilemap, cost);     // Create graph for pathfinding

        // Snap player to grid  
        Vector3Int cell = tilemap.WorldToCell(transform.position);      // Get cell under player
        transform.position = tilemap.GetCellCenterWorld(cell);          // Snap to cell center
    }

    private void Update() // Handle input and movement each frame
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)       // On left mouse click
        {
            Vector3 mouseScreen = Mouse.current.position.ReadValue();       // Get mouse position in screen coordinates
            Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);       // Convert to world coordinates
            mouseWorld.z = 0f; // VERY IMPORTANT in order to avoid Z axis bugs

            Vector3Int targetCell = tilemap.WorldToCell(mouseWorld);   // Get target cell from mouse position
            Vector3Int startCell = tilemap.WorldToCell(transform.position);  // Get start cell from player position
            TileBase tileBase = tilemap.GetTile(targetCell);       // Get the tile at the target cell

            if (tilemap.HasTile(targetCell))                // If the target cell has a tile
            {
                if (mover != null)      // Sync mover if available
                {
                    mover.SyncTargetToCurrent();    // Sync mover target to current position
                    mover.allowedMove = false;      // Disable mover movement
                }
                path = Dijkstra.GetPath(graph, startCell, targetCell);  // Find path using Dijkstra's algorithm
                pathIndex = 0;   // Reset path index


                Debug.Log($"PATH FOUND! Length: {path?.Count}");    // Log path length
            }
            else
            {
                Debug.LogWarning("Clicked an invalid tile!");   // Log warning for invalid tile
            }
        }

        MoveAlongPath();    // Move the player along the calculated path

    }

    private void MoveAlongPath()
    {
        if (path == null)    // No path to follow
            return;

        if (pathIndex >= path.Count)        // Reached the end of the path
        {
            // finished path
            path = null;

            if (mover != null)   // Sync mover if available
            {
                mover.SyncTargetToCurrent();
                mover.allowedMove = true;
            }
            return;
        }

        Vector3 target = tilemap.GetCellCenterWorld(path[pathIndex]);       //  Get world position of the next tile in the path
        TileBase current = tilemap.GetTile(path[pathIndex]);  // Get the tile at the current path position
        if (current != null)   // Ensure the tile is valid
        {
            // Adjust movement speed based on tile type
            if (current.name.Contains("Mountain")) 
            {
                moveSpeed = mountaintSpeed;
            }
            else if (current.name.Contains("Water"))
            {
                moveSpeed = waterSpeed;
            }
            else
            {
                moveSpeed = defaultSpeed;
            }
        }


        target.z = transform.position.z;  // Maintain current Z position

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            moveSpeed * Time.deltaTime
        );   // Move towards the target position

        if (Vector3.Distance(transform.position, target) <= epsilon)   // Check if arrived at the target tile
        {
            Debug.Log($"Arrived at step {pathIndex}: {path[pathIndex]}"); // Log arrival at the current step
            pathIndex++; // Move to the next tile in the path
        }
    }



    private void OnDrawGizmos()  // Visualize the path in the editor
    {
        const int minCount = 2;  // Minimum number of points to draw a path
        if (path == null || path.Count < minCount) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < path.Count - 1; i++)
        {
            // Draw line between consecutive path points
            Gizmos.DrawLine(
                tilemap.GetCellCenterWorld(path[i]), 
                tilemap.GetCellCenterWorld(path[i + 1]) 
            );
        }
    }

}
