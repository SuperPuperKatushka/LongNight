using UnityEngine;

public class BossAI : MonoBehaviour
{
    public Animator animator;

    // ������������ ��������
    private const string ATTACK_TRIGGER = "Attack";
    private const string SUPER_ATTACK_TRIGGER = "SuperAttack";
    private const string CHARGE_TRIGGER = "Charge";

    // ��������� ��������
    private const float CRITICAL_HP_THRESHOLD = 0.3f;
    private const float LOW_HP_THRESHOLD = 0.6f;
    private const float PLAYER_FINISH_THRESHOLD = 0.4f;
    private const float SUPER_ATTACK_CHANCE = 0.35f;
    private const float RAGE_MODE_THRESHOLD = 0.4f;

    private bool isRageMode = false;
    private int turnsUntilSuperAttack = 0;
    private bool isCharging = false;

    public void TakeTurn()
    {
        BattleState state = EvaluateBattleState();

        // �������� �� ����� ������
        if (!isRageMode && state.enemyHPPercent < RAGE_MODE_THRESHOLD)
        {
            EnterRageMode();
        }

        if (isCharging)
        {
            CompleteSuperAttack();
            return;
        }

        ChooseBossAction(state);
    }

    private void EnterRageMode()
    {
        isRageMode = true;
        BattleManager.Instance.ui.ShowMessage($"{EnemyStats.Instance.data.enemyName} ������� � ������!");
        animator.SetTrigger("Rage");
    }

    private BattleState EvaluateBattleState()
    {
        BattleState state = new BattleState();

        state.enemyHPPercent = EnemyStats.Instance.GetHPRatio();
        state.playerHPPercent = (float)PlayerStats.Instance.currentHP / PlayerStats.Instance.maxHP;

        state.canStrongAttack = EnemyStats.Instance.CanStrongAttack();
        state.canHeal = EnemyStats.Instance.CanHeal();

        state.isCriticalHealth = state.enemyHPPercent < CRITICAL_HP_THRESHOLD;
        state.isLowHealth = state.enemyHPPercent < LOW_HP_THRESHOLD;
        state.canFinishPlayer = state.playerHPPercent < PLAYER_FINISH_THRESHOLD;

        return state;
    }

    private void ChooseBossAction(BattleState state)
    {
        // ����� ������ ����������� ���� ����� �����
        float superAttackChance = isRageMode ? SUPER_ATTACK_CHANCE * 1.5f : SUPER_ATTACK_CHANCE;

        // 1. ����� ����� (� ������ ��� ���� ����� ����� ��������)
        if ((Random.value < superAttackChance || state.playerHPPercent < 0.3f))
        {
            StartSuperAttack();
            return;
        }

        // 2. ������� � ����������� ��������
        if (state.isCriticalHealth && state.canHeal)
        {
            PerformHeal();
            return;
        }

        // 3. ��������� ������
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

        // 4. ����������� ������
        if (state.canStrongAttack && !state.isLowHealth)
        {
            PerformStrongAttack();
        }
        else if (state.isLowHealth && state.canHeal)
        {
            PerformHeal();
        }
        else
        {
            PerformAttack();
        }
    }

    private void StartSuperAttack()
    {
        isCharging = true;
        turnsUntilSuperAttack = 1;
        animator.SetTrigger(CHARGE_TRIGGER);
        BattleManager.Instance.ui.ShowMessage($"{EnemyStats.Instance.data.enemyName} �������� �������� ����������� �����!");
    }

    private void CompleteSuperAttack()
    {
        if (turnsUntilSuperAttack > 0)
        {
            turnsUntilSuperAttack--;
            BattleManager.Instance.ui.ShowMessage($"{EnemyStats.Instance.data.enemyName} ������������� �������...");
            return;
        }

        isCharging = false;
        animator.SetTrigger(SUPER_ATTACK_TRIGGER);

        int damage = CalculateDamage(EnemyStats.Instance.data.attackPower * 3);
        EnemyStats.Instance.SpendMana(5);
        BattleManager.Instance.PlayerTakeDamage(damage);

        BattleManager.Instance.ui.ShowMessage($"{EnemyStats.Instance.data.enemyName} ���������� ����������� ����� � ������� {damage} �����!");
    }

    private void PerformAttack()
    {
        animator.SetTrigger(ATTACK_TRIGGER);
        int damage = CalculateDamage(Mathf.RoundToInt(EnemyStats.Instance.data.attackPower * (isRageMode ? 1.5f : 1f)));
        BattleManager.Instance.PlayerTakeDamage(damage);
        BattleManager.Instance.ui.ShowMessage($"{EnemyStats.Instance.data.enemyName} ������� � ������� {damage} �����!");
    }

    private void PerformStrongAttack()
    {
        animator.SetTrigger(ATTACK_TRIGGER);
        int damage = CalculateDamage(Mathf.RoundToInt(EnemyStats.Instance.data.attackPower * 2.2f));
        EnemyStats.Instance.SpendMana(EnemyStats.Instance.StrongAttackCost);
        BattleManager.Instance.PlayerTakeDamage(damage);
        BattleManager.Instance.ui.ShowMessage($"{EnemyStats.Instance.data.enemyName} ���������� ������ ����� � ������� {damage} �����!");
    }

    private void PerformHeal()
    {
        int healAmount = Mathf.RoundToInt(EnemyStats.Instance.HealAmount * (isRageMode ? 1.3f : 1f));
        EnemyStats.Instance.Heal(healAmount);
        EnemyStats.Instance.SpendMana(EnemyStats.Instance.HealCost);
        BattleManager.Instance.ui.ShowMessage($"{EnemyStats.Instance.data.enemyName} ��������������� {healAmount} ��������!");
    }

    private int CalculateDamage(int baseDamage)
    {
        float variation = Random.Range(-0.1f, 0.1f);
        return Mathf.RoundToInt(baseDamage * (1 + variation));
    }
}