using System.Collections.Generic;
using UnityEngine;

/// <summary>Shared grid dragging for the Roman numeral and seven-segment puzzles.</summary>
[RequireComponent(typeof(Collider2D))]
public class GridTileDrag : MonoBehaviour
{
    [Header("Grid and Drag Appearance")]
    [SerializeField, Min(0.01f)] private float gridSize = 1f;
    [SerializeField, Min(0f)] private float growthFactor = 1.2f;
    [SerializeField] private int dragSortingOrder = 10;

    [Header("Audio")]
    [SerializeField] private AudioClip pickUpSound;
    [SerializeField] private AudioClip putDownSound;
    [SerializeField] private AudioSource audioSource;

    private static readonly HashSet<GridTileDrag> ActiveTiles = new HashSet<GridTileDrag>();
    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;
    private Vector3 dragOffset;
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private int originalSortingOrder;

    public bool IsDragging { get; private set; }

    private void Awake()
    {
        originalScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalSortingOrder = spriteRenderer.sortingOrder;
        }
    }

    private void OnEnable() => ActiveTiles.Add(this);

    private void OnDisable()
    {
        ActiveTiles.Remove(this);
        if (IsDragging)
        {
            transform.position = originalPosition;
            RestoreAppearance();
        }
    }

    private void OnMouseDown()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            return;
        }

        IsDragging = true;
        originalPosition = transform.position;
        dragOffset = originalPosition - GetMouseWorldPosition();
        transform.localScale = originalScale * growthFactor;
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = dragSortingOrder;
        }

        PlaySound(pickUpSound);
    }

    private void OnMouseDrag()
    {
        if (IsDragging && mainCamera != null)
        {
            transform.position = GetMouseWorldPosition() + dragOffset;
        }
    }

    private void OnMouseUp()
    {
        if (!IsDragging)
        {
            return;
        }

        float size = Mathf.Max(0.01f, gridSize);
        var snappedPosition = new Vector3(
            Mathf.Round(transform.position.x / size) * size,
            Mathf.Round(transform.position.y / size) * size,
            originalPosition.z);

        transform.position = IsOccupied(snappedPosition) ? originalPosition : snappedPosition;
        RestoreAppearance();
        PlaySound(putDownSound);
    }

    private bool IsOccupied(Vector3 position)
    {
        foreach (GridTileDrag tile in ActiveTiles)
        {
            if (tile != null && tile != this &&
                (tile.transform.position - position).sqrMagnitude < 0.0001f)
            {
                return true;
            }
        }

        return false;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 position = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        position.z = originalPosition.z;
        return position;
    }

    private void RestoreAppearance()
    {
        IsDragging = false;
        transform.localScale = originalScale;
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = originalSortingOrder;
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
