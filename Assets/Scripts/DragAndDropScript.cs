using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropScript : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private ObjectScript objectScript;
    private ScreenBoundriesScript screenBoundries;

    void Start()
    {
        // Pievieno CanvasGroup, ja nav
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        rectTransform = GetComponent<RectTransform>();

        // Automātiski atrod nepieciešamos skriptus ainā
        objectScript = FindObjectOfType<ObjectScript>();
        screenBoundries = FindObjectOfType<ScreenBoundriesScript>();

        if (objectScript == null)
            Debug.LogError("ObjectScript nav atrasts ainā! Pievienojiet ObjectManager ar ObjectScript.");
        if (screenBoundries == null)
            Debug.LogError("ScreenBoundriesScript nav atrasts ainā! Pievienojiet ScreenBoundries GameObject.");
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

        // 👇 Iestatiet lastDragged uz PAŠREIZĒJO OBJEKTU
        ObjectScript.lastDragged = gameObject; // 👈 NE eventData.pointerDrag!

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

            if (objectScript != null)
            {
                if (objectScript.rightPlace)
                {
                    canvasGroup.blocksRaycasts = false;
                    ObjectScript.lastDragged = null;
                }
                objectScript.rightPlace = false;
            }
        }
    }
}