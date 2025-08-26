using UnityEngine;

public class PassTileValue : MonoBehaviour
{
    [SerializeField] private Vector3 neededPosition;
    [SerializeField] private PuzzleManager puzzleManager;
    public enum SlotType { A, B, C }
    [SerializeField] private SlotType slotType;
    private int tileValue;
    private bool valueSet = false;

    void Start()
    {
        tileValue = GetComponent<NumericalValue>().Value;
    }

    void Update()
    {
        if (transform.position == neededPosition)
        {
            if (!valueSet)
            {
                if (slotType == SlotType.C)
                    puzzleManager.SetSlotCValue(tileValue);
                else if (slotType == SlotType.B)
                    puzzleManager.SetSlotBValue(tileValue);
                else if (slotType == SlotType.A)
                    puzzleManager.SetSlotAValue(tileValue);

                valueSet = true;
            }
        }
        else
        {
            if (valueSet)
            {
                if (slotType == SlotType.C)
                    puzzleManager.SetSlotCValue(0);
                else if (slotType == SlotType.B)
                    puzzleManager.SetSlotBValue(0);
                else if (slotType == SlotType.A)
                    puzzleManager.SetSlotAValue(0);

                valueSet = false;
            }
        }
    }
}
