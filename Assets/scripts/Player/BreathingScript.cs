using UnityEngine;

/// <summary>
/// Applies a subtle “breathing” / pulsing scale animation to this GameObject.
/// The object oscillates around its original scale without drifting or inflating.
/// </summary>
public class BreathingScript : MonoBehaviour
{
    // ---------- CONSTANTS ----------

    /// <summary>Default duration (in seconds) for one full inhale–exhale cycle.</summary>
    private const float DefaultBreathDurationSeconds = 2f;

    /// <summary>Default maximum relative size change (e.g. 0.1 = ±10% around base scale).</summary>
    private const float DefaultAmplitude = 0.1f;

    /// <summary>Default number of harmonics used in the breathing wave.</summary>
    private const int DefaultHarmonicCount = 2;

    /// <summary>Minimum allowed duration to avoid division-by-zero or absurd speeds.</summary>
    private const float MinBreathDurationSeconds = 0.01f;

    /// <summary>First harmonic index used in the series.</summary>
    private const int FirstHarmonicIndex = 1;

    /// <summary>Minimum number of harmonics (simple sine wave).</summary>
    private const int MinHarmonicCount = 1;

    /// <summary>Multiplier that turns π into a full turn (2π).</summary>
    private const float FullTurnMultiplier = 2f;

    /// <summary>Base factor around which the scale oscillates (1 = original size).</summary>
    private const float BaseScaleFactor = 1f;

    /// <summary>Initial accumulator value for the harmonic sum.</summary>
    private const float InitialScaleOffset = 0f;

    /// <summary>Unit scale axis (all components equal) for uniform scaling.</summary>
    private static readonly Vector3 UniformScaleAxis = Vector3.one;

    /// <summary>Two-pi constant (360° in radians), computed once.</summary>
    private static readonly float TwoPi = Mathf.PI * FullTurnMultiplier;

    // ---------- CONFIGURATION (EDITABLE IN INSPECTOR) ----------

    [Header("Breathing Timing")]
    [Tooltip("Time in seconds for one full inhale–exhale cycle.")]
    [SerializeField] private float breathDurationSeconds = DefaultBreathDurationSeconds;

    [Header("Breathing Intensity")]
    [Tooltip("Maximum relative size change. 0.1 = ±10% around the original scale.")]
    [SerializeField] private float amplitude = DefaultAmplitude;

    [Header("Wave Detail")]
    [Tooltip("Number of harmonics used to shape the breathing curve (1 = simple sine).")]
    [SerializeField] private int harmonicCount = DefaultHarmonicCount;

    // ---------- RUNTIME STATE ----------

    /// <summary>Original local scale of the object, used as the center of breathing.</summary>
    private Vector3 baseScale;

    // ---------- UNITY LIFECYCLE ----------

    private void Awake()
    {
        // Store the starting scale so we always breathe around this value.
        baseScale = transform.localScale;
    }

    private void Update()
    {
        // Clamp settings to safe ranges.
        float clampedDuration = Mathf.Max(breathDurationSeconds, MinBreathDurationSeconds);
        int clampedHarmonics = Mathf.Max(harmonicCount, MinHarmonicCount);

        // Convert time to radians so one full cycle (0 → 2π) happens over the breath duration.
        float angularTime = Time.time * (TwoPi / clampedDuration);

        // Build a small harmonic series: Σ sin(n * t) / n
        float scaleOffset = InitialScaleOffset;

        for (int harmonicIndex = FirstHarmonicIndex; harmonicIndex <= clampedHarmonics; harmonicIndex++)
        {
            float indexAsFloat = harmonicIndex;
            scaleOffset += Mathf.Sin(indexAsFloat * angularTime) / indexAsFloat;
        }

        // Turn offset into a scale factor around 1 (the base scale).
        float scaleFactor = BaseScaleFactor + (scaleOffset * amplitude);

        // Apply uniform scaling relative to the original scale.
        transform.localScale = baseScale * scaleFactor;
    }
}
