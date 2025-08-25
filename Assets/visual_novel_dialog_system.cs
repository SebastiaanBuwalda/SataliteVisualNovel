// VisualNovelDialogSystem.cs
// Drop this single script on a GameObject in your scene.
// Uses TextMeshProUGUI for both speaker and line.
// Assign a TextAsset (drag a .txt into the inspector) or set streamingAssetsFilePath to load at runtime.
// Click / Space advances lines. Click while typing will instantly finish the line.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System.IO;

public class VisualNovelDialogSystem : MonoBehaviour
{
    [Header("UI - TextMeshPro")]
    [SerializeField] private TextMeshProUGUI speakerTMP;
    [SerializeField] private TextMeshProUGUI lineTMP;

    [Header("Dialog source")]
    [SerializeField] private TextAsset dialogTextAsset;
    [Tooltip("Relative to Application.streamingAssetsPath, e.g. 'dialogs/mystory.txt'")]
    [SerializeField] private string streamingAssetsFilePath;

    [Header("Typewriter & behavior")]
    [SerializeField] private float typeSpeed = 0.02f;
    [SerializeField] private bool useTypewriter = true;

    [SerializeField] private GameObject[] objectsThatDisableAfterDialogue;
    [SerializeField] private GameObject[] objectsThatEnableAfterDialogue;

    private List<DialogueEntry> entries;
    private int index = 0;
    private Coroutine typingCoroutine;

    void Start()
    {
        if (dialogTextAsset != null)
        {
            LoadFromText(dialogTextAsset.text);
            ShowCurrent();
        }
        else if (!string.IsNullOrEmpty(streamingAssetsFilePath))
        {
            LoadFromStreamingAssets(streamingAssetsFilePath);
            ShowCurrent();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            Advance();
        }
    }

    public void LoadFromTextAsset(TextAsset ta)
    {
        if (ta == null) return;
        LoadFromText(ta.text);
        index = 0;
        ShowCurrent();
    }

    public void LoadFromStreamingAssets(string relativePath)
    {
        string full = Path.Combine(Application.streamingAssetsPath, relativePath);
        if (!File.Exists(full)) return;
        LoadFromText(File.ReadAllText(full));
        index = 0;
        ShowCurrent();
    }

    public void LoadFromText(string rawText)
    {
        entries = DialogueParser.Parse(rawText);
        index = 0;
    }

    void ShowCurrent()
    {
        if (entries == null || entries.Count == 0) return;

        var e = entries[index];
        speakerTMP.text = e.speaker;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        if (useTypewriter)
            typingCoroutine = StartCoroutine(TypeText(e.line));
        else
            lineTMP.text = e.line;
    }

    public void Advance()
    {
        if (entries == null || entries.Count == 0) return;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
            lineTMP.text = entries[index].line;
            return;
        }

        index++;
        if (index >= entries.Count)
        {
            Debug.Log("Dialogue finished.");
            // Enable objects
            if (objectsThatEnableAfterDialogue != null)
            {
                foreach (var objectToEnable in objectsThatEnableAfterDialogue)
                {
                    if (objectToEnable != null) objectToEnable.SetActive(true);
                }
            }
            // Disable objects
            if (objectsThatDisableAfterDialogue != null)
            {
                foreach (var objectToDisable in objectsThatDisableAfterDialogue)
                {
                    if (objectToDisable != null) objectToDisable.SetActive(false);
                }
            }
        }
        else
        {
            ShowCurrent();
        }
    }

    IEnumerator TypeText(string text)
    {
        lineTMP.text = string.Empty;
        foreach (char c in text)
        {
            lineTMP.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
        typingCoroutine = null;
    }

    [System.Serializable]
    public class DialogueEntry
    {
        public string speaker;
        public string line;
        public DialogueEntry(string s, string l)
        {
            speaker = s;
            line = l;
        }
    }
}

public static class DialogueParser
{
    public static List<VisualNovelDialogSystem.DialogueEntry> Parse(string raw)
    {
        var result = new List<VisualNovelDialogSystem.DialogueEntry>();
        if (string.IsNullOrEmpty(raw)) return result;

        var pattern = "<(?<tag>[sl])>(?<content>.*?)</\\k<tag>>";
        var rx = new Regex(pattern, RegexOptions.Singleline);

        string currentSpeaker = string.Empty;

        foreach (Match m in rx.Matches(raw))
        {
            var tag = m.Groups["tag"].Value;
            var content = m.Groups["content"].Value.Trim();

            if (tag == "s")
            {
                currentSpeaker = content;
            }
            else if (tag == "l")
            {
                result.Add(new VisualNovelDialogSystem.DialogueEntry(currentSpeaker, content));
            }
        }
        return result;
    }
}
