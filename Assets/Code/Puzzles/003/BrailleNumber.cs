using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class BrailleNumber: MonoBehaviour
{
    [SerializeField] private BrailleDot[] brailleDots = new BrailleDot[6]; // Array of 6 dots
    [SerializeField] private TMPro.TextMeshPro displayText; // Optional UI text to show the number
    
    // Braille number patterns (dots 1-6 mapped to array indices 0-5)
    // Standard Braille numbering: dot 1=top-left, 2=middle-left, 3=bottom-left, 4=top-right, 5=middle-right, 6=bottom-right
    private readonly Dictionary<string, int> brailleToNumber = new Dictionary<string, int>
    {
        {"100000", 1}, // dot 1 only
        {"101000", 2}, // dots 1,2
        {"110000", 3}, // dots 1,4
        {"110100", 4}, // dots 1,4,5
        {"100100", 5}, // dots 1,5
        {"111000", 6}, // dots 1,2,4
        {"111100", 7}, // dots 1,2,4,5
        {"101100", 8}, // dots 1,2,5
        {"011000", 9}, // dots 2,4
        {"011100", 0}  // dots 2,4,5
    };
    
    private void Start()
    {
        // Subscribe to dot toggle events
        for (int i = 0; i < brailleDots.Length; i++)
        {
            if (brailleDots[i] != null)
            {
                brailleDots[i].OnDotToggled += OnDotToggled;
            }
        }
        
        // Initial interpretation
        InterpretDots();
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        for (int i = 0; i < brailleDots.Length; i++)
        {
            if (brailleDots[i] != null)
            {
                brailleDots[i].OnDotToggled -= OnDotToggled;
            }
        }
    }
    
    private void OnDotToggled(int dotIndex, bool isOn)
    {
        InterpretDots();
    }
    
    private void InterpretDots()
    {
        string pattern = GetCurrentPattern();
        int number = GetNumberFromPattern(pattern);
        
        Debug.Log($"Braille Pattern: {pattern} = Number: {(number >= 0 ? number.ToString() : "Invalid")}");
        
        // Update UI if available
        if (displayText != null)
        {
            if (number >= 0)
            {
                displayText.text = number.ToString();
            }
            else
            {
                displayText.text = "?";
            }
        }
    }
    
    private string GetCurrentPattern()
    {
        string pattern = "";
        for (int i = 0; i < 6; i++)
        {
            if (brailleDots[i] != null)
            {
                pattern += brailleDots[i].GetState() ? "1" : "0";
            }
            else
            {
                pattern += "0";
            }
        }
        return pattern;
    }
    
    private int GetNumberFromPattern(string pattern)
    {
        if (brailleToNumber.ContainsKey(pattern))
        {
            return brailleToNumber[pattern];
        }
        return -1; // Invalid pattern
    }
    
    // Public method to get the current number
    public int GetCurrentNumber()
    {
        string pattern = GetCurrentPattern();
        return GetNumberFromPattern(pattern);
    }
    
    // Public method to check if current pattern is valid
    public bool IsValidNumber()
    {
        return GetCurrentNumber() >= 0;
    }
    
    // Method to set a specific number pattern
    public void SetNumber(int number)
    {
        if (number < 0 || number > 9) return;
        
        string targetPattern = "";
        foreach (var kvp in brailleToNumber)
        {
            if (kvp.Value == number)
            {
                targetPattern = kvp.Key;
                break;
            }
        }
        
        if (!string.IsNullOrEmpty(targetPattern))
        {
            for (int i = 0; i < 6; i++)
            {
                if (brailleDots[i] != null)
                {
                    bool shouldBeOn = targetPattern[i] == '1';
                    brailleDots[i].SetState(shouldBeOn);
                }
            }
        }
    }
    
    // Method to clear all dots
    public void ClearDots()
    {
        for (int i = 0; i < brailleDots.Length; i++)
        {
            if (brailleDots[i] != null)
            {
                brailleDots[i].SetState(false);
            }
        }
    }
}