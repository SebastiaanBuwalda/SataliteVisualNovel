using TMPro;
using UnityEngine;

public class TextPulse : MonoBehaviour
{
    [SerializeField] private TextMeshPro tmpText;
    [SerializeField] private Color startColor = Color.white;
    [SerializeField] private Color endColor = Color.red;
    [Tooltip("Pulses per second.")]
    [SerializeField, Min(0f)] private float pulseSpeed = 0.2f;

    private void Update()
    {
        if (tmpText == null)
        {
            return;
        }

        float phase = (Mathf.Sin(Time.time * pulseSpeed * Mathf.PI * 2f) + 1f) * 0.5f;
        float easedPhase = Mathf.SmoothStep(0f, 1f, phase);
        // Squaring the phase keeps the text near its start color for longer.
        tmpText.color = Color.Lerp(startColor, endColor, easedPhase * easedPhase);
    }
}
