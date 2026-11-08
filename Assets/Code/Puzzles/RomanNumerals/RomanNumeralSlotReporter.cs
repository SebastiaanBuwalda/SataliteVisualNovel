using UnityEngine;

[RequireComponent(typeof(TileValue))]
public class RomanNumeralSlotReporter : MonoBehaviour
{
    public enum SlotType { A, B, C }

    [SerializeField] private Vector3 neededPosition;
    [SerializeField] private RomanNumeralPuzzleManager puzzleManager;
    [SerializeField] private SlotType slotType;
    [SerializeField, Min(0f)] private float positionTolerance = 0.01f;

    private TileValue numericalValue;
    private GridTileDrag tileMovement;
    private bool isInSlot;

    private void Awake()
    {
        numericalValue = GetComponent<TileValue>();
        tileMovement = GetComponent<GridTileDrag>();
    }

    private void Update()
    {
        if (puzzleManager == null)
        {
            return;
        }

        bool isAtTarget = (tileMovement == null || !tileMovement.IsDragging) &&
            (transform.position - neededPosition).sqrMagnitude <= positionTolerance * positionTolerance;
        if (isAtTarget == isInSlot)
        {
            return;
        }

        isInSlot = isAtTarget;
        if (isInSlot)
        {
            puzzleManager.SetSlotOccupant(slotType, this, numericalValue.Value);
        }
        else
        {
            puzzleManager.ClearSlotOccupant(slotType, this);
        }
    }

    private void OnDisable()
    {
        if (puzzleManager != null && isInSlot)
        {
            puzzleManager.ClearSlotOccupant(slotType, this);
        }

        isInSlot = false;
    }
}
