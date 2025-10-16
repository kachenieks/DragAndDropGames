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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if(canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        rectTransform = GetComponent<RectTransform>();

        image = GetComponent<Image>();
        originalColor = image.color;

        objectScript = Object.FindFirstObjectByType<ObjectScript>();
        screenBoundriesScript = Object.FindFirstObjectByType<ScreenBoundriesScript>();
        StartCoroutine(FadeIn());
    }

    // Update is called once per frame
    // ... iepriekšējais kods ...

void Update()
{
    float waveOffset = Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;
    rectTransform.anchoredPosition += new Vector2(-speed * Time.deltaTime, waveOffset * Time.deltaTime);
    
    // Iznīcināšana, kad iziet ārpus ekrāna
    if ((speed > 0 && transform.position.x < (screenBoundriesScript.minX + 80)) ||
        (speed < 0 && transform.position.x > (screenBoundriesScript.maxX - 80)))
    {
        if (!isFadingOut)
        {
            isFadingOut = true;
            StartCoroutine(FadeOutAndDestroy());
        }
    }

    // Bomba bez vilkšanas
    if (CompareTag("Bomb") && !isExploding && 
        RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, Camera.main))
    {
        TriggerExplosion();
    }

    // ✅ SASKARE AR OBSTACLE KAMĒR VILK MAŠĪNU → ZAUDĒJUMS
    if (ObjectScript.drag && !isFadingOut &&
        RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, Camera.main))
    {
        if (ObjectScript.lastDragged != null)
        {
            // 👇 TIEŠI ŠEIT IZSAUC ZAUDĒJUMU
            GameManager.Instance?.OnVehicleDestroyed();

            Destroy(ObjectScript.lastDragged);
            ObjectScript.lastDragged = null;
            ObjectScript.drag = false;
        }

        // Efekti
        image.color = CompareTag("Bomb") ? Color.red : Color.cyan;
        StartCoroutine(RecoverColor(0.5f));
        StartCoroutine(Vibrate());

        if (objectScript != null && objectScript.effects != null && objectScript.audioCli.Length > 14)
        {
            objectScript.effects.PlayOneShot(objectScript.audioCli[14]);
        }
    }
}

// ... pārējais kods ...

    public void TriggerExplosion()
    {
        isExploding = true;
        objectScript.effects.PlayOneShot(objectScript.audioCli[15], 5f);

        if (TryGetComponent<Animator>(out Animator animator))
        {
            animator.SetBool("Explode", true);
        }

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
            //....
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
                {
                    obj.StartToDestroy(Color.cyan);
                }
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
        while(a < fadeDuration)
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

    IEnumerator ShrinkAndDestroy(GameObject target, float duration)
    {
        Vector3 originalScale = target.transform.localScale;
        Quaternion originalRotation = target.transform.rotation;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            target.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t / duration);
            float angle = Mathf.Lerp(0, 360, t / duration);
            target.transform.rotation = Quaternion.Euler(0, 0, angle);

            yield return null;
        }

        Destroy(target);
    }

    IEnumerator RecoverColor(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        image.color = originalColor;
    }

    IEnumerator Vibrate()
    {
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

if (ObjectScript.drag && !isFadingOut && 
    RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, Camera.main))
{
    Debug.Log("✅ Sadursme atpazīta!");

            if (ObjectScript.lastDragged != null)
            {
                Debug.Log("🚀 Iznīcina mašīnu: " + ObjectScript.lastDragged.name);
                Destroy(ObjectScript.lastDragged); // 👈 Strādā arī ar UI
                ObjectScript.lastDragged = null;
                ObjectScript.drag = false;
            }
else
{
    Debug.Log("❌ lastDragged ir NULL!");
}

    // Efekti
    image.color = Color.cyan;
    StartCoroutine(RecoverColor(5f));
    StartCoroutine(Vibrate());

    if (objectScript != null && objectScript.effects != null && objectScript.audioCli != null)
    {
        objectScript.effects.PlayOneShot(objectScript.audioCli[14]);
    }
}
    }
}
