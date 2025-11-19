using UnityEngine;
using TMPro;

public class HanojaUIManager : MonoBehaviour
{
    public TextMeshProUGUI MoveCounterText;
    public TextMeshProUGUI MinMovesText;

    private TowerManager towerManager;

    private void Start()
    {
        towerManager = TowerManager.Instance;
        UpdateUI();
    }

    private void Update()
    {
        UpdateUI();

        // CHEAT CODE: Nospied O lai uzvarētu
        if (Input.GetKeyDown(KeyCode.O))
        {
            towerManager.CheatAutoWin(); // <-- SALABOTS
        }
    }

    private void UpdateUI()
    {
        if (towerManager == null) return;

        if (MoveCounterText != null)
        {
            MoveCounterText.text = $"Gājieni: {towerManager.GetMoveCount()}";
        }

        if (MinMovesText != null)
        {
            int minMoves = (int)(Mathf.Pow(2, towerManager.TotalBlocks) - 1);
            MinMovesText.text = $"Minimālie: {minMoves}";
        }
    }
}
