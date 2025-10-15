using UnityEngine;
using UnityEngine.EventSystems;

public class DropPlaceScript : MonoBehaviour, IDropHandler
{
    private float placeZRot, vehicleZRot, rotDiff;
    private Vector3 placeSiz, vehicleSiz;
    private float xSizeDiff, ySizeDiff;
    private ObjectScript objectScript; // 👈 tagad privāts, netiek iestatīts Inspectorā

    void Start()
    {
        // Automātiski atrod ObjectScript ainā
        objectScript = FindObjectOfType<ObjectScript>();
        if (objectScript == null)
            Debug.LogError("ObjectScript nav atrasts! Pievienojiet ObjectManager ar ObjectScript.");
    }

    public void OnDrop(PointerEventData eventData) 
{
    if (eventData.pointerDrag != null)
    {
        // 👇 Paziņo, ka tika nometts uz DropPlace
        var dragScript = eventData.pointerDrag.GetComponent<DragAndDropScript>();
        if (dragScript != null)
            dragScript.MarkAsDroppedOnDropPlace();

        if (eventData.pointerDrag.tag.Equals(tag)) 
        {
            // ... (rotācijas un izmēra pārbaude)

            if ((rotDiff <= 5 || (rotDiff >= 355 && rotDiff <= 360)) &&
                (xSizeDiff <= 0.05f && ySizeDiff <= 0.05f))
            {
                Debug.Log("Correct place");
                objectScript.rightPlace = true;

                GameManager gm = FindObjectOfType<GameManager>();
                gm?.OnVehicleCorrectlyPlaced();

                // Novieto precīzi
                var rt = eventData.pointerDrag.GetComponent<RectTransform>();
                rt.anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
                rt.localRotation = GetComponent<RectTransform>().localRotation;
                rt.localScale = GetComponent<RectTransform>().localScale;

                PlaySound(eventData.pointerDrag.tag);
            }
            else
            {
                // Nepareizi novietots — rightPlace = false (atgriešanu veic DragAndDropScript)
                objectScript.rightPlace = false;
                objectScript.effects.PlayOneShot(objectScript.audioCli[1]);
            }
        }
        else
        {
            objectScript.rightPlace = false;
            objectScript.effects.PlayOneShot(objectScript.audioCli[1]);
        }
    }
}

void PlaySound(string tag)
{
    switch (tag)
    {
        case "Garbage": objectScript.effects.PlayOneShot(objectScript.audioCli[2]); break;
        case "Medicine": objectScript.effects.PlayOneShot(objectScript.audioCli[3]); break;
        case "Fire": objectScript.effects.PlayOneShot(objectScript.audioCli[4]); break;
        case "Bus": objectScript.effects.PlayOneShot(objectScript.audioCli[5]); break;
        case "B2": objectScript.effects.PlayOneShot(objectScript.audioCli[6]); break;
        case "Cement": objectScript.effects.PlayOneShot(objectScript.audioCli[7]); break;
        case "E46": objectScript.effects.PlayOneShot(objectScript.audioCli[8]); break;
        case "E61": objectScript.effects.PlayOneShot(objectScript.audioCli[9]); break;
        case "Eskavators": objectScript.effects.PlayOneShot(objectScript.audioCli[10]); break;
        case "Police": objectScript.effects.PlayOneShot(objectScript.audioCli[11]); break;
        case "Traktors": objectScript.effects.PlayOneShot(objectScript.audioCli[12]); break;
        case "Traktors2": objectScript.effects.PlayOneShot(objectScript.audioCli[13]); break;
    }
}
}