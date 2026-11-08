using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class BrailleDot : MonoBehaviour
{
    [Tooltip("Index in row order: top-left, top-right, middle-left, middle-right, bottom-left, bottom-right.")]
    [SerializeField, Range(0, 5)] private int dotIndex;
    [SerializeField] private Sprite offSprite;
    [SerializeField] private Sprite onSprite;
    [SerializeField] private bool isOn;

    private SpriteRenderer spriteRenderer;
    private bool initialState;

    public bool IsInteractable { get; set; } = true;

    /// <summary>Raised only by a player toggle, so resets do not consume moves.</summary>
    public event Action<int, bool> OnDotToggled;
    public event Action StateChanged;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialState = isOn;
        UpdateVisual();
    }

    // Unity already dispatches OnMouseDown through this object's collider.
    private void OnMouseDown() => Toggle();

    public void Toggle()
    {
        if (!IsInteractable)
        {
            return;
        }

        SetState(!isOn);
        OnDotToggled?.Invoke(dotIndex, isOn);
    }

    public void SetState(bool on)
    {
        if (isOn == on)
        {
            return;
        }

        isOn = on;
        UpdateVisual();
        StateChanged?.Invoke();
    }

    public bool GetState() => isOn;

    public void ResetToInitial() => SetState(initialState);

    private void UpdateVisual()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = isOn ? onSprite : offSprite;
        }
    }
}
