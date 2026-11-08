using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>Controls the three-slot Roman numeral puzzle and its story transition.</summary>
public class RomanNumeralPuzzleManager : MonoBehaviour
{
    [Header("Puzzle Settings")]
    [SerializeField] private int targetValue = 30;
    [SerializeField, Min(0f)] private float completionDelay = 3f;

    [Header("Objects To Destroy On Success")]
    [SerializeField] private List<GameObject> objectsToDestroy = new List<GameObject>();

    [Header("Objects To Activate On Success")]
    [SerializeField] private List<GameObject> objectsToLoad = new List<GameObject>();

    [Header("UI")]
    [SerializeField] private TextMeshPro puzzleText;
    [SerializeField] private Color invalidColor = Color.red;
    [SerializeField] private Color solvedColor = Color.green;

    private readonly int?[] slotValues = new int?[3];
    private readonly RomanNumeralSlotReporter[] slotOccupants = new RomanNumeralSlotReporter[3];
    private bool puzzleSolved;
    private bool displayDirty = true;

    private void LateUpdate()
    {
        // All tile reporters must finish updating before evaluating the board.
        if (puzzleSolved || !displayDirty)
        {
            return;
        }

        displayDirty = false;
        bool hasAllTiles = RomanNumeralRules.TryEvaluate(
            slotValues[0], slotValues[1], slotValues[2], out int result);
        bool solved = hasAllTiles && result == targetValue;
        UpdatePuzzleText(hasAllTiles, result, solved);

        if (solved)
        {
            puzzleSolved = true;
            StartCoroutine(PuzzleTransition.CompleteAfterDelay(
                completionDelay, objectsToDestroy, objectsToLoad));
        }
    }

    private void UpdatePuzzleText(bool hasAllTiles, int result, bool solved)
    {
        if (puzzleText == null)
        {
            return;
        }

        if (!hasAllTiles)
        {
            puzzleText.text = $"INVALID = {targetValue}";
        }
        else if (solved)
        {
            string formula = slotValues[1] == 10
                ? $"{slotValues[0]} × {slotValues[2]}"
                : $"{slotValues[0]} + {slotValues[1]} + {slotValues[2]}";
            puzzleText.text = $"{formula} = {targetValue}";
        }
        else
        {
            puzzleText.text = $"{result} ≠ {targetValue}";
        }

        puzzleText.color = solved ? solvedColor : invalidColor;
    }

    public void SetSlotOccupant(RomanNumeralSlotReporter.SlotType slot, RomanNumeralSlotReporter tile, int value)
    {
        int index = (int)slot;
        slotOccupants[index] = tile;
        SetSlotValue(index, value);
    }

    public void ClearSlotOccupant(RomanNumeralSlotReporter.SlotType slot, RomanNumeralSlotReporter tile)
    {
        int index = (int)slot;
        // A departing tile must not erase a replacement tile's value in the same frame.
        if (slotOccupants[index] == tile)
        {
            slotOccupants[index] = null;
            SetSlotValue(index, null);
        }
    }

    // Kept as public entry points for existing Inspector events or other scripts.
    public void SetSlotAValue(int value) => SetSlotValue(0, value);
    public void SetSlotBValue(int value) => SetSlotValue(1, value);
    public void SetSlotCValue(int value) => SetSlotValue(2, value);

    private void SetSlotValue(int index, int? value)
    {
        slotValues[index] = value;
        displayDirty = true;
    }
}
