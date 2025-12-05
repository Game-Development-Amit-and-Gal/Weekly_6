using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Mover : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float defaultSpeed = 4f;

    [Header("Tilemap")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private List<TileBase> AllowedTiles = new List<TileBase>();

    [Header("Boat / Inventory")]
    [SerializeField] private PlayerBoatInteraction boatInteraction;
    [SerializeField] private bool hasGoat = false;
    [SerializeField] private bool hasPickaxe = false;

    private InputAction moveAction;
    private Vector3 targetPos;
    private float currentSpeed;

    private const float ArrivalThreshold = 0.01f;

    private static readonly Vector3 Up = Vector3.up;
    private static readonly Vector3 Down = Vector3.down;
    private static readonly Vector3 Left = Vector3.left;
    private static readonly Vector3 Right = Vector3.right;
    private static readonly Vector3 Zero = Vector3.zero;

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
        if (tile == null) return false;
        foreach (TileBase allowedTile in AllowedTiles)
        {
            if (tile == allowedTile)
            {
                // Additional checks for special tiles
                if (allowedTile.name == "Water")
                {
                    if (boatInteraction.isInBoat)
                    {
                        AllowedTiles.Add(tile);
                    } else if(AllowedTiles.Contains(tile))
                    {
                        AllowedTiles.Remove(tile);
                    }
                }
                else if (allowedTile.name == "GrassWithGoat")
                {
                    return hasGoat;
                }
                else if (allowedTile.name == "RockyGround")
                {
                    return hasPickaxe;
                }
                return true;
            }
        }

        return false;
    }
}
