using UnityEngine;
using UnityEngine.EventSystems;

public class DropPlaceScript : MonoBehaviour, IDropHandler
{
    private ObjectScript objectScript;

    void Start()
    {
        objectScript = FindObjectOfType<ObjectScript>();
        if (objectScript == null)
            Debug.LogError("ObjectScript nav atrasts!");
    }

public void OnDrop(PointerEventData eventData)
{
    if (eventData.pointerDrag == null) return;

    var dragScript = eventData.pointerDrag.GetComponent<DragAndDropScript>();
    if (dragScript == null) return;

    dragScript.MarkAsDroppedOnDropPlace();

    if (eventData.pointerDrag.CompareTag(tag))
    {
        Debug.Log("Correct place");
        objectScript.rightPlace = true;

        GameManager gm = FindObjectOfType<GameManager>();
        gm?.OnVehicleCorrectlyPlaced();

        // Novieto precīzi uz DropPlace
        var vehicleRT = eventData.pointerDrag.GetComponent<RectTransform>();
        var placeRT = GetComponent<RectTransform>();
        vehicleRT.anchoredPosition = placeRT.anchoredPosition;
        vehicleRT.localRotation = placeRT.localRotation;
        vehicleRT.localScale = placeRT.localScale;

        PlaySound(tag);
    }
    else
    {
        objectScript.rightPlace = false;
        PlayIncorrectSound();
    }
}

    void PlayIncorrectSound()
    {
        if (objectScript?.effects != null && objectScript.audioCli.Length > 1)
            objectScript.effects.PlayOneShot(objectScript.audioCli[1]);
    }

    void PlaySound(string tag)
    {
        if (objectScript == null || objectScript.effects == null || objectScript.audioCli == null)
            return;

        AudioClip clip = null;
        switch (tag)
        {
            case "Garbage": clip = GetClip(2); break;
            case "Medicine": clip = GetClip(3); break;
            case "Fire": clip = GetClip(4); break;
            case "Bus": clip = GetClip(5); break;
            case "B2": clip = GetClip(6); break;
            case "Cement": clip = GetClip(7); break;
            case "E46": clip = GetClip(8); break;
            case "E61": clip = GetClip(9); break;
            case "Eskavators": clip = GetClip(10); break;
            case "Police": clip = GetClip(11); break;
            case "Traktors": clip = GetClip(12); break;
            case "Traktors2": clip = GetClip(13); break;
        }

        if (clip != null)
            objectScript.effects.PlayOneShot(clip);
    }

    AudioClip GetClip(int index)
    {
        if (index >= 0 && index < objectScript.audioCli.Length)
            return objectScript.audioCli[index];
        return null;
    }
}