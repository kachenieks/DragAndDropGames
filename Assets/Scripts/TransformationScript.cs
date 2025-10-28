using UnityEngine;

public class TransformationScript : MonoBehaviour
{
    public float rotationSpeed = 90f;
    public float scaleSpeed = 0.5f;

    private bool rotateCW, rotateCCW, scaleUpY, scaleDownY, scaleUpX, scaleDownX;
    public static bool isTransforming = false;
    // [Header("Izmēra ierobežojumi")]
    // public float minScale = 0.3f;
    // public float maxScale = 1.05f;
    // public float scaleSpeed = 0.005f;

    // [Header("Rotācijas ātrums")]
    // public float rotationSpeed = 15f;

    void Update()
    {
        if (ObjectScript.lastDragged == null)
            return;

        RectTransform rt = ObjectScript.lastDragged.GetComponent<RectTransform>();

        if (rotateCW)
            rt.Rotate(0, 0, -rotationSpeed * Time.deltaTime);

        if (rotateCWW)
            rt.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        if (scaleUpY && rt.localScale.y < 0.8f)
            rt.localScale += new Vector3(0, scaleSpeed * Time.deltaTime, 0);

        if (scaleDownY && rt.localScale.y > 0.8f)
            rt.localScale += new Vector3(0, scaleSpeed * Time.deltaTime, 0);

        if (scaleUpX && rt.localScale.y < 0.8f)
            rt.localScale += new Vector3(scaleSpeed * Time.deltaTime, 0, 0);

        if (scaleDownX && rt.localScale.y < 0.8f)
            rt.localScale += new Vector3(scaleSpeed * Time.deltaTime, 0, 0);

        isTransforming = rotateCW || rotateCCW || scaleUpY || scaleUpX || scaleDownX || scaleDownY;
    }

    public void StartRotateCW(BaseEventData data){rotateCW = true;}
    public void StartRotateCW(BaseEventData data){rotateCW = false;}

    public void StartRotateCWW(BaseEventData data){rotateCWW = true;}
    public void StartRotateCWW(BaseEventData data){rotateCWW = false;}

    public void StartScaleUpY(BaseEventData data){scaleUpY = true;}
    public void StartScaleUpY(BaseEventData data){scaleUpY = false;}
    
    public void StartScaleUpX(BaseEventData data){scaleUpX = false;}
    public void StartScaleUpX(BaseEventData data){scaleUpX = true;}

    public void StartScaleDownX(BaseEventData data){scaleDownX = true;}
    public void StartScaleDownX(BaseEventData data){scaleDownX = false;}

    public void StartScaleDownY(BaseEventData data){scaleDownY = true;}
    public void StartScaleDownY(BaseEventData data){scaleDownY = false;}

}