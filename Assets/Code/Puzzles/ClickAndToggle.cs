using UnityEngine;

public class ClickAndToggle : MonoBehaviour
{
    [Header("Objects to Enable on Click")]
    [SerializeField] private GameObject[] objectsToEnable;
    
    [Header("Objects to Disable on Click")]
    [SerializeField] private GameObject[] objectsToDisable;
    
    [Header("Optional Settings")]
    [SerializeField] private bool toggleMode = false; // If true, clicking again will reverse the action
    [SerializeField] private LayerMask clickableLayer = -1; // Layer mask for raycast filtering
    
    private bool isToggled = false;
    private Camera mainCamera;
    
    void Start()
    {
        // Get the main camera for raycasting
        mainCamera = Camera.main;
        // Ensure this object has a 2D collider
        if (GetComponent<Collider2D>() == null)
        {
            Debug.LogWarning($"ClickableObjectController on {gameObject.name} requires a 2D Collider component!");
        }
    }
    
    void Update()
    {
        // Check for mouse click
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }
    
    private void HandleClick()
    {
        // Convert mouse position to world position
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        
        // Raycast to check if we clicked on this object's collider
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, Mathf.Infinity, clickableLayer);
        
        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            if (toggleMode && isToggled)
            {
                // Reverse the action if in toggle mode and already toggled
                ReverseToggle();
            }
            else
            {
                // Perform the main action
                PerformAction();
            }
        }
    }
    
    private void PerformAction()
    {
        // Enable specified objects
        foreach (GameObject obj in objectsToEnable)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
        
        // Disable specified objects
        foreach (GameObject obj in objectsToDisable)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
        
        if (toggleMode)
        {
            isToggled = true;
        }
        
        //Debug.Log($"Clicked on {gameObject.name} - Objects toggled!");
    }
    
    private void ReverseToggle()
    {
        // Reverse the previous action
        foreach (GameObject obj in objectsToEnable)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
        
        foreach (GameObject obj in objectsToDisable)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
        
        isToggled = false;
        
        //Debug.Log($"Reversed toggle on {gameObject.name}!");
    }
    
    // Public method to manually trigger the action (can be called from other scripts)
    public void TriggerAction()
    {
        if (toggleMode && isToggled)
        {
            ReverseToggle();
        }
        else
        {
            PerformAction();
        }
    }
    
    // Public method to reset the toggle state
    public void ResetToggle()
    {
        if (isToggled)
        {
            ReverseToggle();
        }
    }
    
    
    private void HandleClickDirect()
    {
        if (toggleMode && isToggled)
        {
            ReverseToggle();
        }
        else
        {
            PerformAction();
        }
    }
}