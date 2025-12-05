using UnityEngine;
using State = UnityEngine.MonoBehaviour;

public class EnemyStateMachine : StateMachine
{
    [Header("Chase Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private float chaseRadius = 2f;

    [Header("States")]
    [SerializeField] private State patrolState;   // <- ANY patrol script
    [SerializeField] private State chaseState;    // <- your ChaseState

    private void Awake()
    {
        if (patrolState != null) AddState(patrolState);
        if (chaseState != null) AddState(chaseState);

        if (patrolState != null && chaseState != null)
        {
            AddTransition(patrolState, PlayerInChaseRange, chaseState);
            AddTransition(chaseState, () => !PlayerInChaseRange(), patrolState);
        }
    }

    private void Start() => Initialize(); // Call to Parent to Initialize

    private bool PlayerInChaseRange()
    {
        if (!player) return false;
        var d = player.position - transform.position; // Distance between enemy and player
        return d.sqrMagnitude <= chaseRadius * chaseRadius; // Compare squared distances for efficiency
    }
}
