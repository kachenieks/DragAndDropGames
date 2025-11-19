using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance;

    public Transform PoleA;
    public Transform PoleB;
    public Transform PoleC;

    public float GroundY = -3;
    public float SpacingY = 0.7f;
    public float SnapRange = 2f;

    // Uzvaras settings
    public int TotalBlocks = 6;
    public int WinningPole = 2; // 0=A, 1=B, 2=C (pēc noklusējuma C)

    // UI elementi
    public GameObject WinPanel;
    public TextMeshProUGUI WinText; 
    public UnityEngine.UI.Button ResetButton;

    private int moveCount = 0;
    private bool gameWon = false;

    Transform[] poles;
    List<DragAndDropHanojaBlock>[] towers;

    private void Awake()
    {
        Instance = this;

        poles = new Transform[] { PoleA, PoleB, PoleC };

        towers = new List<DragAndDropHanojaBlock>[] {
            new List<DragAndDropHanojaBlock>(),
            new List<DragAndDropHanojaBlock>(),
            new List<DragAndDropHanojaBlock>()
        };
    }

    private void Start()
    {
        // Atrodam visus blokus
        var all = FindObjectsOfType<DragAndDropHanojaBlock>();
        TotalBlocks = all.Length;

        // Sakārtojam pēc izmēra (lielākais = apakšā)
        var sorted = all.OrderByDescending(b => b.SizeIndex).ToList();

        // Sākumā visi bloki uz Pole A
        int pole = 0;
        towers[pole].Clear();

        foreach (var b in sorted)
        {
            towers[pole].Add(b);
            b.CurrentPole = pole;
        }

        // Sakārto pozīcijas
        RealignTower(pole);

        // Citi torņi tukši
        towers[1].Clear();
        towers[2].Clear();

        moveCount = 0;
        gameWon = false;

        // UI setup
        if (WinPanel != null)
        {
            WinPanel.SetActive(false);
        }

        if (ResetButton != null)
        {
            ResetButton.onClick.RemoveAllListeners();
            ResetButton.onClick.AddListener(ResetGame);
        }

        Debug.Log($"[HANOJA] Spēle sākta! Bloku skaits: {TotalBlocks}, Minimālie gājieni: {Mathf.Pow(2, TotalBlocks) - 1}");
    }

    // --- SPĒLES LOĢIKA ---

    public bool CanPickUp(DragAndDropHanojaBlock b)
    {
        if (gameWon) return false; // Pēc uzvaras vairs nevar vilkt
        
        var t = towers[b.CurrentPole];
        return t.Count > 0 && t[t.Count - 1] == b;
    }

    public void RemoveBlock(DragAndDropHanojaBlock b)
    {
        towers[b.CurrentPole].Remove(b);
    }

    public bool TryPlaceBlock(DragAndDropHanojaBlock b, Vector3 pos)
    {
        int pole = ClosestPole(pos);

        if (Mathf.Abs(pos.x - poles[pole].position.x) > SnapRange)
            return false;

        // Pārbaude – nedrīkst likt lielāku uz mazāka
        if (towers[pole].Count > 0)
        {
            var top = towers[pole][towers[pole].Count - 1];
            if (b.SizeIndex > top.SizeIndex)
                return false;
        }

        towers[pole].Add(b);
        b.CurrentPole = pole;

        RealignTower(pole);
        
        // Skaita gājienus
        moveCount++;
        Debug.Log($"[HANOJA] Gājiens #{moveCount}");

        // Pārbauda uzvaras nosacījumu
        CheckWinCondition();

        return true;
    }

    public void ReturnToPole(DragAndDropHanojaBlock b)
    {
        // Atgriež bloku atpakaļ uz savu torni
        towers[b.CurrentPole].Add(b);
        RealignTower(b.CurrentPole);
    }

    public void DropToGround(DragAndDropHanojaBlock b)
    {
        b.transform.position = new Vector3(b.transform.position.x, GroundY, 0);
    }

    private void RealignTower(int pole)
    {
        for (int i = 0; i < towers[pole].Count; i++)
        {
            var b = towers[pole][i];
            b.transform.position = new Vector3(
                poles[pole].position.x,
                GroundY + i * SpacingY,
                0
            );
        }
    }

    int ClosestPole(Vector3 pos)
    {
        float best = 999f;
        int index = 0;

        for (int i = 0; i < 3; i++)
        {
            float d = Mathf.Abs(pos.x - poles[i].position.x);
            if (d < best)
            {
                best = d;
                index = i;
            }
        }
        return index;
    }

    // --- UZVARAS PĀRBAUDE ---

    private void CheckWinCondition()
    {
        if (gameWon) return;

        // Pārbauda vai visi bloki ir uz uzvaras torņa
        if (towers[WinningPole].Count == TotalBlocks)
        {
            // Pārbauda vai tie ir pareizā secībā (lielākais apakšā)
            bool correctOrder = true;
            for (int i = 0; i < towers[WinningPole].Count - 1; i++)
            {
                if (towers[WinningPole][i].SizeIndex < towers[WinningPole][i + 1].SizeIndex)
                {
                    correctOrder = false;
                    break;
                }
            }

            if (correctOrder)
            {
                gameWon = true;
                OnGameWon();
            }
        }
    }

    private void OnGameWon()
    {
        int minMoves = (int)(Mathf.Pow(2, TotalBlocks) - 1);
        string poleName = WinningPole == 0 ? "A" : (WinningPole == 1 ? "B" : "C");

        Debug.Log($"║     UZVARA!      ║");
        Debug.Log($"║  Tornis pabeigts uz: Pole {poleName}     ║");
        Debug.Log($"║  Tavi gājieni: {moveCount}              ║");
        Debug.Log($"║  Minimālie gājieni: {minMoves}          ║");

        // Parāda Win Panel
        if (WinPanel != null)
        {
            WinPanel.SetActive(true);
        }

        // Atjaunina tekstu
        if (WinText != null)
        {
            string message = $" APSVEICU! \n\n";
            message += $"Tornis pabeigts uz Pole {poleName}\n\n";
            message += $"Tavi gājieni: {moveCount}\n";
            message += $"Optimālie gājieni: {minMoves}\n\n";

            if (moveCount == minMoves)
            {
                message += " PERFEKTS REZULTĀTS! ";
            }
            else if (moveCount <= minMoves * 1.5f)
            {
                message += " Lielisks rezultāts!";
            }
            else
            {
                message += $" Pamēģini sasniegt {minMoves}!";
            }

            WinText.text = message;
        }

        // Animācija
        StartCoroutine(VictoryAnimation());
    }

    private System.Collections.IEnumerator VictoryAnimation()
    {
        // Pacelšana uz augšu visiem blokiem
        for (int i = 0; i < towers[WinningPole].Count; i++)
        {
            var block = towers[WinningPole][i];
            Vector3 startPos = block.transform.position;
            Vector3 endPos = startPos + Vector3.up * 0.3f;

            float duration = 0.3f;
            float elapsed = 0;

            while (elapsed < duration)
            {
                block.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Atgriež atpakaļ
            elapsed = 0;
            while (elapsed < duration)
            {
                block.transform.position = Vector3.Lerp(endPos, startPos, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(0.05f);
        }
    }

    // --- PUBLISKAS METODES ---

    public int GetMoveCount() => moveCount;
    public bool IsGameWon() => gameWon;

    public void ResetGame()
{
    Debug.Log("[HANOJA] Pārstartē spēli...");
    UnityEngine.SceneManagement.SceneManager.LoadScene("HanojasTornis");
}


    // --- CHEAT CODE ---
    
    public void CheatAutoWin()
    {
        // Pārvieto visus blokus uz uzvaras torni
        List<DragAndDropHanojaBlock> allBlocks = new List<DragAndDropHanojaBlock>();
        
        // Savāc visus blokus
        for (int i = 0; i < 3; i++)
        {
            allBlocks.AddRange(towers[i]);
            towers[i].Clear();
        }
        
        // Sakārto pēc izmēra (lielākais pirmais)
        allBlocks = allBlocks.OrderByDescending(b => b.SizeIndex).ToList();
        
        // Saliek uz uzvaras torņa
        foreach (var block in allBlocks)
        {
            towers[WinningPole].Add(block);
            block.CurrentPole = WinningPole;
        }
        
        RealignTower(WinningPole);
        
        // Pieskaitīt gājienu (testa režīms)
        moveCount++;
        
        // Aktivizē uzvaras ekrānu
        CheckWinCondition();
    }

    // --- REWARDED AD FUNKCIJA ---
public bool ReduceMoveByOne()
{
    if (moveCount <= 0)
    {
        Debug.Log("[ADS] Nevar samazināt gājienus – jau ir 0.");
        return false;
    }

    moveCount--;
    Debug.Log("[ADS] Gājiens samazināts! Jaunie gājieni: " + moveCount);
    return true;
}

}