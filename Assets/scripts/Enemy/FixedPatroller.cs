using UnityEngine;

public class FixedPatroller : MonoBehaviour         // Patrols along a fixed set of waypoints
{
    [SerializeField] private Transform[] route;     /* Enemy would go circling around and reach
                                                      waypoint_i such that i belongs to {waypoint_1,waypoint_2,waypoint_3}
                                                     */
                                                    
    private int currentTargetIndex = 0;             // Index of the current target waypoint
    private float epsilon = 0.1f;                   // Threshold to consider "arrived" at a waypoint



    private void Update()   // Move towards the current target waypoint each frame
    {
        int one = 1;    // To avoid magic number
        float speed = 2f; 


        if (route.Length == 0) return;      // No waypoints defined



        Transform target = route[currentTargetIndex]; // Get the current target waypoint
        float step = speed * Time.deltaTime; // Patrol speed

        transform.position = Vector3.MoveTowards(transform.position, target.position, step); // Move towards the target waypoint_i.
        if (Vector3.Distance(transform.position, target.position) < epsilon) // Reached the Waypoint
        {
            currentTargetIndex = (currentTargetIndex + one) % route.Length; // Move to the next waypoint, looping back to start if needed
        }
    }
}

