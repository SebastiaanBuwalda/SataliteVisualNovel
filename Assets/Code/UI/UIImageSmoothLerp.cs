using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIImageSmoothLerp : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Vector2 startPosition = new Vector2(-200f, 0f);
    [SerializeField] private Vector2 endPosition = new Vector2(200f, 0f);
    [SerializeField, Min(0f)] private float movementSpeed = 2f;
    [SerializeField] private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Options")]
    [SerializeField] private bool useLocalPosition = true;
    [SerializeField] private bool startMovingOnAwake = true;
    [SerializeField] private bool pingPongMovement = true;

    private RectTransform rectTransform;
    private float progress;
    private bool movingForward = true;
    private bool isMoving;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        ApplyPosition(startPosition);
    }

    private void Start()
    {
        if (startMovingOnAwake)
        {
            StartMovement();
        }
    }

    private void Update()
    {
        if (!isMoving)
        {
            return;
        }

        progress = Mathf.Clamp01(progress + Time.deltaTime * movementSpeed);
        float interpolation = movementCurve != null ? movementCurve.Evaluate(progress) : progress;
        Vector2 from = movingForward ? startPosition : endPosition;
        Vector2 to = movingForward ? endPosition : startPosition;
        ApplyPosition(Vector2.Lerp(from, to, interpolation));

        if (progress >= 1f)
        {
            ApplyPosition(to);
            progress = 0f;
            if (pingPongMovement)
            {
                movingForward = !movingForward;
            }
            else
            {
                isMoving = false;
            }
        }
    }

    public void StartMovement()
    {
        isMoving = true;
        ResetToStart();
    }

    public void StopMovement() => isMoving = false;
    public void ToggleMovement() => isMoving = !isMoving;

    public void ResetToStart()
    {
        progress = 0f;
        movingForward = true;
        ApplyPosition(startPosition);
    }

    public void SetPositions(Vector2 newStartPosition, Vector2 newEndPosition)
    {
        startPosition = newStartPosition;
        endPosition = newEndPosition;
    }

    public void SetSpeed(float newSpeed) => movementSpeed = Mathf.Max(0.1f, newSpeed);

    private void ApplyPosition(Vector2 position)
    {
        if (rectTransform == null)
        {
            return;
        }

        if (useLocalPosition)
        {
            rectTransform.anchoredPosition = position;
        }
        else
        {
            // Keep the existing depth when animating world-space UI.
            rectTransform.position = new Vector3(position.x, position.y, rectTransform.position.z);
        }
    }

    private void OnDrawGizmosSelected()
    {
        RectTransform rect = GetComponent<RectTransform>();
        if (rect == null)
        {
            return;
        }

        Vector3 start = GetWorldPosition(rect, startPosition);
        Vector3 end = GetWorldPosition(rect, endPosition);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(start, 10f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(end, 10f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(start, end);
    }

    private Vector3 GetWorldPosition(RectTransform rect, Vector2 position)
    {
        if (!useLocalPosition)
        {
            return new Vector3(position.x, position.y, rect.position.z);
        }

        // anchoredPosition is relative to the anchor reference, not this transform's origin.
        Vector3 localPosition = rect.localPosition;
        localPosition += (Vector3)(position - rect.anchoredPosition);
        return rect.parent != null ? rect.parent.TransformPoint(localPosition) : localPosition;
    }
}
