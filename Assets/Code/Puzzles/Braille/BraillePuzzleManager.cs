using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>Multiply two Braille digits within a move budget; retry from the initial dots.</summary>
public class BraillePuzzleManager : MonoBehaviour
{
    [Header("Braille Interpreters")]
    [SerializeField] private BrailleNumber firstNumber;
    [SerializeField] private BrailleNumber secondNumber;

    [Header("UI")]
    [SerializeField] private TextMeshPro answerText;
    [SerializeField] private TextMeshPro movesText;
    [Tooltip("Optional Canvas UI button. World-space collider buttons use BrailleResetButton.")]
    [SerializeField] private Button resetButton;

    [Header("Puzzle Settings")]
    [SerializeField, Min(1)] private int maxMoves = 20;
    [SerializeField] private int correctAnswer = 35;
    [SerializeField] private UnityEvent onCompleted = new UnityEvent();

    [Header("Colors")]
    [SerializeField] private Color normalTextColor = Color.white;
    [SerializeField] private Color correctAnswerColor = Color.green;
    [SerializeField] private Color invalidAnswerColor = Color.red;

    private readonly HashSet<BrailleDot> allDots = new HashSet<BrailleDot>();
    private int remainingMoves;
    private bool puzzleCompleted;
    private bool hasStarted;
    private bool displayDirty;

    private void OnEnable()
    {
        CollectDots(firstNumber);
        CollectDots(secondNumber);
        foreach (BrailleDot dot in allDots)
        {
            dot.OnDotToggled += OnAnyDotToggled;
            dot.StateChanged += OnDotStateChanged;
        }

        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetPuzzle);
        }

        if (hasStarted)
        {
            RefreshPuzzle();
        }
    }

    private void Start()
    {
        hasStarted = true;
        remainingMoves = Mathf.Max(1, maxMoves);
        RefreshPuzzle();
    }

    private void OnDisable()
    {
        foreach (BrailleDot dot in allDots)
        {
            if (dot != null)
            {
                dot.OnDotToggled -= OnAnyDotToggled;
                dot.StateChanged -= OnDotStateChanged;
            }
        }

        allDots.Clear();
        if (resetButton != null)
        {
            resetButton.onClick.RemoveListener(ResetPuzzle);
        }
    }

    private void CollectDots(BrailleNumber number)
    {
        if (number == null || number.Dots == null)
        {
            return;
        }

        foreach (BrailleDot dot in number.Dots)
        {
            if (dot != null)
            {
                allDots.Add(dot);
            }
        }
    }

    private void OnAnyDotToggled(int dotIndex, bool isOn)
    {
        if (puzzleCompleted)
        {
            return;
        }

        remainingMoves = Mathf.Max(0, remainingMoves - 1);
        RefreshPuzzle();
        // A solution on the final allowed move takes precedence over resetting.
        if (remainingMoves == 0 && !puzzleCompleted)
        {
            ResetPuzzle();
        }
    }

    private void OnDotStateChanged() => displayDirty = true;

    private void LateUpdate()
    {
        // Batch programmatic changes (SetNumber, ClearDots) without charging moves.
        if (displayDirty)
        {
            RefreshPuzzle();
        }
    }

    private void RefreshPuzzle()
    {
        displayDirty = false;
        int first = firstNumber != null ? firstNumber.GetCurrentNumber() : -1;
        int second = secondNumber != null ? secondNumber.GetCurrentNumber() : -1;
        bool valid = first >= 0 && second >= 0;
        int result = valid ? first * second : 0;
        bool solved = valid && result == correctAnswer;

        if (movesText != null)
        {
            movesText.text = $"Moves: {remainingMoves}";
        }

        if (answerText != null)
        {
            string left = first >= 0 ? first.ToString() : "?";
            string right = second >= 0 ? second.ToString() : "?";
            string product = valid ? result.ToString() : "?";
            answerText.text = $"{left} × {right} = {product}";
            answerText.color = !valid ? invalidAnswerColor : solved ? correctAnswerColor : normalTextColor;
        }

        // Completion must work even when no answer text is assigned.
        if (solved && !puzzleCompleted)
        {
            puzzleCompleted = true;
            SetDotsInteractable(false);
            onCompleted.Invoke();
        }
    }

    public void ResetPuzzle()
    {
        puzzleCompleted = false;
        remainingMoves = Mathf.Max(1, maxMoves);
        foreach (BrailleDot dot in allDots)
        {
            if (dot != null)
            {
                dot.ResetToInitial();
            }
        }

        SetDotsInteractable(true);
        RefreshPuzzle();
    }

    private void SetDotsInteractable(bool interactable)
    {
        foreach (BrailleDot dot in allDots)
        {
            if (dot != null)
            {
                dot.IsInteractable = interactable;
            }
        }
    }

    public bool IsPuzzleCompleted() => puzzleCompleted;
    public int GetRemainingMoves() => remainingMoves;
    public int GetCorrectAnswer() => correctAnswer;

    public void SetCorrectAnswer(int newAnswer)
    {
        correctAnswer = newAnswer;
        puzzleCompleted = false;
        SetDotsInteractable(true);
        RefreshPuzzle();
    }

    public void SetMaxMoves(int newMaxMoves)
    {
        maxMoves = Mathf.Max(1, newMaxMoves);
        if (!puzzleCompleted)
        {
            remainingMoves = maxMoves;
            RefreshPuzzle();
        }
    }
}
