using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    public Animator animatorPlayer;
    public Animator animatorEnemy;

    private const string ATTACK_TRIGGER = "Attack";
    private const string TAKE_DAMAGE = "TakeDamage";

    public EnemyStats enemy;
    public BattleUI ui;

    private float shieldReduction = 0f; // ��������� �������� ����� (0 = ��� ����)
    private bool _realityRiftBonus = false; // ����� �� ������� ����������

    public bool isPlayerTurn = true;

    private void Awake()
    {
        // ������������� ��������� � ���������� UI
        Instance = this;
        ui.UpdateUI();
    }

    private void Start()
    {
        // �������� ������������� ������� � ��������� ������
        var equippedSkills = PlayerStats.Instance.GetEquippedSkills();
        Debug.Log(equippedSkills.Count);
        SkillsUIManager.Instance.SetupSkillButtons(equippedSkills);
    }

    // ������� ����� ������
    public void PlayerAttack()
    {
        if (!isPlayerTurn) return;

        animatorPlayer.SetTrigger(ATTACK_TRIGGER);
        int damage = PlayerStats.Instance.attackPower;
        EnemyTakeDamage(damage);
        PlayerStats.Instance.currentMana = Mathf.Min(PlayerStats.Instance.maxMana, PlayerStats.Instance.currentMana + 1);
        ui.ShowMessage($"����� ���������� ������� ����� � ������� ����� {damage} �����!");
        ui.UpdateUI();
        CheckBattleEnd();
        EndPlayerTurn();
    }

    // ������� �����, ������� 1 ����
    public void PlayerStrongAttack()
    {
        if (!isPlayerTurn) return;

        if (PlayerStats.Instance.currentMana >= 1)
        {
            animatorPlayer.SetTrigger(ATTACK_TRIGGER);
            int damage = Mathf.RoundToInt(PlayerStats.Instance.attackPower * 1.5f);
            PlayerStats.Instance.currentMana--;
            EnemyTakeDamage(damage);
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

    // ���������� ���� ������ � ������ ���� �����
    public void EndPlayerTurn()
    {
        isPlayerTurn = false;
        ui.ChangeTitle("��� �����!");
        StartCoroutine(EnemyTurn());
    }

    // ��� ����� � ���������
    IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(2f);

        if (enemy.data.enemyType == EnemyType.Boss)
        {
            enemy.GetComponent<BossAI>().TakeTurn();
        } else
        {
            enemy.GetComponent<EnemyAI>().TakeTurn();
        }
        yield return new WaitForSeconds(2f);
        isPlayerTurn = true;
        ui.ChangeTitle("���� ���!");
    }

    // ��������� ����� �������, � ������ �������� (���, ����)
    public void PlayerTakeDamage(int damage)
    {
        if (shieldReduction > 0)
        {
            int reducedDamage = Mathf.RoundToInt(damage * (1 - shieldReduction));
            damage = reducedDamage;
            shieldReduction = 0f;
            ui.ShowMessage($"��� �������� ����� �����! �������� {damage} �����");
        }

        if (_realityRiftBonus)
        {
            damage = Mathf.RoundToInt(damage * 1.5f);
            _realityRiftBonus = false;
            ui.ShowMessage("����������� ���� �� ����������!");
        }

        animatorPlayer.SetTrigger(TAKE_DAMAGE);
        PlayerStats.Instance.TakeDamage(damage);
        ui.UpdateUI();
        CheckBattleEnd();
    }

    // �������� ���������� �����
    public void CheckBattleEnd()
    {
        if (enemy.currentHP <= 0)
        {
            
            ui.ShowMessage("�� �������!");
            EnemyDataTransfer.Instance.shouldDestroyEnemy = true;
            PlayerPrefs.SetString("SceneSave", "SampleScene");
            PlayerStats.Instance.GainEXP(enemy.data.maxHP / 2);
            SceneManager.LoadScene("SampleScene");
            isPlayerTurn = false;
            return;
        }

        if (PlayerStats.Instance.currentHP <= 0)
        {
            Debug.Log("�������" + enemy.data.enemyType);
            Debug.Log("��" + (enemy.data.enemyType == EnemyType.Boss));


            if ((enemy.data.enemyType == EnemyType.Boss) || (enemy.enemyName == "�������"))
            {
               
                Debug.Log("�������");
                PlayerPrefs.DeleteKey("ChoiceDialog_ChoiceMade");
                PlayerPrefs.DeleteKey("ChoiceDialog_Side");  
                PlayerPrefs.Save();
              
            }
            ui.ShowMessage("�� ��������...");
            ui.ShowDefeatScreen();
            isPlayerTurn = false;
            return;
        }
    }

    // �������� ����������� ������������ ����� � ������ ����
    private bool CanUseSkill(ItemData skill, int manaCost)
    {
        if (!isPlayerTurn)
        {
            ui.ShowMessage("������ �� ��� ���!");
            return false;
        }

        if (PlayerStats.Instance.currentMana < manaCost)
        {
            ui.ShowMessage($"������������ ����! ����� {manaCost}");
            return false;
        }

        return true;
    }

    // �������� ����������� ������������ ����� ��� ����
    private bool CanUseSkill(ItemData skill)
    {
        if (!isPlayerTurn)
        {
            ui.ShowMessage("������ �� ��� ���!");
            return false;
        }

        return true;
    }

    // �����: ������� 25% HP
    public void UseLifeImpulse(ItemData skill, int manaCost)
    {
        if (!CanUseSkill(skill, manaCost)) return;
        animatorPlayer.SetTrigger(ATTACK_TRIGGER);

        int healAmount = Mathf.RoundToInt(PlayerStats.Instance.maxHP * 0.25f);
        PlayerStats.Instance.Heal(healAmount);
        PlayerStats.Instance.SpendMana(1);

        ui.ShowMessage($"��������� �������! +{healAmount} HP");
        EndPlayerTurn();
    }

    // �����: ������ ���� � ���������� 2.8
    public void UseFuryFlash(ItemData skill, int manaCost)
    {
        if (!CanUseSkill(skill, manaCost)) return;
        animatorPlayer.SetTrigger(ATTACK_TRIGGER);

        int damage = Mathf.RoundToInt(PlayerStats.Instance.attackPower * 2.8f);
        EnemyTakeDamage(damage);
        PlayerStats.Instance.SpendMana(2);

        ui.ShowMessage($"������� ������! {damage} �����");
        EndPlayerTurn();
    }

    // �����: ���������� ����� ��������� �����
    public void UseGuardianShield(ItemData skill, int manaCost)
    {
        if (!CanUseSkill(skill, manaCost)) return;
        animatorPlayer.SetTrigger(ATTACK_TRIGGER);

        shieldReduction = 0.5f;
        PlayerStats.Instance.SpendMana(2);

        ui.ShowMessage($"��� ������! ��������� ����� ��������� �� 50%");
        EndPlayerTurn();
    }

    // �����: ��������� ������ (���� ����� ��� ����)
    public void UseRealityRift(ItemData skill, int manaCost)
    {
        if (!CanUseSkill(skill, manaCost)) return;
        animatorPlayer.SetTrigger(ATTACK_TRIGGER);

        bool beneficialEffect = UnityEngine.Random.value > 0.5f;

        if (beneficialEffect)
        {
            int damage = Mathf.RoundToInt(PlayerStats.Instance.attackPower * 0.35f);
            EnemyTakeDamage(damage);
            ui.ShowMessage($"������ ����������! {damage} ����� �����");
        }
        else
        {
            int damage = Mathf.RoundToInt(PlayerStats.Instance.currentHP * 0.25f);
            PlayerStats.Instance.TakeDamage(damage);
            ui.ShowMessage($"������ ����������! �� �������� {damage} �����");
        }

        _realityRiftBonus = beneficialEffect;
        PlayerStats.Instance.SpendMana(1);
        EndPlayerTurn();
    }

    // ���������� ����� � ������� �� �����
    public void EndBattle()
    {
        PlayerPrefs.SetString("SceneSave", "SampleScene");
        SceneManager.LoadScene("SampleScene");
    }

    // ���������� ����� �����
    private void EnemyTakeDamage(int damage)
    {
        animatorEnemy.SetTrigger(TAKE_DAMAGE);
        enemy.TakeDamage(damage);
    }
}
