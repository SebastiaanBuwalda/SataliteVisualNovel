using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class DecisionChoice : MonoBehaviour, IPointerClickHandler
{
    [Header("Scale Settings")]
    [SerializeField] private float hoverScale = 1.2f;
    [SerializeField] private float scaleSpeed = 5f;
    [SerializeField] private float fullScreenScale = 10f;
    [SerializeField] private float fullScreenGrowSpeed = 3f;

    [Header("Choice Objects")]
    [SerializeField] private List<GameObject> otherChoices;

    [Header("Object Activation After Delay")]
    [SerializeField] private float delayBeforeSwitch = 2f;
    [SerializeField] private List<GameObject> objectsToEnable;
    [SerializeField] private List<GameObject> objectsToDisable;

    private TMP_Text tmpText;
    private Vector3 originalScale;
    private bool isHovered = false;
    private bool isClicked = false;
    private Collider2D pointerArea;

    private void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
        originalScale = transform.localScale;
        pointerArea = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (!isClicked)
        {
            Vector3 targetScale = isHovered ? originalScale * hoverScale : originalScale;
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);

            // Manual hover detection
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            bool over = pointerArea.OverlapPoint(mousePos);
            isHovered = over;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isClicked) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (pointerArea != null && pointerArea.OverlapPoint(mousePos))
        {
            isClicked = true;
            StartCoroutine(HandleClickSequence());
        }
    }

    private void OnMouseDown()
    {
        if (!isClicked && isHovered)
        {
            isClicked = true;
            StartCoroutine(HandleClickSequence());
        }
    }

    private IEnumerator HandleClickSequence()
    {



        // Hide other choices
        foreach (var choice in otherChoices)
        {
            if (choice != null) choice.SetActive(false);
        }

        StartCoroutine(Grow());
        yield return new WaitForSeconds(delayBeforeSwitch);

        // Disable objects
        foreach (var obj in objectsToDisable)
        {
            if (obj != null) obj.SetActive(false);
        }

        // Enable objects
        foreach (var obj in objectsToEnable)
        {
            if (obj != null) obj.SetActive(true);
        }

        // Grow to full screen
    
        // Wait

    }
    
    private IEnumerator Grow()
    {
        Vector3 targetScale = Vector3.one * fullScreenScale;
        while (Vector3.Distance(transform.localScale, targetScale) > 0.01f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * fullScreenGrowSpeed);
            yield return null;
        }
        transform.localScale = targetScale;
    }
}
