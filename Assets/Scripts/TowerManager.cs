using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance;

    public Transform PoleA;
    public Transform PoleB;
    public Transform PoleC;

    public float GroundY = -3;
    public float SpacingY = 0.7f;
    public float SnapRange = 2f;

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
    // atrodam visus blokus
    var all = FindObjectsOfType<DragAndDropHanojaBlock>();

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

    // Uzreiz sakārto pozīcijas
    RealignTower(pole);

    // Citi torņi tukši
    towers[1].Clear();
    towers[2].Clear();
}


    // --- SPĒLES LOĢIKA ---

    public bool CanPickUp(DragAndDropHanojaBlock b)
    {
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

        // pārbaude – nedrīkst likt lielāku uz mazāka
        if (towers[pole].Count > 0)
        {
            var top = towers[pole][towers[pole].Count - 1];
            if (b.SizeIndex > top.SizeIndex)
                return false;
        }

        towers[pole].Add(b);
        b.CurrentPole = pole;

        RealignTower(pole);
        return true;
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
}
