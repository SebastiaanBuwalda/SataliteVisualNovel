using UnityEngine;
using TMPro;
using System.Collections;

public class TMPTextChanger : MonoBehaviour
{
    [Header("TextMeshPro Component")]
    public TextMeshPro tmpText;

    [Header("Text Settings")]
    public string newText = "This is the new text!";
    public float delayInSeconds = 3f;

    private void Start()
    {
        StartCoroutine(ChangeTextAfterDelay());
    }

    private IEnumerator ChangeTextAfterDelay()
    {
        yield return new WaitForSeconds(delayInSeconds);
        tmpText.text = newText;
    }
}
