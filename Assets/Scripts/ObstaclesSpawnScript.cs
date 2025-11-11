using UnityEngine;

public class ObstaclesSpawnScript : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] cloudsPrefabs;
    public GameObject[] obstaclesPrefabs;

    [Header("References")]
    public RectTransform canvasRect; // Canvas RectTransform
    public RectTransform spawnPoint; // tukšs objekts Canvas iekšienē

    [Header("Spawn Settings")]
    public float cloudSpawnInterval = 3f;
    public float obstaclesSpawnInterval = 2f;
    public float minY = -540f;
    public float maxY = 540f;

    [Header("Speed Settings")]
    public float cloudMinSpeed = 50f;
    public float cloudMaxSpeed = 150f;
    public float obstaclesMinSpeed = 100f;
    public float obstaclesMaxSpeed = 200f;

    void Start()
    {
        // automātiski atrod Canvas, ja nav ielikts
        if (canvasRect == null)
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
                canvasRect = canvas.GetComponent<RectTransform>();
        }

        InvokeRepeating(nameof(SpawnCloud), 0f, cloudSpawnInterval);
        InvokeRepeating(nameof(SpawnObstacles), 0f, obstaclesSpawnInterval);
    }

    // ==========================
    // ☁️ Mākoņi un bumbas — no labās uz kreiso
    // ==========================
    void SpawnCloud()
    {
        if (cloudsPrefabs.Length == 0 || canvasRect == null)
            return;

        GameObject prefab = cloudsPrefabs[Random.Range(0, cloudsPrefabs.Length)];
        GameObject cloud = Instantiate(prefab, canvasRect); // parent uz Canvas
        RectTransform rect = cloud.GetComponent<RectTransform>();

        float y = Random.Range(minY, maxY);

        // ✅ Spawn tieši aiz labās malas
        float spawnX = (canvasRect.rect.width / 2f) + 100f;
        rect.anchoredPosition = new Vector2(spawnX, y);

        ObstaclesControlerScript controller = cloud.GetComponent<ObstaclesControlerScript>();
        controller.speed = -Random.Range(cloudMinSpeed, cloudMaxSpeed); // pa kreisi
    }

    // ==========================
    // ✈️ Lidmašīnas — no kreisās uz labo
    // ==========================
    void SpawnObstacles()
    {
        if (obstaclesPrefabs.Length == 0 || canvasRect == null)
            return;

        GameObject prefab = obstaclesPrefabs[Random.Range(0, obstaclesPrefabs.Length)];
        GameObject obstacle = Instantiate(prefab, canvasRect); // parent uz Canvas
        RectTransform rect = obstacle.GetComponent<RectTransform>();

        float y = Random.Range(minY, maxY);

        // ✅ Spawn tieši aiz kreisās malas
        float spawnX = -(canvasRect.rect.width / 2f) - 100f;
        rect.anchoredPosition = new Vector2(spawnX, y);

        ObstaclesControlerScript controller = obstacle.GetComponent<ObstaclesControlerScript>();
        controller.speed = Random.Range(obstaclesMinSpeed, obstaclesMaxSpeed); // pa labi
    }
}
