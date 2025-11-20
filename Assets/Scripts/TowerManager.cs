using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance;

    public Transform PoleA;
    public Transform PoleB;
    public Transform PoleC;

    public float GroundY = -3;
    public float SpacingY = 0.7f;
    public float SnapRange = 2f;

    // 🚀 FIX: kamēr velk, torņi nepārkārtojas
    public bool IsDragging = false;

    // Uzvara
    public int TotalBlocks = 6;
    public int WinningPole = 2;

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
        // Atrod visus blokus jaunajā Unity veidā
        var all = Object.FindObjectsByType<DragAndDropHanojaBlock>(FindObjectsSortMode.None);
        TotalBlocks = all.Length;

        var sorted = all.OrderByDescending(b => b.SizeIndex).ToList();

        towers[0].Clear();
        foreach (var b in sorted)
        {
            towers[0].Add(b);
            b.CurrentPole = 0;
        }

        RealignTower(0);

        towers[1].Clear();
        towers[2].Clear();

        moveCount = 0;
        gameWon = false;

        if (WinPanel != null) WinPanel.SetActive(false);

        if (ResetButton != null)
        {
            ResetButton.onClick.RemoveAllListeners();
            ResetButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("HanojasTornis");
            });
        }
    }

    // --- LOĢIKA ---

    public bool CanPickUp(DragAndDropHanojaBlock b)
    {
        if (gameWon) return false;

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

        if (towers[pole].Count > 0)
        {
            var top = towers[pole][towers[pole].Count - 1];
            if (b.SizeIndex > top.SizeIndex)
                return false;
        }

        towers[pole].Add(b);
        b.CurrentPole = pole;

        RealignTower(pole);

        moveCount++;
        CheckWinCondition();

        return true;
    }

    public void ReturnToPole(DragAndDropHanojaBlock b)
    {
        towers[b.CurrentPole].Add(b);
        RealignTower(b.CurrentPole);
    }

    private void RealignTower(int pole)
    {
        if (IsDragging) return; // 🚀 FIX: nepārbīda blokus vilkšanas laikā

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

    // --- UZVARA ---

    private void CheckWinCondition()
    {
        if (gameWon) return;

        if (towers[WinningPole].Count == TotalBlocks)
        {
            bool correct = true;

            for (int i = 0; i < towers[WinningPole].Count - 1; i++)
            {
                if (towers[WinningPole][i].SizeIndex < towers[WinningPole][i + 1].SizeIndex)
                {
                    correct = false;
                    break;
                }
            }

            if (correct)
            {
                gameWon = true;
                OnGameWon();
            }
        }
    }

    private void OnGameWon()
    {
        int minMoves = (int)(Mathf.Pow(2, TotalBlocks) - 1);

        if (WinPanel) WinPanel.SetActive(true);

        if (WinText)
        {
            WinText.text =
                $"APSVEICU!\n\n" +
                $"Tavi gājieni: {moveCount}\n" +
                $"Minimālie: {minMoves}";
        }
    }

    // --- UI PIEPRASĪTĀS METODES ---

    public int GetMoveCount() => moveCount;

    public bool IsGameWon() => gameWon;

    public void CheatAutoWin()
    {
        List<DragAndDropHanojaBlock> allBlocks = new List<DragAndDropHanojaBlock>();

        for (int i = 0; i < 3; i++)
        {
            allBlocks.AddRange(towers[i]);
            towers[i].Clear();
        }

        allBlocks = allBlocks.OrderByDescending(b => b.SizeIndex).ToList();

        foreach (var block in allBlocks)
        {
            towers[WinningPole].Add(block);
            block.CurrentPole = WinningPole;
        }

        RealignTower(WinningPole);

        moveCount++;
        CheckWinCondition();
    }

    // Reklāmas bonuss
    public bool ReduceMoveByOne()
    {
        if (moveCount <= 0) return false;
        moveCount--;
        return true;
    }
}
