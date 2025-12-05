using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPatrol : MonoBehaviour
{
    [Header("Teleport Points")]
    [SerializeField] private List<Transform> patrolPoints = new List<Transform>();

    [Header("Timing")]
    [SerializeField] private float teleportDelay = 2f;

    private int index = 0;
    private Coroutine patrolRoutine;

    private void OnEnable()
    {
        if (patrolPoints.Count > 0)
            patrolRoutine = StartCoroutine(Patrol());
    }

    private void OnDisable()
    {
        if (patrolRoutine != null)
            StopCoroutine(patrolRoutine);
    }

    private IEnumerator Patrol()
    {
        while (true)
        {
            yield return new WaitForSeconds(teleportDelay);

            transform.position = patrolPoints[index].position; // INSTANT TELEPORT

            index = (index + 1) % patrolPoints.Count; // Loop forward
        }
    }
}
