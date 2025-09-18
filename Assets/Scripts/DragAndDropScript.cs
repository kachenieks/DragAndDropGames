using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropScript : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, 
    IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGro;
    private RectTransform rectTra;
    public ObjectScript objectScr;
    public ScreenBoundriesScript screenBou;

    void Start()
    {
        canvasGro = GetComponent<CanvasGroup>();
        rectTra = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2)) 
        {
            Debug.Log("OnPointerDown");
            objectScr.effects.PlayOneShot(objectScr.audioCli[0]);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
        {
            canvasGro.blocksRaycasts = false;
            canvasGro.alpha = 0.6f;
            //rectTra.SetAsLastSibling();
            int positionIndex = transform.parent.childCount - 1;
            Vector3 cursorWorldPos = Camera.main.ScreenToWorldPoint(
                new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenBou.screenPoint.z));
            rectTra.position = cursorWorldPos;

            screenBou.screenPoint = Camera.main.WorldToScreenPoint(rectTra.localPosition);

            screenBou.offset = rectTra.localPosition - Camera.main.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenBou.screenPoint.z));

            objectScr.lastDragged = eventData.pointerDrag;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenBou.screenPoint.z);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + screenBou.offset;
            rectTra.position = screenBou.GetClampedPosition(curPosition);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButtonUp(0))
        {
            canvasGro.blocksRaycasts = true;
            canvasGro.alpha = 1.0f;

            if(objectScr.rightPlace)
            {
                canvasGro.blocksRaycasts = false;
                objectScr.lastDragged = null;

            }

            objectScr.rightPlace = false;
        }
    }
}
