using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public bool isRunning = false;
    private float elapsedTime = 0f;

    void Start()
    {
        StartTimer();
    }

    void Update()
    {
        if (!isRunning) return;

        // ⏱️ Skaita laiku neatkarīgi no palēnināšanas vai pauzes
        elapsedTime += Time.unscaledDeltaTime;

        int hours = Mathf.FloorToInt(elapsedTime / 3600);
        int minutes = Mathf.FloorToInt((elapsedTime % 3600) / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);

        timerText.text = $"{hours:00}:{minutes:00}:{seconds:00}";
    }

    public void StartTimer()
    {
        elapsedTime = 0f;
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public float GetElapsedTime() => elapsedTime;
}
