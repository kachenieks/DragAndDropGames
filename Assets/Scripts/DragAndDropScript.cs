using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropScript : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private ObjectScript objectScript;
    private ScreenBoundriesScript screenBoundries;

    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;
    private Vector3 originalLocalScale;

    private bool wasDroppedOnDropPlace = false;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        rectTransform = GetComponent<RectTransform>();
        objectScript = FindObjectOfType<ObjectScript>();
        screenBoundries = FindObjectOfType<ScreenBoundriesScript>();

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
            wasDroppedOnDropPlace = false;

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

            // ✅ Ja tika nometts uz DropPlace
            if (wasDroppedOnDropPlace)
            {
                if (objectScript != null && objectScript.rightPlace)
                {
                    // Pareizi novietots — bloķē
                    canvasGroup.blocksRaycasts = false;
                    ObjectScript.lastDragged = null;
                    objectScript.rightPlace = false; // reset
                }
                else
                {
                    // ❌ Nepareizi novietots uz DropPlace → atgriež sākumā
                    rectTransform.anchoredPosition = originalLocalPosition;
                    rectTransform.localRotation = originalLocalRotation;
                    rectTransform.localScale = originalLocalScale;
                }
            }
            // ✅ Ja nometts uz tukšas vietas → PALIEK TUR, kur nometi (nekas nedara)
            // (nekādas darbības nav nepieciešamas)

            wasDroppedOnDropPlace = false;
        }
    }

    public void MarkAsDroppedOnDropPlace()
    {
        wasDroppedOnDropPlace = true;
    }
}