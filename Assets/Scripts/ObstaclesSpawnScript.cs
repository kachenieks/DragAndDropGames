using UnityEngine;

public class ObstaclesSpawnScript : MonoBehaviour
{

    public GameObject[] cloudsPrefabs;
    public GameObject[] obstaclesPrefabs;
    public Transform spawnPoint;
    public float cloudSpawnInterval = 3f;
    public float obstaclesSpawnInterval = 2f;

    public float minY = -540f;
    public float maxY = 540f;

    public float cloudMinSpeed = 1.5f;
    public float cloudMaxSpeed = 150f;

    public float obstaclesMinSpeed = 2f;
    public float obstaclesMaxSpeed = 200f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnCloud), 0f, cloudSpawnInterval);
        InvokeRepeating(nameof(SpawnObstacles), 0f, obstaclesSpawnInterval);
    }

    // Update is called once per frame
    void SpawnCloud()
    {
        if(cloudsPrefabs.Length == 0)
            return;

        GameObject cloudPrefab = cloudsPrefabs[Random.Range(0, cloudsPrefabs.Length)];
        float y = Random.Range(minY, maxY);
        Vector3 spawnPosition = new Vector3(spawnPoint.position.x, y, spawnPoint.position.z);
        GameObject cloud = Instantiate(cloudPrefab, spawnPosition, Quaternion.identity, spawnPoint);
        float movementSpeed = Random.Range(cloudMinSpeed, cloudMaxSpeed);
        ObstaclesControlerScript controller = cloud.GetComponent<ObstaclesControlerScript>();
        controller.speed = movementSpeed;

    }

    void SpawnObstacles()
    {
        if (cloudsPrefabs.Length == 0)
            return;

        GameObject obstaclePrefab = obstaclesPrefabs[Random.Range(0, obstaclesPrefabs.Length)];
        float y = Random.Range(minY, maxY);
        Vector3 spawnPosition = new Vector3(-spawnPoint.position.x, y, spawnPoint.position.z);
        GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity, spawnPoint);
        float movementSpeed = Random.Range(obstaclesMinSpeed, obstaclesMaxSpeed);
        ObstaclesControlerScript controller = obstacle.GetComponent<ObstaclesControlerScript>();
        controller.speed = -movementSpeed;
    }
}
