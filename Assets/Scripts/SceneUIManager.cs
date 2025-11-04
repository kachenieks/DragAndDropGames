using UnityEngine;

public class SceneUIManager : MonoBehaviour
{
    [Header("UI objekti CityScene")]
    public GameObject gameCanvas;      // ← Tavs "Canvas" (spēles lauks)
    public GameObject canvasButtons;   // ← Tavs "CanvasButtons" (Android pogas)

    [Header("Editor testēšanai")]
    public bool showAndroidUIInEditor = false;

    void Awake()
    {
        bool isAndroid = false;

#if UNITY_ANDROID && !UNITY_EDITOR
        isAndroid = true;
#endif

#if UNITY_EDITOR
        if (showAndroidUIInEditor)
            isAndroid = true;
#endif

        if (gameCanvas != null)
            gameCanvas.SetActive(true); // spēles Canvas vienmēr redzams

        if (canvasButtons != null)
            canvasButtons.SetActive(isAndroid); // pogas tikai Android versijā
    }
}
