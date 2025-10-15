using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropScript : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private ObjectScript objectScript;
    private ScreenBoundriesScript screenBoundries;

    // 👇 Sākuma stāvoklis
    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;
    private Vector3 originalLocalScale;

    // 👇 Jauns: vai tika nometts uz DropPlace
    private bool wasDroppedOnDropPlace = false;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        rectTransform = GetComponent<RectTransform>();

        objectScript = FindObjectOfType<ObjectScript>();
        screenBoundries = FindObjectOfType<ScreenBoundriesScript>();

        // Saglabā sākotnējo stāvokli
        originalLocalPosition = rectTransform.anchoredPosition;
        originalLocalRotation = rectTransform.localRotation;
        originalLocalScale = rectTransform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            Debug.Log("Vilku: " + name);
            if (objectScript != null && objectScript.effects != null && objectScript.audioCli.Length > 0)
                objectScript.effects.PlayOneShot(objectScript.audioCli[0]);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            ObjectScript.drag = true;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0.6f;
            transform.SetAsLastSibling();

            ObjectScript.lastDragged = gameObject;
            wasDroppedOnDropPlace = false; // ⚠️ Reset

            if (screenBoundries != null)
            {
                Vector3 cursorWorldPos = Camera.main.ScreenToWorldPoint(
                    new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenBoundries.screenPoint.z));
                rectTransform.position = cursorWorldPos;

                screenBoundries.screenPoint = Camera.main.WorldToScreenPoint(rectTransform.position);
                screenBoundries.offset = rectTransform.position - Camera.main.ScreenToWorldPoint(
                    new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenBoundries.screenPoint.z));
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0) && screenBoundries != null)
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenBoundries.screenPoint.z);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + screenBoundries.offset;
            rectTransform.position = screenBoundries.GetClampedPosition(curPosition);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButtonUp(0))
        {
            ObjectScript.drag = false;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1.0f;

            // ✅ Atgriež sākumā TIKAI ja:
            // - tika nometts uz DropPlace (wasDroppedOnDropPlace = true)
            // - un tas bija NEPAREIZS (rightPlace = false)
            if (wasDroppedOnDropPlace && objectScript != null && !objectScript.rightPlace)
            {
                rectTransform.anchoredPosition = originalLocalPosition;
                rectTransform.localRotation = originalLocalRotation;
                rectTransform.localScale = originalLocalScale;
            }
            else if (objectScript != null && objectScript.rightPlace)
            {
                // Pareizi novietots — bloķē
                canvasGroup.blocksRaycasts = false;
                ObjectScript.lastDragged = null;
                objectScript.rightPlace = false; // reset
            }

            // Reset
            wasDroppedOnDropPlace = false;
        }
    }

    // 👇 Jaunu metodi, ko izsauc DropPlaceScript
    public void MarkAsDroppedOnDropPlace()
    {
        wasDroppedOnDropPlace = true;
    }
}