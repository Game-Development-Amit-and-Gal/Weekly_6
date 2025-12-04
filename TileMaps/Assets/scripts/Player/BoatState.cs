using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// State where the player is mounted on the boat and controls the boat's movement.
/// Uses its own dedicated input action for reliable movement.
/// </summary>
public class BoatState : MonoBehaviour
{
    // ---------------- CONFIGURATION ----------------

    [Tooltip("Reference to the player's Transform component.")]
    [SerializeField] private Transform playerTransform; // <--- MUST BE ASSIGNED

    [Tooltip("Reference to the player's boat interaction manager.")]
    [SerializeField] private PlayerBoatInteraction interaction;

    [Tooltip("The speed at which the boat moves.")]
    [SerializeField] private float boatSpeed = 5f;

    // The previous serialized InputAction field is no longer needed.

    // ---------------- CONSTANTS ----------------
    private const float ZERO_Z_AXIS = 0f;
    private const string BOAT_MOVEMENT_ACTION_NAME = "BoatMove";

    // ---------------- PRIVATE FIELDS ----------------
    private InputAction boatMovementAction;

    // ---------------- UNITY LIFECYCLE ----------------

    private void Awake()
    {
        // Define an independent input action for the boat to prevent conflicts.
        boatMovementAction = new InputAction(
            name: BOAT_MOVEMENT_ACTION_NAME,
            type: InputActionType.Value,
            binding: ""
        );

        // 2D Composite (WASD)
        boatMovementAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
    }

    private void OnEnable()
    {
        // When entering the BoatState, finalize the mount operation
        interaction.MountBoat();
        boatMovementAction.Enable(); // Use the dedicated action
    }

    private void OnDisable()
    {
        // When exiting the BoatState, finalize the dismount operation
        interaction.DismountBoat();
        boatMovementAction.Disable(); // Disable the dedicated action
    }

    private void Update()
    {
        // 1. Handle Boat Movement Input
        Vector2 input = boatMovementAction.ReadValue<Vector2>();

        // Calculate the movement vector
        Vector3 move = new Vector3(input.x, input.y, ZERO_Z_AXIS) * boatSpeed * Time.deltaTime;

        // Move the Boat
        Transform boatTransform = interaction.getBoat;
        boatTransform.position += move;

        // 2. Manually Synchronize Player Position
        // This is the line that fixes the movement by moving the player with the boat.
        if (playerTransform != null)
        {
            playerTransform.position += move;
        }
        else
        {
            // CRITICAL ERROR: This means playerTransform is unassigned.
            Debug.LogError("BoatState: playerTransform is NOT assigned. Boat movement will fail!");
        }
    }
}