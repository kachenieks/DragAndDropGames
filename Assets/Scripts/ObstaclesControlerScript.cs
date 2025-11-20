using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ObstaclesControlerScript : MonoBehaviour
{
    [HideInInspector]
    public float speed = 1f;
    public float waveAmplitude = 25f;
    public float waveFrequency = 1f;
    public float fadeDuration = 1.5f;
    private ObjectScript objectScript;
    private ScreenBoundriesScript screenBoundriesScript;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private bool isFadingOut = false;
    private bool isExploding = false;

    private Image image;
    private Color originalColor;

    // Debug režīma pārslēgšanai
    public bool showDebugLines = true;
    public Color minXColor = Color.red;
    public Color maxXColor = Color.green;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        rectTransform = GetComponent<RectTransform>();

        image = GetComponent<Image>();
        originalColor = image.color;

        objectScript = Object.FindFirstObjectByType<ObjectScript>();
        screenBoundriesScript = Object.FindFirstObjectByType<ScreenBoundriesScript>();
        StartCoroutine(FadeIn());
    }

    void Update()
    {
        float waveOffset = Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;
        rectTransform.anchoredPosition += new Vector2(speed * Time.deltaTime, waveOffset * Time.deltaTime);

        // ✅ Pareiza robežu pārbaude
        if ((speed > 0 && rectTransform.anchoredPosition.x > screenBoundriesScript.maxX + 300f) ||
            (speed < 0 && rectTransform.anchoredPosition.x < screenBoundriesScript.minX - 300f))
        {
            if (!isFadingOut)
            {
                isFadingOut = true;
                StartCoroutine(FadeOutAndDestroy());
            }
        }

        // ✅ Debug: parāda objektu pozīciju un ātrumu
        // Debug.Log($"{gameObject.name} | X: {rectTransform.anchoredPosition.x} | Speed: {speed}");

        // Bomba bez vilkšanas
        Vector2 inputPosition;
        if (!TryGetInputPosition(out inputPosition))
            return;

        if (CompareTag("Bomb") && !isExploding &&
            RectTransformUtility.RectangleContainsScreenPoint(rectTransform, inputPosition, Camera.main))
        {
            TriggerExplosion();
        }

        // ✅ Saskare ar obstacle, kamēr velk mašīnu → zaudējums
        if (ObjectScript.drag && !isFadingOut &&
            RectTransformUtility.RectangleContainsScreenPoint(rectTransform, inputPosition, Camera.main))
        {
            if (ObjectScript.lastDragged != null)
            {
                GameManager.Instance?.OnVehicleDestroyed();

                Destroy(ObjectScript.lastDragged);
                ObjectScript.lastDragged = null;
                ObjectScript.drag = false;
            }

            image.color = CompareTag("Bomb") ? Color.red : Color.cyan;
            StartCoroutine(RecoverColor(0.5f));
            StartCoroutine(Vibrate());

            if (objectScript != null && objectScript.effects != null && objectScript.audioCli.Length > 14)
                objectScript.effects.PlayOneShot(objectScript.audioCli[14]);
        }
    }

    bool TryGetInputPosition(out Vector2 position)
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        position = Input.mousePosition;
        return true;
#elif UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            position = Input.GetTouch(0).position;
            return true;
        }
        else
        {
            position = Vector2.zero;
            return false;
        }
#endif
    }

    public void TriggerExplosion()
    {
        isExploding = true;
        objectScript.effects.PlayOneShot(objectScript.audioCli[15], 5f);

        if (TryGetComponent<Animator>(out Animator animator))
            animator.SetBool("Explode", true);

        image.color = Color.red;
        StartCoroutine(RecoverColor(0.4f));
        StartCoroutine(Vibrate());
        StartCoroutine(WaitBeforeExplode());
    }

    IEnumerator WaitBeforeExplode()
    {
        float radius = 0;
        if (TryGetComponent<CircleCollider2D>(out CircleCollider2D collider))
        {
            radius = collider.radius * rectTransform.lossyScale.x;
            ExplodeAndDestroyNearbyObjects(radius);
            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }
    }

    void ExplodeAndDestroyNearbyObjects(float radius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D hit in hits)
        {
            if (hit != null && hit.gameObject != gameObject)
            {
                ObstaclesControlerScript obj = hit.GetComponent<ObstaclesControlerScript>();
                if (obj != null && !obj.isExploding)
                    obj.StartToDestroy(Color.cyan);
            }
        }
    }

    public void StartToDestroy(Color c)
    {
        if (!isFadingOut)
        {
            StartCoroutine(FadeOutAndDestroy());
            isFadingOut = true;

            image.color = c;
            StartCoroutine(RecoverColor(0.5f));
            StartCoroutine(Vibrate());
            objectScript.effects.PlayOneShot(objectScript.audioCli[15]);
        }
    }

    IEnumerator FadeIn()
    {
        float a = 0f;
        while (a < fadeDuration)
        {
            a += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, a / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    IEnumerator FadeOutAndDestroy()
    {
        float a = 0f;
        float startAlpha = canvasGroup.alpha;

        while (a < fadeDuration)
        {
            a += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, a / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0;
        Destroy(gameObject);
    }

    IEnumerator RecoverColor(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        image.color = originalColor;
    }

    IEnumerator Vibrate()
    {
#if UNITY_ANDROID
        Handheld.Vibrate();
#endif

        Vector2 originalPosition = rectTransform.anchoredPosition;
        float duration = 0.3f;
        float elapsed = 0f;
        float intensity = 5f;

        while (elapsed < duration)
        {
            rectTransform.anchoredPosition = originalPosition + Random.insideUnitCircle * intensity;
            elapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = originalPosition;
    }

    // ✅ Debug režīms — redz minX / maxX līnijas
    void OnDrawGizmos()
    {
        if (showDebugLines && screenBoundriesScript != null)
        {
            Gizmos.color = minXColor;
            Gizmos.DrawLine(new Vector3(screenBoundriesScript.minX, -2000, 0), new Vector3(screenBoundriesScript.minX, 2000, 0));

            Gizmos.color = maxXColor;
            Gizmos.DrawLine(new Vector3(screenBoundriesScript.maxX, -2000, 0), new Vector3(screenBoundriesScript.maxX, 2000, 0));
        }
    }
}
