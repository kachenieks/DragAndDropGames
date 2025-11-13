using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropHanojaBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int SizeIndex;
    public int CurrentPole;

    private Vector3 offset;
    private Camera cam;
    private TowerManager TM;

    private void Start()
    {
        cam = Camera.main;
        TM = TowerManager.Instance;
    }

    public void OnBeginDrag(PointerEventData e)
    {
        if (!TM.CanPickUp(this))
        {
            Debug.Log("[HANOJA] Nevari pacelt apakšējo bloku!");
            return;
        }

        offset = transform.position - ScreenToWorld(e.position);
        TM.RemoveBlock(this);
    }

    public void OnDrag(PointerEventData e)
    {
        transform.position = ScreenToWorld(e.position) + offset;
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (TM.TryPlaceBlock(this, transform.position))
        {
            Debug.Log("[HANOJA] Uzlikts uz torņa!");
        }
        else
        {
            Debug.Log("[HANOJA] Nokrita uz zemes!");
            TM.DropToGround(this);
        }
    }

    private Vector3 ScreenToWorld(Vector2 p)
    {
        var v = cam.ScreenToWorldPoint(p);
        v.z = 0;
        return v;
    }
}
