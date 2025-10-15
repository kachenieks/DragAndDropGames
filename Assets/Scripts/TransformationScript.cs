using UnityEngine;

public class TransformationScript : MonoBehaviour
{
    [Header("Izmēra ierobežojumi")]
    public float minScale = 0.3f;
    public float maxScale = 1.05f;
    public float scaleSpeed = 0.005f;

    [Header("Rotācijas ātrums")]
    public float rotationSpeed = 15f;

    void Update()
    {
        if (ObjectScript.lastDragged == null) return;

        RectTransform rect = ObjectScript.lastDragged.GetComponent<RectTransform>();
        Vector3 currentScale = rect.localScale;

        // ===== ROTĀCIJA =====
        if (Input.GetKey(KeyCode.Z))
        {
            rect.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.X))
        {
            rect.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
        }

        // ===== IZMĒRA MAIŅA =====
        float deltaX = 0f;
        float deltaY = 0f;

        // X ass (pa kreisi / pa labi)
        if (Input.GetKey(KeyCode.RightArrow))
            deltaX = scaleSpeed;
        if (Input.GetKey(KeyCode.LeftArrow))
            deltaX = -scaleSpeed;

        // Y ass (augšup / lejup)
        if (Input.GetKey(KeyCode.UpArrow))
            deltaY = scaleSpeed;
        if (Input.GetKey(KeyCode.DownArrow))
            deltaY = -scaleSpeed;

        // Piemēro izmaiņas, bet ierobežo robežās
        float newScaleX = Mathf.Clamp(currentScale.x + deltaX, minScale, maxScale);
        float newScaleY = Mathf.Clamp(currentScale.y + deltaY, minScale, maxScale);

        rect.localScale = new Vector3(newScaleX, newScaleY, 1f);
    }
}