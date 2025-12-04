using UnityEngine;

/// <summary>
/// Normal walking state for the player.
/// Enables the Mover component to handle grid-based movement.
/// </summary>
public class WalkState : MonoBehaviour
{
    [Tooltip("Reference to the Mover component for player movement.")]
    [SerializeField] private Mover mover;

    /// <summary>
    /// Called when entering the WalkState. Enables the Mover.
    /// </summary>
    private void OnEnable()
    {
        mover.enabled = true;
    }

    /// <summary>
    /// Called when exiting the WalkState. Disables the Mover.
    /// </summary>
    private void OnDisable()
    {
        mover.enabled = false;
    }
}