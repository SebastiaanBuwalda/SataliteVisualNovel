using UnityEngine;
using TMPro;

public class TextPulse : MonoBehaviour
{
    public TextMeshPro tmpText; // Assign in Inspector
    public Color startColor = Color.white;
    public Color endColor = Color.red;
    public float pulseSpeed = 0.2f; // Pulses per second

    void Update()
    {
        if (tmpText != null)
        {
            // Sin wave from 0 to 1
            float t = (Mathf.Sin(Time.time * pulseSpeed * Mathf.PI * 2) + 1f) / 2f;

            // Ease in/out more dramatically for heartbeat effect
            t = Mathf.SmoothStep(0f, 1f, t); 

            // Optional: exaggerate the linger at red using a curve
            t = Mathf.Pow(t, 2f);

            tmpText.color = Color.Lerp(startColor, endColor, t);
        }
    }
}
