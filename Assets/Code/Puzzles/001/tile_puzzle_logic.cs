using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PuzzleManager : MonoBehaviour
{
    [Header("Puzzle Slots")]
    private NumericalValue slotA;
    private NumericalValue slotB;
    private NumericalValue slotC;

    [Header("Puzzle Settings")]
    [SerializeField] private int targetValue = 18;

    [Header("Objects To Destroy On Success")]
    [SerializeField] private List<GameObject> objectsToDestroy = new List<GameObject>();

    [Header("Objects To Load On Success")]
    [SerializeField] private List<GameObject> objectsToLoad = new List<GameObject>();

    [Header("UI")]
    [SerializeField] private TextMeshPro puzzleText;
    [SerializeField] private Color invalidColor = Color.red;
    [SerializeField] private Color solvedColor = Color.green;

    private bool puzzleSolved = false;

    private void Update()
    {
        if (puzzleSolved) return;

        UpdatePuzzleText();

        if (CheckPuzzleCondition())
        {
            SolvePuzzle();
        }
    }

    private bool CheckPuzzleCondition()
    {
        if (slotA == null || slotB == null || slotC == null) return false;

        int a = slotA.Value;
        int b = slotB.Value;
        int c = slotC.Value;

        int result;

        // Special case: middle slot is 10 (X in Roman numerals)
        if (b == 10)
        {
            result = a * c;
        }
        else
        {
            result = a + b + c;
        }

        return result == targetValue;
    }

    private void UpdatePuzzleText()
    {
        if (slotA == null || slotB == null || slotC == null)
        {
            puzzleText.text = $"INVALID = {targetValue}";
            puzzleText.color = invalidColor;
            return;
        }

        int a = slotA.Value;
        int b = slotB.Value;
        int c = slotC.Value;

        int result;
        string formula;
        print("yo");

        if (b == 10)
        {
            result = a * c;
            formula = $"{a} * {c}";
        }
        else
        {
            result = a + b + c;
            formula = $"{a} + {b} + {c}";
        }

        if (result == targetValue)
        {
            puzzleText.text = $"{formula} = {targetValue}";
            puzzleText.color = solvedColor;
        }
        else
        {
            puzzleText.text = $"{formula} = {targetValue}";
            puzzleText.color = invalidColor;
        }
    }

    private void SolvePuzzle()
    {
        puzzleSolved = true;

        // Destroy objects
        foreach (var obj in objectsToDestroy)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }

        // Load (activate) objects
        foreach (var obj in objectsToLoad)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }

        Debug.Log("Puzzle solved! Value reached: " + targetValue);
    }

    // Allow external scripts to set slot values
    public void SetSlotAValue(int value)
    {
        slotA.Value = value;
    }

    public void SetSlotBValue(int value)
    {
        slotB.Value = value;
    }

    public void SetSlotCValue(int value)
    {
       slotC.Value = value;
    }
}
