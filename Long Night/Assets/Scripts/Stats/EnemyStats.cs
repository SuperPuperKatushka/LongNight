using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public static EnemyStats Instance;

    public string enemyName;
    public int maxHP;
    public int currentHP;
    public int attackPower;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        // «агружаем данные из шаблона врага
        var template = EnemyDataTransfer.Instance.currentEnemyTemplate;
        if (template != null)
        {
            enemyName = template.enemyName;
            maxHP = template.maxHP;
            currentHP = maxHP;
            attackPower = template.attackPower;
        }
        else
        {
            Debug.LogWarning("Ќе найден шаблон дл€ создани€ статов врага");
        }
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        currentHP = Mathf.Max(0, currentHP);
    }

    public bool IsDead()
    {
        return currentHP <= 0;
    }
}
