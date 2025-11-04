using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropScript : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private ObjectScript objectScript;
    private ScreenBoundriesScript screenBoundries;
    private Camera uiCamera;
    private Canvas canvas;

    private Vector3 originalWorldPosition;
    private Quaternion originalLocalRotation;
    private Vector3 originalLocalScale;

    private bool wasDroppedOnDropPlace = false;

    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("DragAndDropScript: No Canvas found in parent hierarchy on " + name);
            return;
        }

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            uiCamera = null;
        }
        else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            uiCamera = canvas.worldCamera;
            if (uiCamera == null)
            {
                Debug.LogError("DragAndDropScript: Canvas is ScreenSpaceCamera but worldCamera is not assigned!");
            }
        }
        else
        {
            uiCamera = Camera.main;
        }

        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();

        objectScript = FindObjectOfType<ObjectScript>();
        screenBoundries = FindObjectOfType<ScreenBoundriesScript>();
    }

    void Start()
    {
        // Saglabā sākuma pozīciju pēc tam, kad objekts jau ir spawnots
        originalWorldPosition = transform.position;
        originalLocalRotation = rectTransform.localRotation;
        originalLocalScale = rectTransform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Vilku: " + name);
        if (objectScript != null && objectScript.effects != null && objectScript.audioCli.Length > 0)
        {
            objectScript.effects.PlayOneShot(objectScript.audioCli[0]);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        ObjectScript.drag = true;
        ObjectScript.lastDragged = gameObject;

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
        transform.SetAsLastSibling();

        wasDroppedOnDropPlace = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 pointerWorld;
        if (!RectTransformUtility.ScreenPointToWorldPointInRectangle(
                rectTransform, eventData.position, uiCamera, out pointerWorld))
            return;

        Vector3 desired = pointerWorld;
        desired.z = transform.position.z; // saglabājam z dziļumu

        if (screenBoundries != null)
        {
            Vector2 clamped = screenBoundries.GetClampedPosition(desired);
            transform.position = new Vector3(clamped.x, clamped.y, desired.z);
        }
        else
        {
            transform.position = desired;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ObjectScript.drag = false;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1.0f;

        if (wasDroppedOnDropPlace)
        {
            if (objectScript != null && objectScript.rightPlace)
            {
                // ✅ Pareizi novietots
                canvasGroup.blocksRaycasts = false;
                ObjectScript.lastDragged = null;
                objectScript.rightPlace = false;
            }
            else
            {
                // ❌ Nepareizs DropPlace
                ResetToStart();
            }
        }

        // 🚫 Ja nav dropots uz DropPlace, paliek tur, kur nometi
        wasDroppedOnDropPlace = false;
    }

    public void MarkAsDroppedOnDropPlace()
    {
        wasDroppedOnDropPlace = true;
    }

    private void ResetToStart()
    {
        transform.position = originalWorldPosition;
        rectTransform.localRotation = originalLocalRotation;
        rectTransform.localScale = originalLocalScale;
    }
}
