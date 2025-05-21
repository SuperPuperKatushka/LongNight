using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Animator animator;

    // Анимационные триггеры
    private const string ATTACK_TRIGGER = "Attack";


    // Пороговые значения для принятия решений
    private const float CRITICAL_HP_THRESHOLD = 0.25f; //Враг считает своё здоровье критически низким, когда у него остаётся ≤ 25% HP.
    private const float LOW_HP_THRESHOLD = 0.5f; //Враг считает своё здоровье низким, но не критическим (≤ 50% HP). 
    private const float PLAYER_FINISH_THRESHOLD = 0.3f; // Враг считает, что игрока нужно добить, если у того ≤ 30% HP.

    public void TakeTurn()
    {
        // Получаем текущее состояние боя
        BattleState state = EvaluateBattleState();

        // Выбираем наилучшее действие на основе состояния
        ChooseBestAction(state);
    }

    /// Оценивает текущее состояние боя
    private BattleState EvaluateBattleState()
    {
        BattleState state = new BattleState();

        // Рассчитываем процент здоровья
        state.enemyHPPercent = EnemyStats.Instance.GetHPRatio();
        state.playerHPPercent = (float)PlayerStats.Instance.currentHP / PlayerStats.Instance.maxHP;

        // Проверяем доступные действия
        state.canStrongAttack = EnemyStats.Instance.CanStrongAttack();
        state.canHeal = EnemyStats.Instance.CanHeal();

        // Анализируем опасность ситуации
        state.isCriticalHealth = state.enemyHPPercent < CRITICAL_HP_THRESHOLD;
        state.isLowHealth = state.enemyHPPercent < LOW_HP_THRESHOLD;
        state.canFinishPlayer = state.playerHPPercent < PLAYER_FINISH_THRESHOLD;

        return state;
    }

    /// Выбирает оптимальное действие на основе состояния
    private void ChooseBestAction(BattleState state)
    {
        // Приоритет 1: Экстренные действия
        if (state.isCriticalHealth && state.canHeal)
        {
            PerformHeal();
            return;
        }

        // Приоритет 2: Добивание игрока
        if (state.canFinishPlayer)
        {
            if (state.canStrongAttack)
            {
                PerformStrongAttack();
            }
            else
            {
                PerformAttack();
            }
            return;
        }

        // Приоритет 3: Поведение в зависимости от типа
        switch (EnemyStats.Instance.data.enemyType)
        {
            case EnemyType.Aggressive:
                ExecuteAggressiveStrategy(state);
                break;

            case EnemyType.Healer:
                ExecuteHealerStrategy(state);
                break;
        }
    }

    /// Стратегия для агрессивного типа
    private void ExecuteAggressiveStrategy(BattleState state)
    {
        // Агрессивный враг фокусируется на атаке
        if (state.canStrongAttack && !state.isLowHealth)
        {
            // Используем сильную атаку, если не в опасности
            PerformStrongAttack();
        }
        else if (state.isLowHealth && state.canHeal)
        {
            // Лечение только в крайнем случае
            PerformHeal();
        }
        else if (state.isLowHealth && Random.value < 0.7f)
        {
            // Защита при низком HP
            PerformDefend();
        }
        else
        {
            // Стандартная атака
            PerformAttack();
        }
    }

    /// Стратегия для лекаря
    private void ExecuteHealerStrategy(BattleState state)
    {
        // 1. Критическое здоровье - лечимся любой ценой
        if (state.enemyHPPercent < 0.2f)
        {
            if (state.canHeal)
            {
                PerformHeal();
                return;
            }

            // Если нельзя лечиться - защищаемся
            PerformDefend();
            return;
        }

        // 2. Если здоровье ниже 50% и есть мана - высокий шанс лечения
        if (state.enemyHPPercent < 0.5f && state.canHeal)
        {
            // 80% шанс на лечение, 20% на защиту
            if (Random.value < 0.8f)
            {
                PerformHeal();
            }
            else
            {
                PerformDefend();
            }
            return;
        }

        // 3. Если игрок слаб - добиваем
        if (state.playerHPPercent < 0.4f)
        {
            if (state.canStrongAttack)
            {
                PerformStrongAttack();
            }
            else
            {
                PerformAttack();
            }
            return;
        }

        // 4. Лечение если HP не полное
        if (state.enemyHPPercent < 0.8f && state.canHeal && Random.value < 0.4f)
        {
            PerformHeal();
            return;
        }

        // 5. Стандартные действия
        if (state.canStrongAttack && Random.value < 0.3f)
        {
            PerformStrongAttack();
        }
        else if (state.enemyHPPercent < 0.7f && Random.value < 0.4f)
        {
            PerformDefend();
        }
        else
        {
            PerformAttack();
        }
    }
    private void PerformAttack()
    {
        animator.SetTrigger(ATTACK_TRIGGER);
        int damage = CalculateDamage(EnemyStats.Instance.data.attackPower);
        BattleManager.Instance.PlayerTakeDamage(damage);
        EnemyStats.Instance.RestoreMana(1);
        BattleManager.Instance.ui.ShowMessage($"{EnemyStats.Instance.data.enemyName} атакует и наносит {damage} урона!");
    }

    private void PerformStrongAttack()
    {
        animator.SetTrigger(ATTACK_TRIGGER);

        int damage = CalculateDamage(Mathf.RoundToInt(EnemyStats.Instance.data.attackPower * 1.8f));
        EnemyStats.Instance.SpendMana(EnemyStats.Instance.StrongAttackCost);
        BattleManager.Instance.PlayerTakeDamage(damage);
        BattleManager.Instance.ui.ShowMessage($"{EnemyStats.Instance.data.enemyName} использует мощную атаку  и наносит {damage} урона!");
    }

    private void PerformHeal()
    {
        int healAmount = EnemyStats.Instance.HealAmount;
        EnemyStats.Instance.Heal(healAmount);
        EnemyStats.Instance.SpendMana(EnemyStats.Instance.HealCost);
        BattleManager.Instance.ui.ShowMessage($"{EnemyStats.Instance.data.enemyName} восстанавливает здоровье!");
        BattleManager.Instance.ui.UpdateUI();
    }

    private void PerformDefend()
    {
        EnemyStats.Instance.StartDefense();
    }

    /// Рассчитывает итоговый урон с учетом случайного разброса (+/- 10%)
    private int CalculateDamage(int baseDamage)
    {
        float variation = Random.Range(-0.1f, 0.1f);
        return Mathf.RoundToInt(baseDamage * (1 + variation));
    }
}

public struct BattleState
{
    public float enemyHPPercent;
    public float playerHPPercent;
    public bool canStrongAttack;
    public bool canHeal;
    public bool isCriticalHealth;
    public bool isLowHealth;
    public bool canFinishPlayer;
}