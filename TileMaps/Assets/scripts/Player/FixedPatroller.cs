using UnityEngine;

public class FixedPatroller : MonoBehaviour
{
    [SerializeField] private Transform[] route;
    private int currentTargetIndex = 0;
    private float epsilon = 0.1f;
  


    private void Update()
    {
        int one = 1;
        if (route.Length == 0) return;
        Transform target = route[currentTargetIndex];
        float step = 2f * Time.deltaTime; // Patrol speed
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        if (Vector3.Distance(transform.position, target.position) < epsilon)
        {
            currentTargetIndex = (currentTargetIndex + one) % route.Length;
        }
    }
}

