using UnityEngine;

/// <summary>
/// The StateMachine managing the player's core states (e.g., Walking, Boating).
/// </summary>
public class PlayerStateMachine : StateMachine
{
    // ---------------- CONFIGURATION ----------------

    [Header("States")]
    [Tooltip("The normal walking state.")]
    [SerializeField] private WalkState walkState;
    [Tooltip("The state when the player is mounted on the boat.")]
    [SerializeField] private BoatState boatState;

    [Header("Interaction")]
    [Tooltip("Reference to the component handling boat mounting/dismounting logic.")]
    [SerializeField] private PlayerBoatInteraction boatInteraction;

    // ---------------- UNITY LIFECYCLE ----------------

    private void Awake()
    {
        // 1. Add all states to the machine
        this
            .AddState(walkState)
            .AddState(boatState);

        // 2. Define the transition from Walking to Boating
        this.AddTransition(
            walkState,
            () => boatInteraction.ShouldEnterBoat(),
            boatState
        );

        // 3. Define the transition from Boating back to Walking
        this.AddTransition(
            boatState,
            () => boatInteraction.ShouldExitBoat(),
            walkState
        );
    }

    private void Start()
    {
        // Initialize the state machine by explicitly starting the WalkState
        Initialize();
    }
}