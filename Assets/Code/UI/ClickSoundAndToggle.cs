using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>Responds to a click anywhere, then toggles menu or hint objects after a delay.</summary>
public class ClickSoundAndToggle : MonoBehaviour
{
    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;

    [Header("Toggle")]
    [SerializeField] private GameObject[] objectsToToggle;
    [SerializeField, Min(0f)] private float delaySeconds = 2f;

    [Header("Text")]
    [SerializeField] private TMP_Text tmpText;
    [SerializeField] private Color targetColor = Color.red;

    private bool isWaiting;
    private int activationFrame;

    private void OnEnable() => activationFrame = Time.frameCount;

    private void OnDisable()
    {
        StopAllCoroutines();
        isWaiting = false;
    }

    private void Update()
    {
        if (Time.frameCount <= activationFrame || isWaiting || !Input.GetMouseButtonDown(0))
        {
            return;
        }

        isWaiting = true;
        if (audioSource != null)
        {
            audioSource.Play();
        }

        if (tmpText != null)
        {
            tmpText.color = targetColor;
        }

        StartCoroutine(ToggleObjectsAfterDelay());
    }

    private IEnumerator ToggleObjectsAfterDelay()
    {
        yield return new WaitForSeconds(delaySeconds);
        isWaiting = false;
        SceneObjectState.Toggle(objectsToToggle);
    }
}
