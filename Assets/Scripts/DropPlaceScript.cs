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
    if ((eventData.pointerDrag != null) && Input.GetMouseButtonUp(0))
    {
        if (eventData.pointerDrag.tag.Equals(tag)) 
        {
            // 👇 Rotācijas un mērogošanas pārbaude
            float placeZRot = eventData.pointerDrag.GetComponent<RectTransform>().transform.eulerAngles.z;
            float vehicleZRot = GetComponent<RectTransform>().transform.eulerAngles.z;
            float rotDiff = Mathf.Abs(placeZRot - vehicleZRot);
            Debug.Log("Rotation difference: " + rotDiff);

            Vector3 placeSiz = eventData.pointerDrag.GetComponent<RectTransform>().localScale;
            Vector3 vehicleSiz = GetComponent<RectTransform>().localScale;
            float xSizeDiff = Mathf.Abs(placeSiz.x - vehicleSiz.x);
            float ySizeDiff = Mathf.Abs(placeSiz.y - vehicleSiz.y);
            Debug.Log("X size difference: " + xSizeDiff);
            Debug.Log("Y size difference: " + ySizeDiff);

            if ((rotDiff <= 5 || (rotDiff >= 355 && rotDiff <= 360)) &&
                (xSizeDiff <= 0.05f && ySizeDiff <= 0.05f))
            {
                Debug.Log("Correct place");
                objectScript.rightPlace = true;

                // Paziņo GameManager
                GameManager gm = FindObjectOfType<GameManager>();
                gm?.OnVehicleCorrectlyPlaced();

                // Novieto objektu precīzi
                eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
                eventData.pointerDrag.GetComponent<RectTransform>().localRotation = GetComponent<RectTransform>().localRotation;
                eventData.pointerDrag.GetComponent<RectTransform>().localScale = GetComponent<RectTransform>().localScale;

                // Skaņas
                PlaySound(eventData.pointerDrag.tag);
            }
            else
            {
                // Nepareizi novietots
                objectScript.rightPlace = false;
                objectScript.effects.PlayOneShot(objectScript.audioCli[1]);
                // Atgriež sākumā (ja vēlaties)
            }
        }
        else
        {
            // Nepareizs objekts
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