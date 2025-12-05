using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerPathfinder : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileCost cost;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float defaultSpeed = 5f;
    [SerializeField] private List<TileBase> allowedTiles = new List<TileBase>();
    [SerializeField] private Mover mover;
    [SerializeField] private float mountaintSpeed = 2f;
    [SerializeField] private float waterSpeed = 3f;


    private const float epsilon = 0.05f; // Threshold to consider "arrived" at a tile 
    private List<Vector3Int> path;
    private int pathIndex = 0;
    private TilemapGraph graph;


    private Camera cam;




    private void Start()
    {
        moveSpeed = defaultSpeed;
        cam = Camera.main;
        graph = new TilemapGraph(tilemap, cost);

        // Snap player to grid
        Vector3Int cell = tilemap.WorldToCell(transform.position);
        transform.position = tilemap.GetCellCenterWorld(cell);
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 mouseScreen = Mouse.current.position.ReadValue();
            Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);
            mouseWorld.z = 0f; // VERY IMPORTANT

            Vector3Int targetCell = tilemap.WorldToCell(mouseWorld);
            Vector3Int startCell = tilemap.WorldToCell(transform.position);
            TileBase tileBase = tilemap.GetTile(targetCell);

            if (tilemap.HasTile(targetCell))
            {
                if (mover != null)
                {
                    mover.SyncTargetToCurrent();
                    mover.allowedMove = false;
                }
                path = Dijkstra.GetPath(graph, startCell, targetCell);
                pathIndex = 0;


                Debug.Log($"PATH FOUND! Length: {path?.Count}");
            }
            else
            {
                Debug.LogWarning("Clicked an invalid tile!");
            }
        }

        MoveAlongPath();

    }

    private void MoveAlongPath()
    {
        if (path == null)
            return;

        if (pathIndex >= path.Count)
        {
            // finished path
            path = null;

            if (mover != null)
            {
                mover.SyncTargetToCurrent();
                mover.allowedMove = true;
            }
            return;
        }

        Vector3 target = tilemap.GetCellCenterWorld(path[pathIndex]);
        TileBase current = tilemap.GetTile(path[pathIndex]);
        if (current != null)
        {
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


        target.z = transform.position.z;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target) <= epsilon)
        {
            Debug.Log($"Arrived at step {pathIndex}: {path[pathIndex]}");
            pathIndex++;
        }
    }



    private void OnDrawGizmos()
    {
        if (path == null || path.Count < 2) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < path.Count - 1; i++)
        {
            Gizmos.DrawLine(
                tilemap.GetCellCenterWorld(path[i]),
                tilemap.GetCellCenterWorld(path[i + 1])
            );
        }
    }

}
