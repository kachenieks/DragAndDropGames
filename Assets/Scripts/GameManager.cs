using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs & Canvas")]
    public List<GameObject> vehiclePrefabs;
    public List<GameObject> dropPlacePrefabs;
    public RectTransform canvasRect;

    [Header("Win UI")]
    public GameObject winPanel;
    public TMP_Text winTimeText;
    public Button restartButton;
    public Button menuButton;
    public GameTimer gameTimer;
    public GameObject starPrefab;
    public Transform starsContainer;

    [Header("Lose UI")]
    public GameObject losePanel;
    public TMP_Text loseTimeText;
    public TMP_Text loseHeaderText;
    public Button loseRestartButton;
    public Button loseMenuButton;

    [Header("Camera Settings")]
    public float cameraMoveDuration = 1.0f; // sekundes, cik ātri kamera pārvietojas
    public float cameraDistance = 2.5f; // attālums no Canvas centra

    private int totalVehicles;
    private int correctPlaced = 0;
    private bool gameEnded = false;

    private Camera mainCamera;
    private Vector3 cameraStartPos;
    private Quaternion cameraStartRot;

    public static GameManager Instance;

    void Start()
    {
        Instance = this;

        if (vehiclePrefabs.Count != dropPlacePrefabs.Count)
        {
            Debug.LogError("Vehicle and drop place counts must match!");
            return;
        }

        mainCamera = Camera.main;
        if (mainCamera == null)
            Debug.LogError("Main Camera not found!");

        totalVehicles = vehiclePrefabs.Count;
        SpawnAll();
        gameTimer.StartTimer();

        restartButton.onClick.AddListener(RestartLevel);
        menuButton.onClick.AddListener(ReturnToMenu);
        loseRestartButton.onClick.AddListener(RestartLevel);
        loseMenuButton.onClick.AddListener(ReturnToMenu);

        winPanel.SetActive(false);
        losePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void SpawnAll()
    {
        var allPositions = GenerateRandomPositions(totalVehicles * 2);

        for (int i = 0; i < dropPlacePrefabs.Count; i++)
        {
            GameObject dp = Instantiate(dropPlacePrefabs[i], canvasRect);
            dp.GetComponent<RectTransform>().anchoredPosition = allPositions[i];
        }

        for (int i = 0; i < vehiclePrefabs.Count; i++)
        {
            GameObject v = Instantiate(vehiclePrefabs[i], canvasRect);
            v.GetComponent<RectTransform>().anchoredPosition = allPositions[i + totalVehicles];
        }
    }

    List<Vector2> GenerateRandomPositions(int count)
    {
        List<Vector2> positions = new List<Vector2>();
        Vector2 canvasSize = canvasRect.sizeDelta;

        float margin = 150f;
        float minX = -canvasSize.x / 2f + margin;
        float maxX = canvasSize.x / 2f - margin;
        float minY = -canvasSize.y / 2f + margin;
        float maxY = canvasSize.y / 2f - margin;

        for (int i = 0; i < count; i++)
        {
            Vector2 pos;
            bool valid = false;
            int attempts = 0;

            do
            {
                float x = Random.Range(minX, maxX);
                float y = Random.Range(minY, maxY);
                pos = new Vector2(x, y);

                valid = positions.All(p => Vector2.Distance(p, pos) > 180f);
                attempts++;
            } while (!valid && attempts < 50);

            positions.Add(pos);
        }

        return positions;
    }

    public void OnVehicleCorrectlyPlaced()
    {
        if (gameEnded) return;

        correctPlaced++;
        if (correctPlaced >= totalVehicles)
            Win();
    }

    public void OnVehicleDestroyed()
    {
        if (gameEnded) return;
        Lose();
    }

    void Win()
    {
        if (gameEnded) return;
        gameEnded = true;

        gameTimer.StopTimer();
        Time.timeScale = 0f;

        float time = gameTimer.GetElapsedTime();
        int stars = time < 60f ? 3 : (time < 90f ? 2 : 1);

        winTimeText.text = "Laiks: " + gameTimer.timerText.text;

        foreach (Transform child in starsContainer)
            Destroy(child.gameObject);

        for (int i = 0; i < stars; i++)
        {
            GameObject star = Instantiate(starPrefab, starsContainer);
            RectTransform rt = star.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(i * 60 - 60, 0);
        }

        StartCoroutine(MoveCameraToCanvas(winPanel));
    }

    void Lose()
    {
        if (gameEnded) return;
        gameEnded = true;

        gameTimer.StopTimer();
        Time.timeScale = 0f;

        if (loseHeaderText != null)
            loseHeaderText.text = "YOU LOST!";
        if (loseTimeText != null)
            loseTimeText.text = "TIME: " + gameTimer.timerText.text;

        StartCoroutine(MoveCameraToCanvas(losePanel));
    }

    IEnumerator MoveCameraToCanvas(GameObject panel)
    {
        // Rāda tikai nepieciešamo paneli
        winPanel.SetActive(panel == winPanel);
        losePanel.SetActive(panel == losePanel);

        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;
        rt.localPosition = Vector3.zero;
        rt.localRotation = Quaternion.identity;
        panel.transform.SetAsLastSibling();

        Vector3 targetPos = canvasRect.position - mainCamera.transform.forward * cameraDistance;
        Quaternion targetRot = Quaternion.LookRotation(canvasRect.position - targetPos);

        float elapsed = 0f;
        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;

        while (elapsed < cameraMoveDuration)
        {
            elapsed += Time.unscaledDeltaTime; // Time unscaled, jo Time.timeScale=0
            float t = Mathf.Clamp01(elapsed / cameraMoveDuration);

            mainCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
            mainCamera.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

            yield return null;
        }

        // Pārliecināmies, ka ir precīzi galamērķī
        mainCamera.transform.position = targetPos;
        mainCamera.transform.rotation = targetRot;
    }

    void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
