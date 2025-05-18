using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    public EnemyStats enemy;
    public BattleUI ui;

    private float shieldReduction = 0f; // ��������� �������� ����� (0 = ��� ����)
    private bool _realityRiftBonus = false; // ����� �� ������� ����������

    public bool isPlayerTurn = true;

    private void Awake()
    {
        Instance = this;
        ui.UpdateUI();
    }

    private void Start()
    {
        var equippedSkills = PlayerStats.Instance.GetEquippedSkills();
        Debug.Log(equippedSkills.Count);
        SkillsUIManager.Instance.SetupSkillButtons(equippedSkills);
    }

    public void PlayerAttack()
    {

        if (!isPlayerTurn)
        {
            return;
        };
        int damage = PlayerStats.Instance.attackPower;
        enemy.TakeDamage(damage);
        PlayerStats.Instance.currentMana = Mathf.Min(PlayerStats.Instance.maxMana, PlayerStats.Instance.currentMana + 1);
        ui.ShowMessage($"����� ���������� ������� ����� � ������� ����� {damage} �����!");
        ui.UpdateUI();
        CheckBattleEnd();
        EndPlayerTurn();
    }

    public void PlayerStrongAttack()
    {
        if (!isPlayerTurn)
        {
            return;

        };

        if (PlayerStats.Instance.currentMana >= 1)
        {
            int damage = Mathf.RoundToInt(PlayerStats.Instance.attackPower * 1.5f);
            PlayerStats.Instance.currentMana--;
            enemy.TakeDamage(damage);
            ui.UpdateUI();
            CheckBattleEnd();
            ui.ShowMessage($"����� ���������� ������� ����� � ������� ����� {damage} �����!");
            EndPlayerTurn();
        }
        else
        {
            ui.ShowMessage("������������ ���� ��� ������� �����!");
        }
    }


    public void EndPlayerTurn()
    {
        //StartCoroutine(ChangeTurn());
        isPlayerTurn = false;
        ui.ChangeTitle("��� �����!");
        StartCoroutine(EnemyTurn());
    }
    IEnumerator ChangeTurn()
    {
        yield return new WaitForSeconds(3f);
    }

    IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(2f);
        enemy.GetComponent<EnemyAI>().TakeTurn();
        yield return new WaitForSeconds(2f);
        isPlayerTurn = true;
        ui.ChangeTitle("���� ���!");
    }

    public void PlayerTakeDamage(int damage)
    {
        if (shieldReduction > 0)
        {
            int reducedDamage = Mathf.RoundToInt(damage * (1 - shieldReduction));
            Debug.Log("reducedDamage" + reducedDamage);
            Debug.Log("damage" + damage);

            damage = reducedDamage;
            shieldReduction = 0f; // ���������� ��� ����� �������������
            ui.ShowMessage($"��� �������� ����� �����! �������� {damage} �����");
        }

        if (_realityRiftBonus)
        {
            damage = Mathf.RoundToInt(damage * 1.5f); // ����������� ����
            _realityRiftBonus = false;
            ui.ShowMessage("����������� ���� �� ����������!");
        }

        ui.ShowMessage($"���� ������� ������ {damage} �����!");
        PlayerStats.Instance.TakeDamage(damage);
        ui.UpdateUI();
        CheckBattleEnd();
    }


    public void CheckBattleEnd()
    {
        if (enemy.currentHP <= 0)
        {
            ui.ShowMessage("�� �������!");
            //ui.ShowVictoryScreen(); // ����� ������� ����
            EnemyDataTransfer.Instance.shouldDestroyEnemy = true;
            SceneManager.LoadScene("SampleScene");
            isPlayerTurn = false;
            return;
        }

        if (PlayerStats.Instance.currentHP <= 0)
        {
            ui.ShowMessage("�� ��������...");
            //ui.ShowDefeatScreen(); // ����� ���� �������
            isPlayerTurn = false;
            return;
        }
    }

    private bool CanUseSkill(ItemData skill, int manaCost)
    {
        if (!isPlayerTurn)
        {
            ui.ShowMessage("������ �� ��� ���!");
            return false;
        }

        if (PlayerStats.Instance.currentMana < manaCost)
        {
            ui.ShowMessage($"������������ ����! ����� { manaCost }");
            return false;
        }

        return true;
    }

    private bool CanUseSkill(ItemData skill)
    {
        if (!isPlayerTurn)
        {
            ui.ShowMessage("������ �� ��� ���!");
            return false;
        }

        return true;
    }



    internal void UseLifeImpulse(ItemData skill, int manaCost)
    {
        if (!CanUseSkill(skill, manaCost) ) return;

        // ������� 25% �� ������������� HP
        int healAmount = Mathf.RoundToInt(PlayerStats.Instance.maxHP * 0.25f);
        PlayerStats.Instance.Heal(healAmount);
        PlayerStats.Instance.SpendMana(1); // ��������� 1 ����

        ui.ShowMessage($"��������� �������! +{healAmount} HP");
        EndPlayerTurn();
    }

    internal void UseFuryFlash(ItemData skill, int manaCost)
    {
        if (!CanUseSkill(skill, manaCost)) return;

        // ���� 1.8x �� �����
        int damage = Mathf.RoundToInt(PlayerStats.Instance.attackPower * 1.8f);
        enemy.TakeDamage(damage);
        PlayerStats.Instance.SpendMana(2); // ��������� 2 ����

        ui.ShowMessage($"������� ������! {damage} �����");
        EndPlayerTurn();
    }

    internal void UseGuardianShield(ItemData skill, int manaCost)
    {
        if (!CanUseSkill(skill, manaCost)) return;

        // ���������� ���������� ����� �� 50%
        shieldReduction = 0.5f;
        PlayerStats.Instance.SpendMana(2); // ��������� 2 ����

        ui.ShowMessage($"��� ������! ��������� ����� ��������� �� 50%");
        EndPlayerTurn();
    }

    internal void UseRealityRift(ItemData skill, int manaCost)
    {
        if (!CanUseSkill(skill, manaCost)) return;

        // 50% ���� �� ������
        bool beneficialEffect = UnityEngine.Random.value > 0.5f;

        if (beneficialEffect)
        {
            // ���� 35% �� �����
            int damage = Mathf.RoundToInt(PlayerStats.Instance.attackPower * 0.35f);
            enemy.TakeDamage(damage);
            ui.ShowMessage($"������ ����������! {damage} ����� �����");
        }
        else
        {
            // ������ 25% �������� HP
            int damage = Mathf.RoundToInt(PlayerStats.Instance.currentHP * 0.25f);
            PlayerStats.Instance.TakeDamage(damage);
            ui.ShowMessage($"������ ����������! �� �������� {damage} �����");
        }

        _realityRiftBonus = beneficialEffect;
        PlayerStats.Instance.SpendMana(1); // ��������� 1 ����
        EndPlayerTurn();
    }


    public void EndBattle()
    {
        SceneManager.LoadScene("SampleScene");
    }

}

