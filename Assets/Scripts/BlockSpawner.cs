using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    [Header("Prefabi (bloki no mazākā līdz lielākajam)")]
    public GameObject[] blockPrefabs; // 0block, 1block, 2block, 3block, 4block

    [Header("Kur likt sākotnējo torni")]
    public Transform startPole; // piemēram, PoleA

    [Header("Cik blokus spawn'ot (max 5)")]
    public int blockCount = 5;

    [Header("Attālums starp blokiem vertikāli")]
    public float yOffset = 0.8f;

    private void Start()
    {
        if (blockPrefabs == null || blockPrefabs.Length == 0)
        {
            Debug.LogError("❌ BlockSpawner: nav pievienoti blockPrefabs!");
            return;
        }

        if (startPole == null)
        {
            Debug.LogError("❌ BlockSpawner: startPole nav pievienots!");
            return;
        }

        SpawnTower();
    }

    private void SpawnTower()
    {
        float baseY = startPole.position.y - 0.8f; // nedaudz zemāk, lai stāvētu uz platformas

        for (int i = 0; i < blockCount && i < blockPrefabs.Length; i++)
{
    Vector3 spawnPos = new Vector3(
        startPole.position.x,
        baseY + (i * yOffset),
        0
    );

    GameObject newBlock = Instantiate(blockPrefabs[i], spawnPos, Quaternion.identity);
    newBlock.name = blockPrefabs[i].name;

    // ✅ Noņemam fiziku sākumā, lai nekristu
    Rigidbody2D rb = newBlock.GetComponent<Rigidbody2D>();
    if (rb == null)
        rb = newBlock.AddComponent<Rigidbody2D>();

    rb.gravityScale = 0; // ❗ Nekrīt sākumā
    rb.freezeRotation = true;
    rb.constraints = RigidbodyConstraints2D.FreezePositionY; // lai paliek vietā

    if (newBlock.GetComponent<BoxCollider2D>() == null)
        newBlock.AddComponent<BoxCollider2D>();

    if (newBlock.GetComponent<DragAndDropHanojaScript>() == null)
    newBlock.AddComponent<DragAndDropHanojaScript>();

}
    }
}