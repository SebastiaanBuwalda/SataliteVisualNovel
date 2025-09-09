using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class PuzzleManager002 : MonoBehaviour
{

    [Header("Objects To Destroy On Success")]
    [SerializeField] private List<GameObject> objectsToDestroy = new List<GameObject>();

    [Header("Objects To Load On Success")]
    [SerializeField] private List<GameObject> objectsToLoad = new List<GameObject>();

    [Header("UI")]
    [SerializeField] private TextMeshPro puzzleText;

    private bool puzzleSolved = false;
    [System.Serializable]
    public class ObjectLocation
    {
        public GameObject obj;
        public Vector3 targetPosition;
        public float tolerance = 0.1f;
    }

    [Header("Objects and Target Locations")]
    [SerializeField] private List<ObjectLocation> objectsWithLocations = new List<ObjectLocation>();

    [SerializeField] private string solvedText = "Correct!";
    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            CheckObjectsAtLocations();
        }
    }
    private void CheckObjectsAtLocations()
    {
        if (puzzleSolved) return;

        foreach (var item in objectsWithLocations)
        {
            if (item.obj == null) return;
            if (Vector3.Distance(item.obj.transform.position, item.targetPosition) > item.tolerance)
                return;
        }

        puzzleSolved = true;
        if (puzzleText != null)
        {
            puzzleText.text = solvedText;
        }
        StartCoroutine(SolvePuzzleCoroutine());
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
}
