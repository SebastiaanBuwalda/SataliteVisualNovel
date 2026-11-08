using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>Checks the tile arrangement for the seven-segment counting puzzle.</summary>
public class SevenSegmentPuzzleManager : MonoBehaviour
{
    [Serializable]
    public class ObjectLocation
    {
        public GameObject obj;
        public Vector3 targetPosition;
        [Min(0f)] public float tolerance = 0.1f;
    }

    [Header("Objects and Target Locations")]
    [SerializeField] private List<ObjectLocation> objectsWithLocations = new List<ObjectLocation>();

    [Header("Completion")]
    [SerializeField, Min(0f)] private float completionDelay = 3f;
    [SerializeField] private List<GameObject> objectsToDestroy = new List<GameObject>();
    [SerializeField] private List<GameObject> objectsToLoad = new List<GameObject>();

    [Header("UI")]
    [SerializeField] private TextMeshPro puzzleText;
    [SerializeField] private string solvedText = "Correct!";

    private bool puzzleSolved;

    private void LateUpdate()
    {
        // OnMouseUp snaps tiles before LateUpdate checks their final locations.
        if (!puzzleSolved && Input.GetMouseButtonUp(0))
        {
            CheckObjectsAtLocations();
        }
    }

    private void CheckObjectsAtLocations()
    {
        if (objectsWithLocations == null || objectsWithLocations.Count == 0)
        {
            return;
        }

        foreach (ObjectLocation location in objectsWithLocations)
        {
            if (location == null || location.obj == null)
            {
                return;
            }

            float tolerance = Mathf.Max(0f, location.tolerance);
            if ((location.obj.transform.position - location.targetPosition).sqrMagnitude > tolerance * tolerance)
            {
                return;
            }
        }

        puzzleSolved = true;
        if (puzzleText != null)
        {
            puzzleText.text = solvedText;
        }

        StartCoroutine(PuzzleTransition.CompleteAfterDelay(
            completionDelay, objectsToDestroy, objectsToLoad));
    }
}
