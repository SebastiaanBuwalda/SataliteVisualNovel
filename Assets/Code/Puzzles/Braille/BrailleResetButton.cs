using UnityEngine;

/// <summary>Connects a collider-based reset button to its Braille puzzle.</summary>
[RequireComponent(typeof(Collider2D))]
public class BrailleResetButton : MonoBehaviour
{
    [SerializeField] private BraillePuzzleManager puzzleManager;

    private void OnMouseDown()
    {
        if (puzzleManager != null)
        {
            puzzleManager.ResetPuzzle();
        }
    }
}
