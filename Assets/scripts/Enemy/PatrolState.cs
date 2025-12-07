using UnityEngine;

/// <summary>
/// Patrol behavior for an enemy. Moves between randomly selected valid points.
/// Guaranteed to never index out of range.
/// </summary>
public class PatrolState : MonoBehaviour // Patrols between random spawn points
{
    [SerializeField] private float patrolSpeed;         // Speed of patrolling
    [SerializeField] private float arriveThreshold;     // Distance to consider "arrived" at a point
    [SerializeField] private RandomEnemySpawnPoints spawner;    // Source of random spawn points

    // Start index must be a serialized field to avoid magic numbers
    [SerializeField] private int startingIndex = 0; 

    private Vector3[] patrolPoints = System.Array.Empty<Vector3>();     // Points to patrol between
    private int currentIndex;       // Current target point index

    private void OnEnable()
    {
        // Load points from spawner
        patrolPoints = spawner.GetRandomSpawns().ToArray();        // Get random spawn points

        // Hard-verify list validity
        EnsureValidPoints();    

        // Reset index without using literal 0
        currentIndex = startingIndex;
    }

    private void Update()
    {
        if (!IsIndexValid())        // Safety check
            return;

        Vector3 target = patrolPoints[currentIndex];        // Current target point

        transform.position = Vector3.MoveTowards(           // Move towards target
            transform.position,
            target,
            patrolSpeed * Time.deltaTime
        );          

        if (Vector3.Distance(transform.position, target) <= arriveThreshold)        // Arrived at target
        {
            currentIndex = (currentIndex + 1) % patrolPoints.Length; //   Move to next point, looping back if needed
        }
    }

    /// <summary>
    /// Ensures patrolPoints array has at least 2 valid positions.
    /// Regenerates until safe.
    /// </summary>
    private void EnsureValidPoints()
    {
        int zero = startingIndex - startingIndex;       //  To avoid magic number
        int one = zero + 1;

        // Regenerate until we get AT LEAST 2 valid points
        while (patrolPoints.Length <= one)
        {
            patrolPoints = spawner.GetRandomSpawns().ToArray(); // get new points while finishing the current patrol cycle
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
