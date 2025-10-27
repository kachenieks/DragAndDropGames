using UnityEngine;

// CHANGES FOR ANDROID
public class ScreenBoundriesScript : MonoBehaviour
{

    [HideInInspector]
    public Vector3 screenPoint, offset;
    [HideInInspector]
    public float minX, maxX, minY, maxY;
    public Rect WorldBounds = new Rect(-960, -540, 1920, 1080);
    [Range(0f, 0.5f)]
    public float padding = 0.02f;
    public Camera targetCam;
    public float minCamX { get; private set; }
    public float maxCamX { get; private set; }
    public float minCamY { get; private set; }
    public float maxCamY { get; private set; }
    float lastOrthoSize;
    float lastAspect;
    Vector3 lastCamPosition;

    void Awake()
    {
        if (targetCam == null)
        {
            targetCam = Camera.main;
        }
        RecalculateBounds();
    }


    private void Update()
    {
        if (targetCam == null)
        {
            return;
        }

        bool changes = true;

        if (targetCam.orthographic)
        {
            if (!Mathf.Approximately(targetCam.orthographicSize, lastOrthoSize))
                changes = true;
        }

        if (!Mathf.Approximately(targetCam.aspect, lastAspect))
            changes = true;

        if (targetCam.transform.position != lastCamPosition)
            changes = true;

        if (changes)
        {
            RecalculateBounds();
        }
    }

    public void RecalculateBounds()
    {
        if (targetCam == null)
        {
            return;
        }

        float wbMinX = WorldBounds.xMin;
        float wbMaxX = WorldBounds.xMax;
        float wbMinY = WorldBounds.yMin;
        float wbMaxY = WorldBounds.yMax;

        if (targetCam.orthographic)
        {
            float halfH = targetCam.orthographicSize;
            float halfW = halfH * targetCam.aspect;

            if (halfW * 2f >= (wbMaxX - wbMinX))
            {
                minCamX = maxCamX = (wbMinX + wbMaxX) * 0.5f;
            }
            else
            {
                minCamX = wbMinX + halfW;
                maxCamX = wbMaxX - halfW;
            }

            if (halfH * 2f >= (wbMaxY - wbMinY))
            {
                minCamY = maxCamY = (wbMinY + wbMaxY) * 0.5f;
            }
            else
            {
                minCamY = wbMinY + halfH;
                maxCamY = wbMaxY - halfH;
            }
        }
        lastOrthoSize = targetCam.orthographicSize;
        lastAspect = targetCam.aspect;
        lastCamPosition = targetCam.transform.position;
    }

    public Vector2 GetClampedPosition(Vector3 curPosition)
    {
        float shrinkX = WorldBounds.width * padding;
        float shrinkY = WorldBounds.height * padding;
        float wbMinX = WorldBounds.xMin + shrinkX;
        float wbMaxX = WorldBounds.xMax - shrinkX;
        float wbMinY = WorldBounds.yMin + shrinkY;
        float wbMaxY = WorldBounds.yMax - shrinkY;

        float cx = Mathf.Clamp(curPosition.x, wbMinX, wbMaxX);
        float cy = Mathf.Clamp(curPosition.y, wbMinY, wbMaxY);

        return new Vector2(cx, cy);
    }

    //For camera
    public Vector3 GetClampedCameraPosition(Vector3 curPosition)
    {
        float cx = Mathf.Clamp(curPosition.x, minCamX, maxCamX);
        float cy = Mathf.Clamp(curPosition.y, minCamY, maxCamY);

        return new Vector3(cx, cy, curPosition.z);
    }
}