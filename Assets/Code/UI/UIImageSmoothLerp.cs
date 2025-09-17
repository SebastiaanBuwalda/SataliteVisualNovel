using UnityEngine;
using UnityEngine.UI;

public class UIImageSmoothLerp : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector2 startPosition = new Vector2(-200f, 0f);
    public Vector2 endPosition = new Vector2(200f, 0f);
    public float movementSpeed = 2f;
    public AnimationCurve movementCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    
    [Header("Options")]
    public bool useLocalPosition = true;
    public bool startMovingOnAwake = true;
    public bool pingPongMovement = true;
    
    private RectTransform rectTransform;
    private Vector2 currentStartPos;
    private Vector2 currentEndPos;
    private float currentTime = 0f;
    private bool movingForward = true;
    private bool isMoving = false;
    
    void Awake()
    {
        // Get the RectTransform component
        rectTransform = GetComponent<RectTransform>();
        
        if (rectTransform == null)
        {
            Debug.LogError("SmoothUIMovement: No RectTransform found on " + gameObject.name);
            enabled = false;
            return;
        }
        
        // Set initial positions
        currentStartPos = startPosition;
        currentEndPos = endPosition;
        
        // Set initial position
        if (useLocalPosition)
            rectTransform.anchoredPosition = currentStartPos;
        else
            rectTransform.position = currentStartPos;
    }
    
    void Start()
    {
        if (startMovingOnAwake)
        {
            StartMovement();
        }
    }
    
    void Update()
    {
        if (!isMoving) return;
        
        // Update time based on movement speed
        currentTime += Time.deltaTime * movementSpeed;
        
        // Calculate interpolation value using the animation curve
        float t = movementCurve.Evaluate(currentTime);
        
        // Determine current start and end positions based on direction
        Vector2 fromPos = movingForward ? currentStartPos : currentEndPos;
        Vector2 toPos = movingForward ? currentEndPos : currentStartPos;
        
        // Interpolate between positions
        Vector2 newPosition = Vector2.Lerp(fromPos, toPos, t);
        
        // Apply the new position
        if (useLocalPosition)
            rectTransform.anchoredPosition = newPosition;
        else
            rectTransform.position = newPosition;
        
        // Check if we've reached the end of the current movement
        if (currentTime >= 1f)
        {
            // Ensure we're exactly at the target position
            if (useLocalPosition)
                rectTransform.anchoredPosition = toPos;
            else
                rectTransform.position = toPos;
            
            if (pingPongMovement)
            {
                // Switch direction and reset time
                movingForward = !movingForward;
                currentTime = 0f;
            }
            else
            {
                // Stop movement
                isMoving = false;
                currentTime = 0f;
            }
        }
    }
    
    /// <summary>
    /// Start the movement animation
    /// </summary>
    public void StartMovement()
    {
        isMoving = true;
        currentTime = 0f;
        movingForward = true;
    }
    
    /// <summary>
    /// Stop the movement animation
    /// </summary>
    public void StopMovement()
    {
        isMoving = false;
    }
    
    /// <summary>
    /// Pause/Resume the movement animation
    /// </summary>
    public void ToggleMovement()
    {
        isMoving = !isMoving;
    }
    
    /// <summary>
    /// Reset to start position
    /// </summary>
    public void ResetToStart()
    {
        currentTime = 0f;
        movingForward = true;
        
        if (useLocalPosition)
            rectTransform.anchoredPosition = currentStartPos;
        else
            rectTransform.position = currentStartPos;
    }
    
    /// <summary>
    /// Set new positions during runtime
    /// </summary>
    public void SetPositions(Vector2 newStartPos, Vector2 newEndPos)
    {
        currentStartPos = newStartPos;
        currentEndPos = newEndPos;
        startPosition = newStartPos;
        endPosition = newEndPos;
    }
    
    /// <summary>
    /// Set movement speed during runtime
    /// </summary>
    public void SetSpeed(float newSpeed)
    {
        movementSpeed = Mathf.Max(0.1f, newSpeed);
    }
    
    // Gizmos for visualizing positions in the scene view
    void OnDrawGizmosSelected()
    {
        if (rectTransform == null) return;
        
        Gizmos.color = Color.green;
        Vector3 worldStartPos = useLocalPosition ? 
            transform.TransformPoint(startPosition) : 
            startPosition;
        Gizmos.DrawWireSphere(worldStartPos, 10f);
        
        Gizmos.color = Color.red;
        Vector3 worldEndPos = useLocalPosition ? 
            transform.TransformPoint(endPosition) : 
            endPosition;
        Gizmos.DrawWireSphere(worldEndPos, 10f);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(worldStartPos, worldEndPos);
    }
}