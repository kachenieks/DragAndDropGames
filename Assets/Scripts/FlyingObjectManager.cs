using UnityEngine;

public class FlyingObjectManager : MonoBehaviour
{
    public void DestroyAllFlyingObjects()
    {
        // ✅ Izmanto pareizo metodi ar vienu argumentu
        ObstaclesControlerScript[] flyingObjects = FindObjectsOfType<ObstaclesControlerScript>(false);


        foreach (ObstaclesControlerScript obj in flyingObjects)
        {
            if (obj == null)
                continue;

            // ✅ Izpilda darbību atkarībā no tag
            if (obj.CompareTag("Bomb"))
            {
                obj.TriggerExplosion();
            }
            else
            {
                obj.StartToDestroy(Color.cyan);
            }
        }

        Debug.Log($"🧹 {flyingObjects.Length} flying objects processed for destruction.");
    }
}
