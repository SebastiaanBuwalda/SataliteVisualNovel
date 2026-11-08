using System.Collections.Generic;
using UnityEngine;

/// <summary>Applies Inspector-configured scene changes, ignoring unassigned entries.</summary>
public static class SceneObjectState
{
    public static void SetActive(IEnumerable<GameObject> objects, bool active)
    {
        if (objects == null)
        {
            return;
        }

        foreach (GameObject sceneObject in objects)
        {
            if (sceneObject != null)
            {
                sceneObject.SetActive(active);
            }
        }
    }

    public static void Toggle(IEnumerable<GameObject> objects)
    {
        if (objects == null)
        {
            return;
        }

        foreach (GameObject sceneObject in objects)
        {
            if (sceneObject != null)
            {
                sceneObject.SetActive(!sceneObject.activeSelf);
            }
        }
    }
}
