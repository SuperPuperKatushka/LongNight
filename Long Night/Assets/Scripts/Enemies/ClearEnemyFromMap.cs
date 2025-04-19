using UnityEngine;

public class EnemyCleanupOnMap : MonoBehaviour
{
    // EnemyCleanupOnMap.cs
    void Start()
    {
        if (EnemyDataTransfer.Instance.shouldDestroyEnemy)
        {
            Debug.Log(EnemyDataTransfer.Instance.enemyId);
            var enemies = FindObjectsOfType<EnemyOnMap>();
            foreach (var enemy in enemies)
            {
                if (enemy.enemyID == EnemyDataTransfer.Instance.enemyId)
                {
                    Destroy(enemy.gameObject);
                    break;
                }
            }

            EnemyDataTransfer.Instance.shouldDestroyEnemy = false;
            EnemyDataTransfer.Instance.enemyId = null;
        }
    }

}
