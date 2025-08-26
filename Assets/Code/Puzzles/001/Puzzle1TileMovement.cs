using UnityEngine;

public class Puzzle1TileMovement : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private Vector3 originalScale;
    private Vector3 originalPosition;
    public float gridSize = 1f;
    public float growthFactor = 1.2f;

    private SpriteRenderer spriteRenderer;
    private int originalSortingOrder;
    public int dragSortingOrder = 10; // Set higher than default

    [SerializeField] private AudioClip pickUpSound;
    [SerializeField] private AudioClip putDownSound;
   [SerializeField] private AudioSource audioSource;

    void Start()
    {
        originalScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalSortingOrder = spriteRenderer.sortingOrder;
        }
    }

    void OnMouseDown()
    {
        isDragging = true;
        originalPosition = transform.position;
        audioSource.PlayOneShot(pickUpSound);
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        offset = transform.position - mouseWorldPos;

        transform.localScale = originalScale * growthFactor;

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = dragSortingOrder;
        }
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
        audioSource.PlayOneShot(putDownSound);
        // Snap to nearest grid point
        float x = Mathf.Round(transform.position.x / gridSize) * gridSize;
        float y = Mathf.Round(transform.position.y / gridSize) * gridSize;
        Vector3 snappedPosition = new Vector3(x, y, 0f);

        // Check for collision with other tiles
        bool positionOccupied = false;
        foreach (var tile in FindObjectsByType<Puzzle1TileMovement>(FindObjectsSortMode.None))
        {
            if (tile != this && tile.transform.position == snappedPosition)
            {
                positionOccupied = true;
                break;
            }
        }

        if (positionOccupied)
        {
            // Another tile is at the location, return to original position
            transform.position = originalPosition;
        }
        else
        {
            // No tile at location, snap to grid
            transform.position = snappedPosition;
        }

        transform.localScale = originalScale;

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = originalSortingOrder;
        }
    }
}
