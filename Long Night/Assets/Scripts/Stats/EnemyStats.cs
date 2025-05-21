using UnityEngine;
using System.Linq;

public class EnemyStats : MonoBehaviour
{
    public static EnemyStats Instance;

    [Header("Current State")]
    public int currentHP;
    public int currentMana;
    public int maxHP;
    public bool isDefending = false;
    public int consecutiveAttacks = 0;
    public string enemyName;
    public int attackPower;

    [Header("Stats")]
    public EnemyData data;
    public int StrongAttackCost = 2;
    public int HealCost = 3;
    public int HealAmount = 25;

    private float defenseModifier = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeFromTemplate();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeFromTemplate()
    {
        var template = EnemyDataTransfer.Instance.currentEnemyTemplate;

        data = new EnemyData
        {
            enemyName = template.enemyName,
            enemyType = template.enemyType,
            maxHP = template.maxHP,
            attackPower = template.attackPower,
            defensePower = template.defensePower,
            manaPool = template.manaPool,
            strongAttackChance = template.strongAttackChance,
            healThreshold = template.healThreshold,
            playerFinishThreshold = template.playerFinishThreshold,
        };

        currentHP = data.maxHP;
        currentMana = data.manaPool;
    }

    public void TakeDamage(int damage)
    {
        int actualDamage = Mathf.RoundToInt(damage * defenseModifier);
        currentHP -= actualDamage;
        currentHP = Mathf.Max(0, currentHP);

        if (isDefending)
        {
            isDefending = false;
            defenseModifier = 1f;
            BattleManager.Instance.ui.ShowMessage("Защита врага сломана!");
        }
    }

    public bool SpendMana(int amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            currentMana = Mathf.Max(0, currentMana);
            return true;
        }
        return false;
    }


    /// Восстановление маны (не больше максимального значения)
    public void RestoreMana(int amount)
    {
        currentMana = Mathf.Min(data.manaPool, currentMana + amount);
    }


    /// Лечение с указанием точного количества HP
    public void Heal(int amount)
    {
        currentHP = Mathf.Min(maxHP, currentHP + amount);
    }

    /// Лечение процентом от максимального HP
    public void HealPercent(float percent)
    {
        int healAmount = Mathf.RoundToInt(maxHP * Mathf.Clamp01(percent));
        Heal(healAmount);
    }

    public void StartDefense()
    {
        isDefending = true;
        defenseModifier = 0.5f; // Уменьшение урона на 50%
        BattleManager.Instance.ui.ShowMessage("Враг готовится к защите!");
    }

    public bool CanStrongAttack()
    {
        return currentMana >= StrongAttackCost;
    }

    public bool CanHeal()
    {
        return currentMana >= HealCost && currentHP < data.maxHP;
    }

    public float GetHPRatio()
    {
        return (float)currentHP / data.maxHP;
    }

    public bool IsDead()
    {
        return currentHP <= 0;
    }
}