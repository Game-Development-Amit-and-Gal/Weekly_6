using UnityEngine;
using UnityEngine.InputSystem;

// ... (Documentation and Configuration are the same) ...

public class PlayerBoatInteraction : MonoBehaviour
{
    // ---------------- CONFIGURATION ----------------
    [Tooltip("Radius around the player used to detect nearby boat objects.")]
    [SerializeField] private float detectRadius = 1f;

    [Tooltip("Time in seconds to block re-entering the boat immediately after dismounting.")]
    [SerializeField] private float exitCooldownTime = 0.2f; // <-- NEW FIELD

    // ... (Other serialized fields are the same) ...
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform boatTransform;
    [SerializeField] private Vector3 mountOffset = Vector3.up * 0.5f;

    // ---------------- CONSTANTS ----------------
    private const float INITIAL_INTERACTION_DELAY = 0.2f;
    private const string INTERACT_ACTION_NAME = "Interact";

    // ---------------- PRIVATE FIELDS ----------------
    private InputAction interactAction;
    private bool isMounted = false;
    private bool interactPressed = false;
    private bool allowInteraction = false;
    private Vector3 initialWorldPosition;

    private float exitTime = 0f; // <-- NEW TRACKER

    // ---------------- UNITY LIFECYCLE ----------------
    // ... (Awake, Start, OnEnable, OnDisable, Update methods are the same) ...

    // ---------------- PUBLIC INTERFACE ----------------

    /// <summary>
    /// Checks if the player is near a boat and pressed the interact key to initiate mounting.
    /// Uses a simple distance check instead of physics detection.
    /// </summary>
    /// <returns>True if the transition to BoatState should occur.</returns>
    public bool ShouldEnterBoat()
    {
        if (!allowInteraction) return false;
        if (isMounted) return false;

        // --- COOLDOWN CHECK ---
        if (Time.time < exitTime)
        {
            // Ignore entry attempts immediately after a dismount
            return false;
        }
        // ----------------------

        float distanceToBoat = Vector3.Distance(playerTransform.position, boatTransform.position);
        bool nearBoat = distanceToBoat <= detectRadius;

        if (nearBoat && interactPressed)
        {
            Debug.Log("Transition SUCCESS (Distance Check): Near Boat and Interact Pressed.");
            return true;
        }

        // ... (Debug logging for failure is the same) ...

        return false;
    }


    /// <summary>
    /// Checks if player pressed interact while mounted, meaning we should exit the boat.
    /// </summary>
    /// <returns>True if the transition back to WalkState should occur.</returns>
    public bool ShouldExitBoat()
    {
        if (isMounted && interactPressed)
        {
            Debug.Log("Dismount SUCCESS: Mounted and Interact Pressed.");
            return true;
        }
        return false;
    }

    // ... (MountBoat method is the same) ...
    public void MountBoat()
    {
        isMounted = true;
        playerTransform.position = boatTransform.position + mountOffset;
        Debug.Log("Player MOUNTED onto boat.");
    }


    /// <summary>
    /// Performs the actual dismount operation: updates state.
    /// </summary>
    public void DismountBoat()
    {
        isMounted = false;
        // RECORD THE TIME OF EXIT
        exitTime = Time.time + exitCooldownTime; // <-- NEW LOGIC
        Debug.Log("Player DISMOUNTED boat.");
    }

    public Transform getBoat
    {
        get { return boatTransform; }
    }
}