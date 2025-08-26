using UnityEngine;
using TMPro; // Needed for TextMeshPro

public class ClickSoundAndToggle : MonoBehaviour
{
    [Header("Sound Settings")]
    public AudioSource audioSource; // Drag AudioSource with your sound clip here

    [Header("Toggle Settings")]
    public GameObject[] objectsToToggle; // Assign objects in Inspector
    public float delaySeconds = 2f; // Delay before toggling objects

    [Header("Text Settings")]
    public TMP_Text tmpText; // Drag your TMP text object here
    public Color targetColor = Color.red; // New color for text

    private bool hasClicked = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !hasClicked) // Left mouse click
        {
            hasClicked = true;

            // Play the sound
            if (audioSource != null)
                audioSource.Play();

            // Change text color
            if (tmpText != null)
                tmpText.color = targetColor;

            // Start object toggle coroutine
            StartCoroutine(ToggleObjectsAfterDelay());
        }
    }

    private System.Collections.IEnumerator ToggleObjectsAfterDelay()
    {
        yield return new WaitForSeconds(delaySeconds);

        foreach (GameObject obj in objectsToToggle)
        {
            if (obj != null)
                obj.SetActive(!obj.activeSelf); // Toggle
        }

        hasClicked = false; // Optional: allow clicking again
    }
}
