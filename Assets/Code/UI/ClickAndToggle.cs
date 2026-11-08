using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ClickAndToggle : MonoBehaviour
{
    [Header("Objects to Enable on Click")]
    [SerializeField] private GameObject[] objectsToEnable;

    [Header("Objects to Disable on Click")]
    [SerializeField] private GameObject[] objectsToDisable;

    [Header("Options")]
    [SerializeField] private bool toggleMode;
    [SerializeField] private LayerMask clickableLayer = -1;

    private bool isToggled;
    private Camera mainCamera;
    private int activationFrame;

    private void OnEnable()
    {
        mainCamera = Camera.main;
        activationFrame = Time.frameCount;
    }

    private void Update()
    {
        if (Time.frameCount <= activationFrame || !Input.GetMouseButtonDown(0))
        {
            return;
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera == null)
        {
            return;
        }

        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePosition, clickableLayer);
        if (hit != null && hit.gameObject == gameObject)
        {
            TriggerAction();
        }
    }

    public void TriggerAction()
    {
        bool reverse = toggleMode && isToggled;
        isToggled = toggleMode && !reverse;
        SceneObjectState.SetActive(objectsToEnable, !reverse);
        SceneObjectState.SetActive(objectsToDisable, reverse);
    }

    public void ResetToggle()
    {
        if (!isToggled)
        {
            return;
        }

        isToggled = false;
        SceneObjectState.SetActive(objectsToEnable, false);
        SceneObjectState.SetActive(objectsToDisable, true);
    }
}
