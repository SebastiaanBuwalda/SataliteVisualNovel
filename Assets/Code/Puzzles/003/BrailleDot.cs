using UnityEngine;
using System;
[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class BrailleDot : MonoBehaviour
{
    [SerializeField] private int dotIndex; // 0..5 (dots 1..6 mapped to 0..5)
    [SerializeField] private Sprite offSprite;
    [SerializeField] private Sprite onSprite;
    private SpriteRenderer sr;
    [SerializeField] private bool isOn;
    private bool initialState; // Store the initial state
    
    public event Action<int, bool> OnDotToggled; // index, newState
    
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        initialState = isOn; // Store the initial state from inspector
        UpdateVisual();
    }
    
    private void OnMouseDown()
    {
        // Ensure the click is on this object's collider
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.OverlapPoint(mousePos);
        if (hit != null && hit.gameObject == gameObject)
        {
            Toggle();
        }
    }
    
    public void Toggle()
    {
        isOn = !isOn;
        UpdateVisual();
        OnDotToggled?.Invoke(dotIndex, isOn);
    }
    
    public void SetState(bool on)
    {
        isOn = on;
        UpdateVisual();
    }
    
    public bool GetState() => isOn;
    
    public void ResetToInitial()
    {
        isOn = initialState;
        UpdateVisual();
        // Note: We don't invoke OnDotToggled here to avoid counting reset as a move
    }
    
    private void UpdateVisual()
    {
        sr.sprite = isOn ? onSprite : offSprite;
    }
}