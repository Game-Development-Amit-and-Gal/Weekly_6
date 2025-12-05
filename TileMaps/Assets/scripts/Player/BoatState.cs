using UnityEngine;

public class BoatState : MonoBehaviour
{
    [SerializeField] private PlayerBoatInteraction boatInteraction;
    [SerializeField] private Mover mover;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform boatTransform;
    [SerializeField] private float boatSpeed = 3f;
    [SerializeField] private float walkSpeed = 4f;

    private void OnEnable()
    {
        mover.enabled = true;
    }

    private void OnDisable()
    {
    }

    private void LateUpdate()
    {
        if (boatInteraction.IsMounted)
        {
            boatTransform.position =
                playerTransform.position - boatInteraction.MountOffset;
        }
    }
}
