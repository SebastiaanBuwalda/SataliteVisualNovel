using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PuzzleManager : MonoBehaviour
{
    private int? slotA = 0;
    private int? slotB = 0;
    private int? slotC= 0;

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
        if (!slotA.HasValue || !slotB.HasValue || !slotC.HasValue) return false;

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
        if (!slotA.HasValue || !slotB.HasValue || !slotC.HasValue)
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
            puzzleText.text = $"{result} ≠ {targetValue}";
            puzzleText.color = invalidColor;
        }
    }

    private void SolvePuzzle()
    {
        puzzleSolved = true;
        StartCoroutine(SolvePuzzleCoroutine());
        Debug.Log("Puzzle solved! Value reached: " + targetValue);
    }

    private System.Collections.IEnumerator SolvePuzzleCoroutine()
    {
        yield return new WaitForSeconds(3f);

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
    }

    // Allow external scripts to set slot values
    public void SetSlotAValue(int value)
    {
        slotA = value;
    }

    public void SetSlotBValue(int value)
    {
        slotB = value;
    }

    public void SetSlotCValue(int value)
    {
        slotC = value;
    }
}
