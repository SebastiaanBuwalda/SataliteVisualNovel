using UnityEngine;

public class Puzzle1TileMovement : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private Vector3 originalScale;
    public float gridSize = 1f; // size of each grid cell
    public float growthFactor = 1.2f; // factor by which the tile grows when picked up

    void Start()
    {
        originalScale = transform.localScale;
    }

    void OnMouseDown()
    {
        // Start dragging
        isDragging = true;
        
        // Save offset so object doesn't jump
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        offset = transform.position - mouseWorldPos;

        // Slightly grow
        transform.localScale = originalScale * growthFactor;
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;
            transform.position = mouseWorldPos + offset;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;

        // Snap to nearest grid point
        float x = Mathf.Round(transform.position.x / gridSize) * gridSize;
        float y = Mathf.Round(transform.position.y / gridSize) * gridSize;
        transform.position = new Vector3(x, y, 0f);

        // Return to normal scale
        transform.localScale = originalScale;
    }
}
