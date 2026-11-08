using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class DecisionChoice : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scale Settings")]
    [SerializeField, Min(0f)] private float hoverScale = 1.2f;
    [SerializeField, Min(0.01f)] private float scaleSpeed = 5f;
    [SerializeField, Min(0f)] private float fullScreenScale = 10f;
    [SerializeField, Min(0.01f)] private float fullScreenGrowSpeed = 3f;

    [Header("Choice Objects")]
    [SerializeField] private List<GameObject> otherChoices = new List<GameObject>();

    [Header("Transition")]
    [SerializeField, Min(0f)] private float delayBeforeSwitch = 2f;
    [SerializeField] private List<GameObject> objectsToEnable = new List<GameObject>();
    [SerializeField] private List<GameObject> objectsToDisable = new List<GameObject>();

    private Vector3 originalScale;
    private bool isHovered;
    private bool isSelected;
    private int activationFrame;

    private void Awake() => originalScale = transform.localScale;

    private void OnEnable() => activationFrame = Time.frameCount;

    private void OnDisable()
    {
        StopAllCoroutines();
        isHovered = false;
        isSelected = false;
        transform.localScale = originalScale;
    }

    private void Update()
    {
        if (!isSelected)
        {
            Vector3 targetScale = isHovered ? originalScale * hoverScale : originalScale;
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
        }
    }

    private void OnMouseEnter() => isHovered = true;
    private void OnMouseExit() => isHovered = false;
    private void OnMouseDown() => SelectChoice();
    public void OnPointerEnter(PointerEventData eventData) => isHovered = true;
    public void OnPointerExit(PointerEventData eventData) => isHovered = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            SelectChoice();
        }
    }

    private void SelectChoice()
    {
        if (isSelected || !isActiveAndEnabled || Time.frameCount <= activationFrame)
        {
            return;
        }

        isSelected = true;
        StartCoroutine(HandleSelection());
    }

    private IEnumerator HandleSelection()
    {
        SceneObjectState.SetActive(otherChoices, false);
        StartCoroutine(Grow());
        yield return new WaitForSeconds(delayBeforeSwitch);
        SceneObjectState.SetActive(objectsToEnable, true);
        SceneObjectState.SetActive(objectsToDisable, false);
    }

    private IEnumerator Grow()
    {
        Vector3 targetScale = Vector3.one * fullScreenScale;
        while ((transform.localScale - targetScale).sqrMagnitude > 0.0001f)
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale, targetScale, Time.deltaTime * Mathf.Max(0.01f, fullScreenGrowSpeed));
            yield return null;
        }

        transform.localScale = targetScale;
    }
}
