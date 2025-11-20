using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropHanojaBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int SizeIndex;
    public int CurrentPole;

    private Vector3 offset;
    private Vector3 startPos;
    private Camera cam;
    private TowerManager TM;
    private bool isDragging = false;

    private SpriteRenderer sr;
    private Color originalColor;

    private void Start()
    {
        cam = Camera.main;
        TM = TowerManager.Instance;

        sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;
    }

    public void OnBeginDrag(PointerEventData e)
    {
        if (!TM.CanPickUp(this))
        {
            e.pointerDrag = null;
            return;
        }

        // ⬅️ IMPORTANT
        TM.IsDragging = true;

        isDragging = true;
        startPos = transform.position;
        offset = transform.position - ScreenToWorld(e.position);

        TM.RemoveBlock(this);

        if (sr != null)
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.7f);
    }

    public void OnDrag(PointerEventData e)
    {
        if (!isDragging) return;
        transform.position = ScreenToWorld(e.position) + offset;
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (!isDragging) return;

        isDragging = false;

        // ⬅️ IMPORTANT
        TM.IsDragging = false;

        if (sr != null)
            sr.color = originalColor;

        if (!TM.TryPlaceBlock(this, transform.position))
        {
            TM.ReturnToPole(this);
        }
    }

    private Vector3 ScreenToWorld(Vector2 p)
    {
        var v = cam.ScreenToWorldPoint(p);
        v.z = 0;
        return v;
    }
}
