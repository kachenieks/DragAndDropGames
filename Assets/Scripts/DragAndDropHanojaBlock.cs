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

    // Vizuāls feedback (opcija)
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Start()
    {
        cam = Camera.main;
        TM = TowerManager.Instance;

        // Pārbauda vai ir collider
        if (GetComponent<Collider2D>() == null)
        {
            Debug.LogWarning($"[HANOJA] Blokam '{name}' nav Collider2D! Pievieno BoxCollider2D.");
        }

        // Sagatavojam vizuālo feedback
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    public void OnBeginDrag(PointerEventData e)
    {
        if (!TM.CanPickUp(this))
        {
            Debug.Log("[HANOJA] Nevari pacelt apakšējo bloku!");
            
            // KRITISKS: Aptur drag operāciju
            e.pointerDrag = null;
            
            // Vizuāls feedback (patrīc vai mainās krāsa)
            StartCoroutine(ShakeBlock());
            return;
        }

        isDragging = true;
        startPos = transform.position;
        offset = transform.position - ScreenToWorld(e.position);
        TM.RemoveBlock(this);

        // Vizuāls feedback - bloks kļūst caurspīdīgāks
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.7f);
        }

        Debug.Log($"[HANOJA] Pacelts bloks ar SizeIndex={SizeIndex}");
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

        // Atjauno krāsu
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        if (TM.TryPlaceBlock(this, transform.position))
        {
            Debug.Log("[HANOJA] Uzlikts uz torņa!");
        }
        else
        {
            Debug.Log("[HANOJA] Nederīgs gājiens - atgriežam atpakaļ!");
            TM.ReturnToPole(this);
        }
    }

    private Vector3 ScreenToWorld(Vector2 p)
    {
        var v = cam.ScreenToWorldPoint(p);
        v.z = 0;
        return v;
    }

    // Vizuāls feedback - bloka patrīcēšana
    private System.Collections.IEnumerator ShakeBlock()
    {
        Vector3 originalPos = transform.position;
        float duration = 0.3f;
        float elapsed = 0;

        while (elapsed < duration)
        {
            float x = originalPos.x + Random.Range(-0.1f, 0.1f);
            transform.position = new Vector3(x, originalPos.y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
    }
}