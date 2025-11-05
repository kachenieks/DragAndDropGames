using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // ← ŠO PIEVIENO

public class CameraScript : MonoBehaviour
{
    private float maxZoom;
    public float minZoom = 150f;
    public float panSpeed = 6f;
    float startZoom;
    public float pinchZoomSpeed = 0.9f, mouseZoomSpeed = 150f;
    public float mouseFollowSpeed = 1f, touchPanSpeed = 1f;
    public ScreenBoundriesScript screenBoundries;
    float initialZoom;
    public Camera cam;
    Vector2 lastTouchPos;
    int panFingerId = -1;
    bool isTouchPanning = false;
    float lastTapTime = 0f;
    public float doubleTapMaxDelay = 0.4f;
    public float doubleTapMaxDistance = 100f;

    void Awake()
    {
        // ✅ ja atrodamies MainMenu scēnā — izslēdzam šo skriptu
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            enabled = false;
            return;
        }

        if (cam == null)
            cam = GetComponent<Camera>();

        if (screenBoundries == null)
            screenBoundries = FindFirstObjectByType<ScreenBoundriesScript>();
    }

    void Start()
    {
        startZoom = cam.orthographicSize;
        screenBoundries.RecalculateBounds();
        transform.position = screenBoundries.GetClampedCameraPosition(transform.position);
    }

    void Update()
    {
        if (TransformationScript.isTransforming) return;

#if UNITY_EDITOR || UNITY_STANDALONE
        DesktopFollowCursor();
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > Mathf.Epsilon)
        {
            cam.orthographicSize -= scroll * mouseZoomSpeed;
        }
#else
        HandleTouch();
#endif

        UpdateMaxZoom();
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        screenBoundries.RecalculateBounds();
        transform.position = screenBoundries.GetClampedCameraPosition(transform.position);
    }

    void DesktopFollowCursor()
    {
        Vector3 mouse = Input.mousePosition;
        if (mouse.x < 0 || mouse.x > Screen.width || mouse.y < 0 || mouse.y > Screen.height) return;

        Vector3 screenPoint = new Vector3(mouse.x, mouse.y, cam.nearClipPlane);
        Vector3 targetWorld = cam.ScreenToWorldPoint(screenPoint);
        Vector3 desired = new Vector3(targetWorld.x, targetWorld.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * mouseFollowSpeed);
    }

    void HandleTouch()
    {
        if (Input.touchCount == 0) return;

        if (Input.touchCount == 2)
        {
            HandlePinch();
            return;
        }

        Touch t = Input.GetTouch(0);
        if (IsTouchingOverUIButton(t.position)) return;

        if (t.phase == TouchPhase.Began)
        {
            float dt = Time.time - lastTapTime;
            if (dt <= doubleTapMaxDelay && Vector2.Distance(t.position, lastTouchPos) <= doubleTapMaxDistance)
            {
                StartCoroutine(ResetZoomSmooth());
                lastTapTime = 0f;
            }
            else
            {
                lastTapTime = Time.time;
            }
            lastTouchPos = t.position;
            panFingerId = t.fingerId;
            isTouchPanning = true;
        }
        else if (t.phase == TouchPhase.Moved && isTouchPanning && t.fingerId == panFingerId)
        {
            Vector2 delta = t.position - lastTouchPos;
            transform.Translate(-ScreenDeltaToWorldDelta(delta) * touchPanSpeed, Space.World);
            lastTouchPos = t.position;
        }
        else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
        {
            isTouchPanning = false;
            panFingerId = -1;
        }
    }

    bool IsTouchingOverUIButton(Vector2 touchPos)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = touchPos;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponent<UnityEngine.UI.Button>() != null)
            {
                return true;
            }
        }
        return false;
    }

    void HandlePinch()
    {
        Touch t0 = Input.GetTouch(0);
        Touch t1 = Input.GetTouch(1);

        float prevDist = (t0.position - t0.deltaPosition - (t1.position - t1.deltaPosition)).magnitude;
        float currDist = (t0.position - t1.position).magnitude;

        cam.orthographicSize -= (prevDist - currDist) * pinchZoomSpeed;
    }

    Vector3 ScreenDeltaToWorldDelta(Vector2 screenDelta)
    {
        float worldPerPixel = (2f * cam.orthographicSize) / Screen.height;
        return new Vector3(screenDelta.x * worldPerPixel, screenDelta.y * worldPerPixel, 0f);
    }

    IEnumerator ResetZoomSmooth()
    {
        float duration = 0.25f;
        float elapsed = 0f;
        float initialZoom = cam.orthographicSize;

        float targetZoom = maxZoom;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cam.orthographicSize = Mathf.Lerp(initialZoom, targetZoom, elapsed / duration);
            screenBoundries.RecalculateBounds();
            transform.position = screenBoundries.GetClampedCameraPosition(transform.position);
            yield return null;
        }

        cam.orthographicSize = targetZoom;
        screenBoundries.RecalculateBounds();
        transform.position = screenBoundries.GetClampedCameraPosition(transform.position);
    }

    void UpdateMaxZoom()
    {
        if (screenBoundries == null || cam == null)
            return;

        Rect wb = screenBoundries.WorldBounds;
        float maxZoomHeight = wb.height / 2f;
        float maxZoomWidth = (wb.width / cam.aspect) / 2f;
        maxZoom = Mathf.Min(maxZoomHeight, maxZoomWidth);
    }
}