using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>Plays a tagged story asset, then activates the next scene section.</summary>
public class VisualNovelDialogSystem : MonoBehaviour
{
    [Header("UI - TextMeshPro")]
    [SerializeField] private TextMeshProUGUI speakerTMP;
    [SerializeField] private TextMeshProUGUI lineTMP;
    [SerializeField] private GameObject nextLineIndicator;

    [Header("Dialogue Source")]
    [SerializeField] private TextAsset dialogTextAsset;
    [Tooltip("Relative to StreamingAssets, for example dialogs/mystory.txt.")]
    [SerializeField] private string streamingAssetsFilePath;

    [Header("Typewriter")]
    [SerializeField, Min(0f)] private float typeSpeed = 0.02f;
    [SerializeField] private bool useTypewriter = true;

    [Header("Completion")]
    [SerializeField] private GameObject[] objectsThatDisableAfterDialogue;
    [SerializeField] private GameObject[] objectsThatEnableAfterDialogue;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip advanceSound;

    private List<DialogueEntry> entries = new List<DialogueEntry>();
    private int currentIndex;
    private int activationFrame;
    private bool hasStarted;
    private bool isFinished;
    private Coroutine typingCoroutine;
    private Coroutine loadingCoroutine;

    private void OnEnable()
    {
        // A click that opens this section must not also advance its first line.
        activationFrame = Time.frameCount;
        if (hasStarted && !isFinished)
        {
            if (entries.Count > 0)
            {
                ShowCurrentLine();
            }
            else
            {
                LoadInitialDialogue();
            }
        }
    }

    private void Start()
    {
        if (speakerTMP == null || lineTMP == null)
        {
            Debug.LogError("Dialogue requires speaker and line text references.", this);
            enabled = false;
            return;
        }

        hasStarted = true;
        LoadInitialDialogue();
    }

    private void Update()
    {
        if (Time.frameCount > activationFrame &&
            (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
        {
            Advance();
        }
    }

    private void OnDisable()
    {
        StopTyping();
        StopLoading();
        SetNextLineIndicator(false);
    }

    private void LoadInitialDialogue()
    {
        if (dialogTextAsset != null)
        {
            LoadFromTextAsset(dialogTextAsset);
        }
        else if (!string.IsNullOrWhiteSpace(streamingAssetsFilePath))
        {
            LoadFromStreamingAssets(streamingAssetsFilePath);
        }
    }

    public void LoadFromTextAsset(TextAsset textAsset)
    {
        if (textAsset == null)
        {
            return;
        }

        LoadFromText(textAsset.text);
        ShowCurrentLine();
    }

    public void LoadFromStreamingAssets(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return;
        }

        StopLoading();
        StopTyping();
        entries.Clear();
        isFinished = false;
        streamingAssetsFilePath = relativePath;
        SetNextLineIndicator(false);
        loadingCoroutine = StartCoroutine(ReadStreamingAssets(relativePath));
    }

    private IEnumerator ReadStreamingAssets(string relativePath)
    {
        string path = Path.Combine(Application.streamingAssetsPath, relativePath);
        // Android and WebGL StreamingAssets use URLs rather than local files.
        string url = path.Contains("://") || path.StartsWith("jar:")
            ? path
            : new System.Uri(path).AbsoluteUri;

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            loadingCoroutine = null;
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Cannot load dialogue '{relativePath}': {request.error}", this);
                yield break;
            }

            LoadFromText(request.downloadHandler.text);
            ShowCurrentLine();
        }
    }

    /// <summary>Replaces the parsed entries. Use LoadFromTextAsset to load and display.</summary>
    public void LoadFromText(string rawText)
    {
        StopLoading();
        StopTyping();
        entries = DialogueParser.Parse(rawText);
        currentIndex = 0;
        isFinished = false;
        SetNextLineIndicator(false);
    }

    public void Advance()
    {
        if (!isActiveAndEnabled || isFinished || entries.Count == 0 || lineTMP == null)
        {
            return;
        }

        if (typingCoroutine != null)
        {
            StopTyping();
            RevealCurrentLine();
            return;
        }

        if (audioSource != null && advanceSound != null)
        {
            audioSource.PlayOneShot(advanceSound);
        }

        if (currentIndex + 1 >= entries.Count)
        {
            FinishDialogue();
            return;
        }

        currentIndex++;
        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        if (entries.Count == 0 || speakerTMP == null || lineTMP == null)
        {
            return;
        }

        StopTyping();
        SetNextLineIndicator(false);
        speakerTMP.text = entries[currentIndex].Speaker;
        lineTMP.text = entries[currentIndex].Line;
        lineTMP.maxVisibleCharacters = 0;
        lineTMP.ForceMeshUpdate();

        if (useTypewriter && typeSpeed > 0f && lineTMP.textInfo.characterCount > 0)
        {
            typingCoroutine = StartCoroutine(TypeCurrentLine());
        }
        else
        {
            RevealCurrentLine();
        }
    }

    private IEnumerator TypeCurrentLine()
    {
        int characterCount = lineTMP.textInfo.characterCount;
        var characterDelay = new WaitForSeconds(typeSpeed);
        for (int visibleCount = 1; visibleCount <= characterCount; visibleCount++)
        {
            lineTMP.maxVisibleCharacters = visibleCount;
            yield return characterDelay;
        }

        typingCoroutine = null;
        RevealCurrentLine();
    }

    private void RevealCurrentLine()
    {
        // TMP counts rendered characters, excluding rich-text markup.
        lineTMP.maxVisibleCharacters = int.MaxValue;
        SetNextLineIndicator(true);
    }

    private void FinishDialogue()
    {
        // Mark completion before activation callbacks can attempt to advance again.
        isFinished = true;
        SetNextLineIndicator(false);
        SceneObjectState.SetActive(objectsThatEnableAfterDialogue, true);
        SceneObjectState.SetActive(objectsThatDisableAfterDialogue, false);
    }

    private void SetNextLineIndicator(bool visible)
    {
        if (nextLineIndicator != null)
        {
            nextLineIndicator.SetActive(visible);
        }
    }

    private void StopTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
    }

    private void StopLoading()
    {
        if (loadingCoroutine != null)
        {
            StopCoroutine(loadingCoroutine);
            loadingCoroutine = null;
        }
    }
}
