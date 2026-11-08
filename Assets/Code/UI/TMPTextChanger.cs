using System.Collections;
using TMPro;
using UnityEngine;

public class TMPTextChanger : MonoBehaviour
{
    [SerializeField] private TextMeshPro tmpText;
    [SerializeField, TextArea] private string newText = "This is the new text!";
    [SerializeField, Min(0f)] private float delayInSeconds = 3f;

    private void OnEnable() => StartCoroutine(ChangeTextAfterDelay());

    private void OnDisable() => StopAllCoroutines();

    private IEnumerator ChangeTextAfterDelay()
    {
        yield return new WaitForSeconds(delayInSeconds);
        if (tmpText != null)
        {
            tmpText.text = newText;
        }
    }
}
