using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class DragAndDropHanojaScript : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 offset;
    private Camera mainCamera;
    private Rigidbody2D rb;
    private bool isDragging = false;
    private float snapDistance = 2.5f; // cik tuvu jābūt, lai pieliptu stabam

    private void Awake()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();

        // drošībai konfigurējam Rigidbody2D
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(eventData.position);
        offset = transform.position - new Vector3(mouseWorldPos.x, mouseWorldPos.y, transform.position.z);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;

        // atļaujam kustību X-Y plaknē
        rb.constraints = RigidbodyConstraints2D.None;
        rb.gravityScale = 0;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(eventData.position);
        transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, transform.position.z) + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        // iesaldē vertikāli un rotāciju
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

        // Snap pie tuvākā staba
        SnapToNearestPole();
    }

    private void SnapToNearestPole()
    {
        GameObject[] poles = GameObject.FindGameObjectsWithTag("Pole");

        if (poles.Length == 0)
        {
            Debug.LogWarning("❌ Nav atrasts neviens stabs ar tagu 'Pole'.");
            return;
        }

        float closestDistance = float.MaxValue;
        Transform closestPole = null;

        foreach (GameObject pole in poles)
        {
            float distance = Vector2.Distance(transform.position, pole.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPole = pole.transform;
            }
        }

        // ja tuvāk par noteikto distanci — pievelkam bloku precīzi
        if (closestPole != null && closestDistance <= snapDistance)
        {
            Vector3 snapPos = new Vector3(
                closestPole.position.x,
                transform.position.y,
                transform.position.z
            );

            transform.position = snapPos;
        }
    }
}
