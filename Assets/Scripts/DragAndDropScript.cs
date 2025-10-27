using UnityEngine;
using UnityEngine.EventSystems;

// CHANGES FOR ANDROID

public class DragAndDropScript : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private ObjectScript objectScript;
    private ScreenBoundriesScript screenBoundries;
    private Vector3 dragOffsetWorld;
    private Camera uiCamera;
    private Canvas canvas;

    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;
    private Vector3 originalLocalScale;

    private bool wasDroppedOnDropPlace = false;


    void Awake()
    {
        if (objectScript == null)
        {
            objectScript = Object.FindFirstObjectByType<ObjectScript>();
        }

        if (screenBoundries == null)
        {
            screenBoundries = Object.FindFirstObjectByType<ScreenBoundriesScript>();
        }

        canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
            uiCamera = canvas.worldCamera;
        else
            Debug.LogWarning("DragAndDropScript: No parent canvas found.");
    }

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
            Debug.Log("Vilku: " + name);
            if (objectScript != null && objectScript.effects != null && objectScript.audioCli.Length > 0)
                objectScript.effects.PlayOneShot(objectScript.audioCli[0]);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
            ObjectScript.drag = true;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0.6f;
            transform.SetAsLastSibling();

            ObjectScript.lastDragged = gameObject;
            wasDroppedOnDropPlace = false;

        Vector3 pointerWorld;
        if (ScreenPointToWorld(eventData.position, out pointerWorld))
        {
            dragOffsetWorld = transform.position - pointerWorld;
        }
        else
        {
            dragOffsetWorld = Vector3.zero;
        }
        
        ObjectScript.lastDragged = eventData.pointerDrag;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 pointerWorld;
        if (!ScreenPointToWorld(eventData.position, out pointerWorld))
            return;

        Vector3 desired = pointerWorld + dragOffsetWorld;
        desired.z = rectTransform.position.z; // Saglabā oriģinālo Z pozīciju
        screenBoundries.RecalculateBounds();

        Vector2 clamped = screenBoundries.GetClampedPosition(desired);
        transform.position = new Vector3(clamped.x, clamped.y, desired.z);
        
    }

    public void OnEndDrag(PointerEventData eventData)
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

    public void MarkAsDroppedOnDropPlace()
    {
        wasDroppedOnDropPlace = true;
    }

    private bool ScreenPointToWorld(Vector3 screenPoint, out Vector3 worldPoint)
    {
        worldPoint = Vector3.zero;
        if (uiCamera == null)
            Debug.LogWarning("DragAndDropScript: No UI camera found for ScreenPointToWorld conversion.");
            return false;

            float z = Mathf.Abs(uiCamera.transform.position.z - transform.position.z);
            Vector3 sp = new Vector3(screenPoint.x, screenPoint.y, z);
            worldPoint = uiCamera.ScreenToWorldPoint(sp);
            return true;
    }
}