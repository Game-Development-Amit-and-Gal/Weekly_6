using UnityEngine;

/// <summary>
/// Chase behavior for an enemy.  
/// Uses BFS to compute the next step toward the player.
/// Enabled when the StateMachine switches into chase mode.
/// </summary>
public class ChaseState : MonoBehaviour
{
    /// <summary>Movement speed during chase.</summary>
    [SerializeField] private float chaseSpeed = 3f;

    /// <summary>Reference to the player's transform used as a chase target.</summary>
    [SerializeField] private Transform player = null;

    /// <summary>BFS component providing the next step toward the player.</summary>
    [SerializeField] private EnemyChaseBFS bfs = null;

    /// <summary>A reusable scaling factor for delta-time movement.</summary>
    [SerializeField] private float timeFactor = 1f;

    /// <summary>
    /// Moves the enemy toward the next BFS-generated step each frame.
    /// </summary>
    private void Update()
    {
        Vector3 next = bfs.GetNextStepTowardsPlayer();

        transform.position = Vector3.MoveTowards(
            transform.position,
            next,
            chaseSpeed * (Time.deltaTime * timeFactor));
    }
}
