using UnityEngine;
using State = UnityEngine.MonoBehaviour;

public class EnemyStateMachine : StateMachine // A simple state machine for enemy AI with patrol and chase states
{
    [Header("Chase Settings")]
    [SerializeField] private Transform player;      // Reference to the player transform
    [SerializeField] private float chaseRadius = 2f;    // Distance within which the enemy will start chasing the player

    [Header("States")]
    [SerializeField] private State patrolState;   // <- ANY patrol script
    [SerializeField] private State chaseState;    // <- your ChaseState

    private void Awake()
    {
        if (patrolState != null) AddState(patrolState);     // Add patrol state to the state machine
        if (chaseState != null) AddState(chaseState);       // Add chase state to the state machine

        if (patrolState != null && chaseState != null)      // Define transitions between states
        {
            AddTransition(patrolState, PlayerInChaseRange, chaseState);         // From patrol to chase if player is in range
            AddTransition(chaseState, () => !PlayerInChaseRange(), patrolState);  // From chase to patrol if player is out of range
        }
    }

    private void Start() => Initialize(); // Call to Parent to Initialize

    private bool PlayerInChaseRange()   // Check if player is within chase radius
    {
        if (!player) return false;
        var d = player.position - transform.position; // Distance between enemy and player
        return d.sqrMagnitude <= chaseRadius * chaseRadius; // Compare squared distances for efficiency
    }
}
