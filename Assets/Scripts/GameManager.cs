using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public List<GameObject> vehiclePrefabs;
    public List<GameObject> dropPlacePrefabs;
    public RectTransform canvasRect;

    public GameObject winPanel;
    public TMP_Text winTimeText;
    public TMP_Text starsText;
    public Button restartButton;
    public Button menuButton;
    public GameTimer gameTimer;

    public GameObject losePanel;
    public TMP_Text loseTimeText;
    public Button loseRestartButton;
    public Button loseMenuButton;
    public TMP_Text loseHeaderText; // "You Lost" teksts

    public GameObject starPrefab;
    public Transform starsContainer;

    private int totalVehicles;
    private int correctPlaced = 0;

    public static GameManager Instance;

    void Start()
    {
        Instance = this;

        if (vehiclePrefabs.Count != dropPlacePrefabs.Count)
        {
            Debug.LogError("Vehicle and drop place counts must match!");
            return;
        }

        totalVehicles = vehiclePrefabs.Count;
        SpawnAll();
        gameTimer.StartTimer();

        restartButton.onClick.AddListener(RestartLevel);
        menuButton.onClick.AddListener(ReturnToMenu);
        loseRestartButton.onClick.AddListener(RestartLevel);
        loseMenuButton.onClick.AddListener(ReturnToMenu);

        winPanel.SetActive(false);
        losePanel.SetActive(false);
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
        correctPlaced++;
        Debug.Log("Pareizi novietoti: " + correctPlaced + "/" + totalVehicles);
        
        if (correctPlaced >= totalVehicles)
        {
            Win();
        }
    }

    public void OnVehicleDestroyed()
    {
        Debug.Log("Mašīna iznīcināta! Zaudējums!");
        Lose();
    }

    void Win()
    {
        gameTimer.StopTimer();
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

        winPanel.transform.SetAsLastSibling();
        winPanel.SetActive(true);
        losePanel.SetActive(false);
    }

void Lose()
{
    gameTimer.StopTimer();

    if (loseHeaderText != null)
        loseHeaderText.text = "YOU LOST!";

    if (loseTimeText != null)
            loseTimeText.text = "TIME: " + gameTimer.timerText.text;

    losePanel.SetActive(true);
        winPanel.SetActive(false);
    losePanel.transform.SetAsLastSibling();
}

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}