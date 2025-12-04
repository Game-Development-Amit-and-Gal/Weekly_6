using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

/// <summary>
/// Grid-based hero movement using the New Input System.
/// Handles movement logic and collision checks against the Tilemap.
/// </summary>
public class Mover : MonoBehaviour
{
    // ---------------- CONFIGURATION ----------------
    [Header("Movement Settings")]
    [Tooltip("Distance threshold to consider the player has arrived at the target position.")]
    [SerializeField] private float arrivalThreshold = 0.01f;
    [Tooltip("Speed at which the player moves between grid cells.")]
    [SerializeField] private float moveSpeed = 4f;

    [Header("Tilemap and Interaction")]
    [Tooltip("Reference to the Tilemap used for movement checks.")]
    [SerializeField] private Tilemap tilemap;
    [Tooltip("Reference to the PlayerBoatInteraction component for boat location.")]
    [SerializeField] private PlayerBoatInteraction boatInteraction;
    [Tooltip("TileBase representing walkable grass terrain.")]
    [SerializeField] private TileBase grassTile;
    [Tooltip("TileBase representing water/sea terrain.")]
    [SerializeField] private TileBase waterTile;
    [Tooltip("TileBase representing impassable mountain terrain.")]
    [SerializeField] private TileBase mountainTile;

    [Header("Inventory")]
    [Tooltip("True if the player possesses the goat item.")]
    [SerializeField] private bool hasGoat = false;
    [Tooltip("True if the player possesses the pickaxe item.")]
    [SerializeField] private bool hasPickaxe = false;

    // ---------------- CONSTANTS ----------------
    private static readonly Vector3 UpDir = Vector3.up;
    private static readonly Vector3 DownDir = Vector3.down;
    private static readonly Vector3 LeftDir = Vector3.left;
    private static readonly Vector3 RightDir = Vector3.right;
    private static readonly Vector3 ZeroVec = Vector3.zero;
    private const string MOVEMENT_ACTION_NAME = "Move";

    // ---------------- PRIVATE FIELDS ----------------
    private InputAction movementAction;
    private Vector3 targetPos;

    // ---------------- UNITY LIFECYCLE ----------------
    private void Awake()
    {
        // Create InputAction with all bindings
        movementAction = new InputAction(
            name: MOVEMENT_ACTION_NAME,
            type: InputActionType.Value,
            binding: "" // Binding is set via composites
        );

        // Add 2D Composite for WASD keys
        movementAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        // Add 2D Composite for Arrow Keys
        movementAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");
    }

    /// <summary>
    /// Enables the movement input action when the component is enabled.
    /// </summary>
    private void OnEnable() => movementAction.Enable();

    /// <summary>
    /// Disables the movement input action when the component is disabled.
    /// </summary>
    private void OnDisable() => movementAction.Disable();

    private void Start()
    {
        targetPos = transform.position;
    }

    private void Update()
    {
        MoveTowardsTarget();
        TryMove();
    }

    // ---------------- MOVEMENT LOGIC ----------------

    /// <summary>
    /// Smoothly moves the player's transform towards the target position.
    /// </summary>
    private void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );
    }

    /// <summary>
    /// Checks for new input and initiates a move if the current move is finished.
    /// </summary>
    private void TryMove()
    {
        // Only allow a new move if the player has arrived at the current target
        if (Vector3.Distance(transform.position, targetPos) > arrivalThreshold)
            return;

        Vector3 dir = GetDirection();
        if (dir == ZeroVec)
            return;

        Vector3 nextWorldPos = transform.position + dir;

        if (CanMoveTo(nextWorldPos))
            targetPos = nextWorldPos;
    }

    /// <summary>
    /// Reads input action and determines the intended movement direction.
    /// </summary>
    /// <returns>A normalized Vector3 representing the direction, or Vector3.zero.</returns>
    private Vector3 GetDirection()
    {
        Vector2 input = movementAction.ReadValue<Vector2>();

        if (input.y > 0) return UpDir;
        if (input.y < 0) return DownDir;
        if (input.x < 0) return LeftDir;
        if (input.x > 0) return RightDir;

        return ZeroVec;
    }

    // ---------------- TILE LOGIC ----------------

    /// <summary>
    /// Determines if the player is allowed to move to a given world position based on tile type and inventory.
    /// </summary>
    /// <param name="worldPos">The destination world position.</param>
    /// <returns>True if movement is allowed, false otherwise.</returns>
    // Mover.cs

    // ... (snip) ...

    /// <summary>
    /// Determines if the player is allowed to move to a given world position based on tile type and inventory.
    /// </summary>
    /// <param name="worldPos">The destination world position.</param>
    /// <returns>True if movement is allowed, false otherwise.</returns>
    private bool CanMoveTo(Vector3 worldPos)
    {
        Vector3Int cell = tilemap.WorldToCell(worldPos);
        TileBase tile = tilemap.GetTile(cell);

        if (tile == null) return false;

        if (tile == grassTile) return true;

        // --- Water Tile Logic ---
        if (tile == waterTile)
        {
            // If the destination cell is the exact cell the boat OBJECT is sitting on, allow movement.
            // This is necessary to walk onto the boat's location to mount it.
            if (boatInteraction != null && boatInteraction.getBoat != null)
            {
                // Convert the boat's world position to its cell position
                Vector3Int boatCell = tilemap.WorldToCell(boatInteraction.getBoat.position);

                // Check if the target cell is the boat's cell
                if (cell == boatCell)
                {
                    Debug.Log("Allowed to move onto boat tile.");
                    return true;
                }
            }

            // Otherwise, water is impassable.
            return false;
        }

        // --- Mountain Tile Logic ---
        if (tile == mountainTile)
        {
            if (hasGoat) return true;

            if (hasPickaxe)
            {
                // Break mountain and replace with grass
                tilemap.SetTile(cell, grassTile);
                return true;
            }

            return false;
        }

        return false;
    }

}