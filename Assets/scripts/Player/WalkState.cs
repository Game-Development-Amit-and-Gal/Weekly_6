using UnityEngine;

public class WalkState : MonoBehaviour
{
    [SerializeField] private Mover mover;
    [SerializeField] private float walkSpeed = 4f;

    private void OnEnable()
    {
        mover.enabled = true;
    }
}
