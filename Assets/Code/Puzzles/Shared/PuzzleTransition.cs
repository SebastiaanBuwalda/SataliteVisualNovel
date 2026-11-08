using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PuzzleTransition
{
    public static IEnumerator CompleteAfterDelay(float delay,
        IEnumerable<GameObject> objectsToDestroy, IEnumerable<GameObject> objectsToLoad)
    {
        yield return new WaitForSeconds(delay);

        // Destroy is deferred until the end of the frame, so activation still completes.
        if (objectsToDestroy != null)
        {
            foreach (GameObject sceneObject in objectsToDestroy)
            {
                if (sceneObject != null)
                {
                    Object.Destroy(sceneObject);
                }
            }
        }

        SceneObjectState.SetActive(objectsToLoad, true);
    }
}
