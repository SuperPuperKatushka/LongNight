using UnityEngine;

public class EnemyDataTransfer : MonoBehaviour
{
    public static EnemyDataTransfer Instance;

    public EnemyTemplate currentEnemyTemplate;
    public bool shouldDestroyEnemy = false;
    public string enemyId;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetEnemyTemplate(EnemyTemplate template)
    {
        currentEnemyTemplate = template;
    }

    public void SetEnemyId(string enemyId)
    {
        this.enemyId = enemyId;
    }
}
