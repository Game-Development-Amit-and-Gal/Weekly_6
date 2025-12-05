using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class Mover : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float defaultSpeed = 4f;

    [Header("Tilemap")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private List<TileBase> AllowedTiles = new List<TileBase>();
    [Header("Tile Cost")]
    [SerializeField] private TileCost tileCost;


    private InputAction moveAction;
    private Vector3 targetPos;
    private float currentSpeed;
    public bool allowedMove = true;
    private const float ArrivalThreshold = 0.01f;

    private static readonly Vector3 Up = Vector3.up;
    private static readonly Vector3 Down = Vector3.down;
    private static readonly Vector3 Left = Vector3.left;
    private static readonly Vector3 Right = Vector3.right;
    private static readonly Vector3 Zero = Vector3.zero;
    private const float lowerBound = 1f;

    private const string MoveActionName = "Move";



    private void Awake()
    {
        // Create one 2D input action (WASD + arrows)
        moveAction = new InputAction(MoveActionName, InputActionType.Value);

        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");
    }

    private void OnEnable()
    {
        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
    }

    private void Start()
    {

        targetPos = transform.position;
        currentSpeed = defaultSpeed;
    }

    public void SetMoveSpeed(float speed)
    {
        currentSpeed = speed;
    }

    private void Update()
    {
        if (!allowedMove) return;
        MoveTowardsTarget();
        TryStartNewMove();
    }

    private void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            currentSpeed * Time.deltaTime);
    }

    private void TryStartNewMove()
    {
        if (Vector3.Distance(transform.position, targetPos) > ArrivalThreshold)
            return;

        Vector2 rawInput = moveAction.ReadValue<Vector2>();
        Vector3 dir = GetDirection(rawInput);
        if (dir == Zero) return;

        Vector3 nextWorldPos = transform.position + dir;
        if (CanMoveTo(nextWorldPos))
            targetPos = nextWorldPos;
    }

    private static Vector3 GetDirection(Vector2 input)
    {
        if (input.y > 0) return Up;
        if (input.y < 0) return Down;
        if (input.x < 0) return Left;
        if (input.x > 0) return Right;
        return Zero;
    }

    private bool CanMoveTo(Vector3 worldPos)
    {
        if (tilemap == null) return true;

        Vector3Int cell = tilemap.WorldToCell(worldPos);
        TileBase tile = tilemap.GetTile(cell);

        if (tile == null) return false; // outside map

        // If there's no TileCost assigned, just follow allowed tiles
        if (tileCost == null)
            return AllowedTiles.Contains(tile);

        // Check if tile is allowed
        if (!AllowedTiles.Contains(tile))
            return false;

        // Get cost and change movement speed dynamically
        int cost = tileCost.GetTileCost(cell);
        currentSpeed = defaultSpeed / Mathf.Max(lowerBound, cost);

        return true;
    }

    /// <summary>
    /// Called when click-movement finishes, so the mover
    /// doesn’t try to pull the player back to its old target.
    /// </summary>
    public void SyncTargetToCurrent()
    {
        targetPos = transform.position;
    }


}
