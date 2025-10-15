using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // 👇 IESTATI ŠOS DIVUS SKAITĻUS PĒC SAVAS KARTES!
    public float worldWidth = 1600f;   // Kopējais platums (piemēram, no -800 līdz +800)
    public float worldHeight = 900f;   // Kopējais augstums (piemēram, no -450 līdz +450)

    public float minZoom = 20f;        // Cik tuvu vari iezumot (mazāk = tuvāk)
    public float zoomSpeed = 80f;      // Zoom ātrums
    public float panSpeed = 10f;

    private Camera cam;
    private float startZoom;

    void Start()
    {
        cam = GetComponent<Camera>();
        float aspect = (float)Screen.width / Screen.height; // Parasti 1.777 (16:9)

        float zoomForHeight = worldHeight / 2f;
        float zoomForWidth = (worldWidth / 2f) / aspect;

        startZoom = Mathf.Max(zoomForHeight, zoomForWidth);

        cam.orthographicSize = startZoom; // ✅ Sāk ar "pilnekrāna" skatu
    }

    void Update()
    {
        // Pārvietošanās
        float x = Input.GetAxis("Mouse X") * panSpeed;
        float y = Input.GetAxis("Mouse Y") * panSpeed;
        transform.Translate(x, y, 0);

        // Zoom
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0)
        {
            cam.orthographicSize -= scroll * zoomSpeed * Time.deltaTime * 10f;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, startZoom);
        }

        // Ierobežo kustību
        float camHeight = cam.orthographicSize;
        float camWidth = cam.orthographicSize * cam.aspect;

        float leftBound = -worldWidth / 2f + camWidth;
        float rightBound = worldWidth / 2f - camWidth;
        float bottomBound = -worldHeight / 2f + camHeight;
        float topBound = worldHeight / 2f - camHeight;

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, leftBound, rightBound);
        pos.y = Mathf.Clamp(pos.y, bottomBound, topBound);
        transform.position = pos;
    }
}