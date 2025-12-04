using UnityEngine;

public class EnemyStateMachine : StateMachine
{
    [SerializeField] private Transform player;
    [SerializeField] private float chaseRadius;

    private PatrolState patrol;
    private ChaseState chase;

    private void Awake()
    {
        patrol = GetComponent<PatrolState>();
        chase = GetComponent<ChaseState>();

        this
            .AddState(patrol)
            .AddState(chase)
            .AddTransition(
                patrol,
                () => Vector3.Distance(transform.position, player.position) <= chaseRadius,
                chase)
            .AddTransition(
                chase,
                () => Vector3.Distance(transform.position, player.position) > chaseRadius,
                patrol);
    }

    private void Start()
    {
        Initialize();   // ← SAFE. Now states exist BEFORE running FSM.
    }
}
