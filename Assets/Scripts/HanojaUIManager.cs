using UnityEngine;
using UnityEngine.UI;

public class HanojaUIManager : MonoBehaviour
{
    public Text MoveCounterText;
    public Text MinMovesText;

    private TowerManager towerManager;

    private void Start()
    {
        towerManager = TowerManager.Instance;
        UpdateUI();
    }

    private void Update()
    {
        UpdateUI();
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