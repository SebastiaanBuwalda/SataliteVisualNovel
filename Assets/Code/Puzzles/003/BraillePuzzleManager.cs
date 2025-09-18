using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BraillePuzzleManager : MonoBehaviour
{
    [Header("Braille Interpreters")]
    [SerializeField] private BrailleNumber firstNumber;
    [SerializeField] private BrailleNumber secondNumber;
    
    [Header("UI Elements")]
    [SerializeField] private TMPro.TextMeshPro answerText;
    [SerializeField] private TMPro.TextMeshPro movesText;
    [SerializeField] private Button resetButton;
    
    [Header("Puzzle Settings")]
    [SerializeField] private int maxMoves = 20;
    [SerializeField] private int correctAnswer = 35;
    
    [Header("Colors")]
    [SerializeField] private Color normalTextColor = Color.white;
    [SerializeField] private Color correctAnswerColor = Color.green;
    [SerializeField] private Color invalidAnswerColor = Color.red;
    
    private int currentMoves;
    private bool puzzleCompleted = false;
    private List<BrailleDot> allDots = new List<BrailleDot>();
    
    private void Start()
    {
        InitializePuzzle();
        SetupResetButton();
        CollectAllDots();
        SubscribeToAllDots();
        UpdateDisplay();
    }
    
    private void OnDestroy()
    {
        UnsubscribeFromAllDots();
    }
    
    private void InitializePuzzle()
    {
        currentMoves = maxMoves;
        puzzleCompleted = false;
        UpdateDisplay();
    }
    
    private void SetupResetButton()
    {
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetPuzzle);
        }
    }
    
    private void CollectAllDots()
    {
        allDots.Clear();
        
        // Collect dots from first number interpreter
        if (firstNumber != null)
        {
            BrailleDot[] dots1 = firstNumber.GetComponentsInChildren<BrailleDot>();
            allDots.AddRange(dots1);
        }
        
        // Collect dots from second number interpreter
        if (secondNumber != null)
        {
            BrailleDot[] dots2 = secondNumber.GetComponentsInChildren<BrailleDot>();
            allDots.AddRange(dots2);
        }
    }
    
    private void SubscribeToAllDots()
    {
        foreach (BrailleDot dot in allDots)
        {
            if (dot != null)
            {
                dot.OnDotToggled += OnAnyDotToggled;
            }
        }
    }
    
    private void UnsubscribeFromAllDots()
    {
        foreach (BrailleDot dot in allDots)
        {
            if (dot != null)
            {
                dot.OnDotToggled -= OnAnyDotToggled;
            }
        }
    }
    
    private void OnAnyDotToggled(int dotIndex, bool isOn)
    {
        if (puzzleCompleted) return;
        
        // Count every dot toggle as a move
        currentMoves--;
        currentMoves = Mathf.Max(0, currentMoves);
        
        UpdateDisplay();
        
        // Check if moves reached zero and puzzle not completed
        if (currentMoves <= 0 && !puzzleCompleted)
        {
            CheckGameOver();
        }
    }
    
    private void UpdateDisplay()
    {
        UpdateMovesDisplay();
        UpdateAnswerDisplay();
    }
    
    private void UpdateMovesDisplay()
    {
        if (movesText != null)
        {
            movesText.text = $"Moves: {currentMoves}";
        }
    }
    
    private void UpdateAnswerDisplay()
    {
        if (answerText == null) return;
        
        int num1 = firstNumber?.GetCurrentNumber() ?? -1;
        int num2 = secondNumber?.GetCurrentNumber() ?? -1;
        
        string displayText;
        Color textColor;
        
        // If either number is invalid, show ?
        if (num1 < 0 || num2 < 0)
        {
            displayText = $"{(num1 < 0 ? "?" : num1.ToString())} × {(num2 < 0 ? "?" : num2.ToString())} = ?";
            textColor = invalidAnswerColor;
        }
        else
        {
            int result = num1 * num2;
            displayText = $"{num1} × {num2} = {result}";
            
            // Check if answer is correct
            if (result == correctAnswer)
            {
                textColor = correctAnswerColor;
                if (!puzzleCompleted)
                {
                    CompletePuzzle();
                }
            }
            else
            {
                textColor = normalTextColor;
            }
        }
        
        answerText.text = displayText;
        answerText.color = textColor;
    }
    
    private void CompletePuzzle()
    {
        puzzleCompleted = true;
        Debug.Log("Puzzle Completed! Correct answer achieved!");
        
        // Optional: Add completion effects here
        // Like particle effects, sounds, etc.
    }
    
    private void CheckGameOver()
    {
        int num1 = firstNumber?.GetCurrentNumber() ?? -1;
        int num2 = secondNumber?.GetCurrentNumber() ?? -1;
        
        // If we have valid numbers, check the answer
        if (num1 >= 0 && num2 >= 0)
        {
            int result = num1 * num2;
            if (result == correctAnswer)
            {
                // Don't reset if the answer is correct - puzzle is completed!
                return;
            }
            else
            {
                Debug.Log("Game Over! Moves exhausted and answer is incorrect. Resetting...");
                ResetPuzzle();
            }
        }
        else
        {
            Debug.Log("Game Over! Moves exhausted and numbers are invalid. Resetting...");
            ResetPuzzle();
        }
    }
    
    public void ResetPuzzle()
    {
        Debug.Log("Resetting puzzle...");
        
        // Reset all dots to their OFF state (initial position)
        foreach (BrailleDot dot in allDots)
        {
            if (dot != null)
            {
                dot.ResetToInitial();
            }
        }
        
        // Reset game state
        currentMoves = maxMoves;
        puzzleCompleted = false;
        
        UpdateDisplay();
    }
    
    // Public methods for external access
    public bool IsPuzzleCompleted() => puzzleCompleted;
    public int GetRemainingMoves() => currentMoves;
    public int GetCorrectAnswer() => correctAnswer;
    
    // Method to set a different correct answer if needed
    public void SetCorrectAnswer(int newAnswer)
    {
        correctAnswer = newAnswer;
        UpdateDisplay();
    }
    
    // Method to set max moves if needed
    public void SetMaxMoves(int newMaxMoves)
    {
        maxMoves = newMaxMoves;
        if (!puzzleCompleted)
        {
            currentMoves = maxMoves;
            UpdateDisplay();
        }
    }
}