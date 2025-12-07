using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPatrol : MonoBehaviour
{
    [Header("Teleport Points")]
    [SerializeField] private List<Transform> patrolPoints = new List<Transform>(); // List of teleportation points

    [Header("Timing")]
    [SerializeField] private float teleportDelay = 2f;          // Delay between teleports in seconds

    private int index = 0;              // Current index in the patrol points list
    private Coroutine patrolRoutine;    //  Reference to the patrol coroutine

    private void OnEnable()             // Start the patrol when the object is enabled
    {
        int zero = 0; // avoid magic number
        if (patrolPoints.Count > zero)     // Ensure there are patrol points defined
            patrolRoutine = StartCoroutine(Patrol());           // Start the patrol coroutine
                                                               //Courtine - is a function that can pause its execution (yield)
                                                               //until the given YieldInstruction finishes.
    }



    private void OnDisable() // Stop the patrol when the object is disabled
    {
        if (patrolRoutine != null)
            StopCoroutine(patrolRoutine);
    }

    private IEnumerator Patrol()        // Coroutine for handling the teleportation patrol
    {
        while (true)
        {
            yield return new WaitForSeconds(teleportDelay);     // Wait for the specified delay

            transform.position = patrolPoints[index].position; // INSTANT TELEPORT

            index = (index + 1) % patrolPoints.Count; // Loop forward While avoiding out of range
        }
    }
}
