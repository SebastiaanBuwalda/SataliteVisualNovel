using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BrailleNumber : MonoBehaviour
{
    [Tooltip("Top-left, top-right, middle-left, middle-right, bottom-left, bottom-right.")]
    [SerializeField] private BrailleDot[] brailleDots = new BrailleDot[BrailleDigitPatterns.DotCount];
    [SerializeField] private TextMeshPro displayText;

    public IEnumerable<BrailleDot> Dots => brailleDots;

    private void OnEnable()
    {
        if (brailleDots != null)
        {
            foreach (BrailleDot dot in brailleDots)
            {
                if (dot != null)
                {
                    dot.StateChanged += RefreshDisplay;
                }
            }
        }

        RefreshDisplay();
    }

    private void OnDisable()
    {
        if (brailleDots == null)
        {
            return;
        }

        foreach (BrailleDot dot in brailleDots)
        {
            if (dot != null)
            {
                dot.StateChanged -= RefreshDisplay;
            }
        }
    }

    public void RefreshDisplay()
    {
        if (displayText != null)
        {
            int number = GetCurrentNumber();
            displayText.text = number >= 0 ? number.ToString() : "?";
        }
    }

    public int GetCurrentNumber()
    {
        if (brailleDots == null || brailleDots.Length != BrailleDigitPatterns.DotCount)
        {
            return -1;
        }

        var pattern = new char[BrailleDigitPatterns.DotCount];
        for (int index = 0; index < pattern.Length; index++)
        {
            // A missing dot is a configuration error, not an OFF dot.
            if (brailleDots[index] == null)
            {
                return -1;
            }

            pattern[index] = brailleDots[index].GetState() ? '1' : '0';
        }

        return BrailleDigitPatterns.GetNumber(new string(pattern));
    }

    public bool IsValidNumber() => GetCurrentNumber() >= 0;

    public void SetNumber(int number)
    {
        string pattern = BrailleDigitPatterns.GetPattern(number);
        if (pattern == null || brailleDots == null || brailleDots.Length != pattern.Length)
        {
            return;
        }

        for (int index = 0; index < pattern.Length; index++)
        {
            if (brailleDots[index] != null)
            {
                brailleDots[index].SetState(pattern[index] == '1');
            }
        }

        RefreshDisplay();
    }

    public void ClearDots()
    {
        if (brailleDots == null)
        {
            return;
        }

        foreach (BrailleDot dot in brailleDots)
        {
            if (dot != null)
            {
                dot.SetState(false);
            }
        }

        RefreshDisplay();
    }
}
