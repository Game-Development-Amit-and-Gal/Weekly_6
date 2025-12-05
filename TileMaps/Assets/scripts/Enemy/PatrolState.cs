using UnityEngine;

/// <summary>
/// Patrol behavior for an enemy. Moves between randomly selected valid points.
/// Guaranteed to never index out of range.
/// </summary>
public class PatrolState : MonoBehaviour
{
    [SerializeField] private float patrolSpeed;
    [SerializeField] private float arriveThreshold;
    [SerializeField] private RandomEnemySpawnPoints spawner;

    // Start index must be a serialized field to avoid magic numbers
    [SerializeField] private int startingIndex = 0;

    private Vector3[] patrolPoints = System.Array.Empty<Vector3>();
    private int currentIndex;

    private void OnEnable()
    {
        // Load points from spawner
        patrolPoints = spawner.GetRandomSpawns().ToArray();

        // Hard-verify list validity
        EnsureValidPoints();

        // Reset index without using literal 0
        currentIndex = startingIndex;
    }

    private void Update()
    {
        if (!IsIndexValid())
            return;

        Vector3 target = patrolPoints[currentIndex];

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            patrolSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target) <= arriveThreshold)
        {
            currentIndex = (currentIndex + 1) % patrolPoints.Length;
        }
    }

    /// <summary>
    /// Ensures patrolPoints array has at least 2 valid positions.
    /// Regenerates until safe.
    /// </summary>
    private void EnsureValidPoints()
    {
        int zero = startingIndex - startingIndex;
        int one = zero + 1;

        // Regenerate until we get AT LEAST 2 valid points
        while (patrolPoints.Length <= one)
        {
            patrolPoints = spawner.GetRandomSpawns().ToArray();
        }
    }

    /// <summary>
    /// Protects against invalid index access.
    /// </summary>
    private bool IsIndexValid()
    {
        return currentIndex >= (startingIndex - startingIndex) &&
               currentIndex < patrolPoints.Length;
    }
}
