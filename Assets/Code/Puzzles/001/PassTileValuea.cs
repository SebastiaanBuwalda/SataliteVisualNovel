using UnityEngine;

public class PassTileValuea : MonoBehaviour
{
    [SerializeField] private Vector3 neededPosition;
    [SerializeField] private PuzzleManager puzzleManager;

    private int tileValue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tileValue = GetComponent<NumericalValue>().Value;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position == neededPosition)
        {
            if (puzzleManager != null)
            {
                puzzleManager.SetSlotAValue(tileValue);
            }
        }
    }
}
