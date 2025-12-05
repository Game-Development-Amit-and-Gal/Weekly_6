using Unity.Hierarchy;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBoatInteraction : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float detectRadius = 1.2f;
    [SerializeField] private Transform boatTransform;

    [Header("Mount Settings")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector3 mountOffset = new Vector3(0f, 1f, 0f);

    [Header("Input")]
    [SerializeField] private InputAction interactAction;  // bind to <Keyboard>/f in Inspector

    private bool isMounted = false;

    private const float debounceTime = 0.15f;
    private const float initialDelay = 0.2f;

    private float enableTime;
    private bool inputCooldown = false;
    private bool inBoat = false;

    public bool isInBoat => inBoat;
    private void Awake()
    {
        if (playerTransform != null)
            playerTransform.SetParent(null, true);
    }

    private void OnEnable()
    {
        interactAction.Enable();
        enableTime = Time.time;
    }

    private void OnDisable()
    {
        interactAction.Disable();
    }

    // ---------- TRANSITIONS ----------

    public bool ShouldEnterBoat()
    {
        // TEMP DEBUG VERSION
        bool pressed = interactAction.WasPerformedThisFrame();
        float dist = Vector2.Distance(playerTransform.position, boatTransform.position);

        Debug.Log($"ENTER check | pressed={pressed} | dist={dist}");

        // For now: if F was pressed AND we are within radius, allow enter
        if (pressed && dist <= detectRadius)
        {
            Debug.Log("ENTER condition TRUE");
            return true;
        }

        return false;
    }

    public bool ShouldExitBoat()
    {
        // TEMP DEBUG VERSION
        bool pressed = interactAction.WasPerformedThisFrame();
        Debug.Log($"EXIT check | pressed={pressed} | mounted={isMounted}");

        if (isMounted && pressed)
        {
            Debug.Log("EXIT condition TRUE");
            return true;
        }

        return false;
    }


    // ---------- ACTIONS ----------

    public void MountBoat()
    {
        isMounted = true;
        StartCooldown();

        playerTransform.position = boatTransform.position + mountOffset;
        Debug.Log("Mounted boat");
    }

    public void DismountBoat()
    {
        isMounted = false;
        StartCooldown();

        playerTransform.position = boatTransform.position - mountOffset;
        Debug.Log("Dismounted boat");
    }

    private float delay = 1000f;
    private async void StartCooldown()
    {
        inputCooldown = true;
        await System.Threading.Tasks.Task.Delay((int)(debounceTime * delay)); 
        inputCooldown = false;
    }

    public bool IsMounted => isMounted;
    public Transform Boat => boatTransform;
    public Vector3 MountOffset => mountOffset;
}
