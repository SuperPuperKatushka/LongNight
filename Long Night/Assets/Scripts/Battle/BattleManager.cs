using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    public EnemyStats enemy;
    public BattleUI ui;

    private float shieldReduction = 0f; // Множитель снижения урона (0 = нет щита)
    private bool _realityRiftBonus = false; // Бонус от Разрыва реальности

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
        ui.ShowMessage($"Ейден использует обычную атаку и наносит врагу {damage} урона!");
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
            ui.ShowMessage($"Эйдан использует сильную атаку и наносит врагу {damage} урона!");
            EndPlayerTurn();
        }
        else
        {
            ui.ShowMessage("Недостаточно маны для сильной атаки!");
        }
    }


    public void EndPlayerTurn()
    {
        //StartCoroutine(ChangeTurn());
        isPlayerTurn = false;
        ui.ChangeTitle("Ход врага!");
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
        ui.ChangeTitle("Твой ход!");
    }

    public void PlayerTakeDamage(int damage)
    {
        if (shieldReduction > 0)
        {
            int reducedDamage = Mathf.RoundToInt(damage * (1 - shieldReduction));
            Debug.Log("reducedDamage" + reducedDamage);
            Debug.Log("damage" + damage);

            damage = reducedDamage;
            shieldReduction = 0f; // Сбрасываем щит после использования
            ui.ShowMessage($"Щит поглотил часть урона! Получено {damage} урона");
        }

        if (_realityRiftBonus)
        {
            damage = Mathf.RoundToInt(damage * 1.5f); // Критический урон
            _realityRiftBonus = false;
            ui.ShowMessage("Критический урон из реальности!");
        }

        ui.ShowMessage($"Тень наносит Эйдану {damage} урона!");
        PlayerStats.Instance.TakeDamage(damage);
        ui.UpdateUI();
        CheckBattleEnd();
    }


    public void CheckBattleEnd()
    {
        if (enemy.currentHP <= 0)
        {
            ui.ShowMessage("Ты победил!");
            //ui.ShowVictoryScreen(); // метод покажем ниже
            EnemyDataTransfer.Instance.shouldDestroyEnemy = true;
            SceneManager.LoadScene("SampleScene");
            isPlayerTurn = false;
            return;
        }

        if (PlayerStats.Instance.currentHP <= 0)
        {
            ui.ShowMessage("Ты проиграл...");
            //ui.ShowDefeatScreen(); // метод тоже покажем
            isPlayerTurn = false;
            return;
        }
    }

    private bool CanUseSkill(ItemData skill, int manaCost)
    {
        if (!isPlayerTurn)
        {
            ui.ShowMessage("Сейчас не ваш ход!");
            return false;
        }

        if (PlayerStats.Instance.currentMana < manaCost)
        {
            ui.ShowMessage($"Недостаточно маны! Нужно { manaCost }");
            return false;
        }

        return true;
    }

    private bool CanUseSkill(ItemData skill)
    {
        if (!isPlayerTurn)
        {
            ui.ShowMessage("Сейчас не ваш ход!");
            return false;
        }

        return true;
    }



    internal void UseLifeImpulse(ItemData skill, int manaCost)
    {
        if (!CanUseSkill(skill, manaCost) ) return;

        // Лечение 25% от максимального HP
        int healAmount = Mathf.RoundToInt(PlayerStats.Instance.maxHP * 0.25f);
        PlayerStats.Instance.Heal(healAmount);
        PlayerStats.Instance.SpendMana(1); // Стоимость 1 маны

        ui.ShowMessage($"Жизненный импульс! +{healAmount} HP");
        EndPlayerTurn();
    }

    internal void UseFuryFlash(ItemData skill, int manaCost)
    {
        if (!CanUseSkill(skill, manaCost)) return;

        // Урон 1.8x от атаки
        int damage = Mathf.RoundToInt(PlayerStats.Instance.attackPower * 1.8f);
        enemy.TakeDamage(damage);
        PlayerStats.Instance.SpendMana(2); // Стоимость 2 маны

        ui.ShowMessage($"Вспышка ярости! {damage} урона");
        EndPlayerTurn();
    }

    internal void UseGuardianShield(ItemData skill, int manaCost)
    {
        if (!CanUseSkill(skill, manaCost)) return;

        // Уменьшение следующего урона на 50%
        shieldReduction = 0.5f;
        PlayerStats.Instance.SpendMana(2); // Стоимость 2 маны

        ui.ShowMessage($"Щит стража! Следующая атака ослаблена на 50%");
        EndPlayerTurn();
    }

    internal void UseRealityRift(ItemData skill, int manaCost)
    {
        if (!CanUseSkill(skill, manaCost)) return;

        // 50% шанс на эффект
        bool beneficialEffect = UnityEngine.Random.value > 0.5f;

        if (beneficialEffect)
        {
            // Урон 35% от атаки
            int damage = Mathf.RoundToInt(PlayerStats.Instance.attackPower * 0.35f);
            enemy.TakeDamage(damage);
            ui.ShowMessage($"Разрыв реальности! {damage} урона врагу");
        }
        else
        {
            // Потеря 25% текущего HP
            int damage = Mathf.RoundToInt(PlayerStats.Instance.currentHP * 0.25f);
            PlayerStats.Instance.TakeDamage(damage);
            ui.ShowMessage($"Разрыв реальности! Вы получили {damage} урона");
        }

        _realityRiftBonus = beneficialEffect;
        PlayerStats.Instance.SpendMana(1); // Стоимость 1 маны
        EndPlayerTurn();
    }


    public void EndBattle()
    {
        SceneManager.LoadScene("SampleScene");
    }

}

