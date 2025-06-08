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

    private float shieldReduction = 0f; // Множитель снижения урона (0 = нет щита)
    private bool _realityRiftBonus = false; // Бонус от Разрыва реальности

    public bool isPlayerTurn = true;

    private void Awake()
    {
        // Инициализация синглтона и обновление UI
        Instance = this;
        ui.UpdateUI();
    }

    private void Start()
    {
        // Загрузка экипированных навыков и настройка кнопок
        var equippedSkills = PlayerStats.Instance.GetEquippedSkills();
        Debug.Log(equippedSkills.Count);
        SkillsUIManager.Instance.SetupSkillButtons(equippedSkills);
    }

    // Обычная атака игрока
    public void PlayerAttack()
    {
        if (!isPlayerTurn) return;

        animatorPlayer.SetTrigger(ATTACK_TRIGGER);
        int damage = PlayerStats.Instance.attackPower;
        EnemyTakeDamage(damage);
        PlayerStats.Instance.currentMana = Mathf.Min(PlayerStats.Instance.maxMana, PlayerStats.Instance.currentMana + 1);
        ui.ShowMessage($"Ейден использует обычную атаку и наносит врагу {damage} урона!");
        ui.UpdateUI();
        CheckBattleEnd();
        EndPlayerTurn();
    }

    // Сильная атака, требует 1 маны
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
            ui.ShowMessage($"Эйдан использует сильную атаку и наносит врагу {damage} урона!");
            EndPlayerTurn();
        }
        else
        {
            ui.ShowMessage("Недостаточно маны для сильной атаки!");
        }
    }

    // Завершение хода игрока и запуск хода врага
    public void EndPlayerTurn()
    {
        isPlayerTurn = false;
        ui.ChangeTitle("Ход врага!");
        StartCoroutine(EnemyTurn());
    }

    // Ход врага с задержкой
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
        ui.ChangeTitle("Твой ход!");
    }

    // Получение урона игроком, с учетом эффектов (щит, крит)
    public void PlayerTakeDamage(int damage)
    {
        if (shieldReduction > 0)
        {
            int reducedDamage = Mathf.RoundToInt(damage * (1 - shieldReduction));
            damage = reducedDamage;
            shieldReduction = 0f;
            ui.ShowMessage($"Щит поглотил часть урона! Получено {damage} урона");
        }

        if (_realityRiftBonus)
        {
            damage = Mathf.RoundToInt(damage * 1.5f);
            _realityRiftBonus = false;
            ui.ShowMessage("Критический урон из реальности!");
        }

        animatorPlayer.SetTrigger(TAKE_DAMAGE);
        PlayerStats.Instance.TakeDamage(damage);
        ui.UpdateUI();
        CheckBattleEnd();
    }

    // Проверка завершения битвы
    public void CheckBattleEnd()
    {
        if (enemy.currentHP <= 0)
        {
            
            ui.ShowMessage("Ты победил!");
            EnemyDataTransfer.Instance.shouldDestroyEnemy = true;
            PlayerPrefs.SetString("SceneSave", "SampleScene");
            PlayerStats.Instance.GainEXP(enemy.data.maxHP / 2);
            SceneManager.LoadScene("SampleScene");
            isPlayerTurn = false;
            return;
        }

        if (PlayerStats.Instance.currentHP <= 0)
        {
            Debug.Log("ааааааа" + enemy.data.enemyType);
            Debug.Log("ВВ" + (enemy.data.enemyType == EnemyType.Boss));


            if ((enemy.data.enemyType == EnemyType.Boss) || (enemy.enemyName == "Малграх"))
            {
               
                Debug.Log("Удаляем");
                PlayerPrefs.DeleteKey("ChoiceDialog_ChoiceMade");
                PlayerPrefs.DeleteKey("ChoiceDialog_Side");  
                PlayerPrefs.Save();
              
            }
            ui.ShowMessage("Ты проиграл...");
            ui.ShowDefeatScreen();
            isPlayerTurn = false;
            return;
        }
    }

    // Проверка возможности использовать навык с учетом маны
    private bool CanUseSkill(ItemData skill, int manaCost)
    {
        if (!isPlayerTurn)
        {
            ui.ShowMessage("Сейчас не ваш ход!");
            return false;
        }

        if (PlayerStats.Instance.currentMana < manaCost)
        {
            ui.ShowMessage($"Недостаточно маны! Нужно {manaCost}");
            return false;
        }

        return true;
    }

    // Проверка возможности использовать навык без маны
    private bool CanUseSkill(ItemData skill)
    {
        if (!isPlayerTurn)
        {
            ui.ShowMessage("Сейчас не ваш ход!");
            return false;
        }

        return true;
    }

    // Навык: Лечение 25% HP
    public void UseLifeImpulse(ItemData skill, int manaCost)
    {
        if (!CanUseSkill(skill, manaCost)) return;
        animatorPlayer.SetTrigger(ATTACK_TRIGGER);

        int healAmount = Mathf.RoundToInt(PlayerStats.Instance.maxHP * 0.25f);
        PlayerStats.Instance.Heal(healAmount);
        PlayerStats.Instance.SpendMana(1);

        ui.ShowMessage($"Жизненный импульс! +{healAmount} HP");
        EndPlayerTurn();
    }

    // Навык: Мощный удар с множителем 2.8
    public void UseFuryFlash(ItemData skill, int manaCost)
    {
        if (!CanUseSkill(skill, manaCost)) return;
        animatorPlayer.SetTrigger(ATTACK_TRIGGER);

        int damage = Mathf.RoundToInt(PlayerStats.Instance.attackPower * 2.8f);
        EnemyTakeDamage(damage);
        PlayerStats.Instance.SpendMana(2);

        ui.ShowMessage($"Вспышка ярости! {damage} урона");
        EndPlayerTurn();
    }

    // Навык: Уменьшение урона следующей атаки
    public void UseGuardianShield(ItemData skill, int manaCost)
    {
        if (!CanUseSkill(skill, manaCost)) return;
        animatorPlayer.SetTrigger(ATTACK_TRIGGER);

        shieldReduction = 0.5f;
        PlayerStats.Instance.SpendMana(2);

        ui.ShowMessage($"Щит стража! Следующая атака ослаблена на 50%");
        EndPlayerTurn();
    }

    // Навык: случайный эффект (урон врагу или себе)
    public void UseRealityRift(ItemData skill, int manaCost)
    {
        if (!CanUseSkill(skill, manaCost)) return;
        animatorPlayer.SetTrigger(ATTACK_TRIGGER);

        bool beneficialEffect = UnityEngine.Random.value > 0.5f;

        if (beneficialEffect)
        {
            int damage = Mathf.RoundToInt(PlayerStats.Instance.attackPower * 0.35f);
            EnemyTakeDamage(damage);
            ui.ShowMessage($"Разрыв реальности! {damage} урона врагу");
        }
        else
        {
            int damage = Mathf.RoundToInt(PlayerStats.Instance.currentHP * 0.25f);
            PlayerStats.Instance.TakeDamage(damage);
            ui.ShowMessage($"Разрыв реальности! Вы получили {damage} урона");
        }

        _realityRiftBonus = beneficialEffect;
        PlayerStats.Instance.SpendMana(1);
        EndPlayerTurn();
    }

    // Завершение битвы и переход на сцену
    public void EndBattle()
    {
        PlayerPrefs.SetString("SceneSave", "SampleScene");
        SceneManager.LoadScene("SampleScene");
    }

    // Применение урона врагу
    private void EnemyTakeDamage(int damage)
    {
        animatorEnemy.SetTrigger(TAKE_DAMAGE);
        enemy.TakeDamage(damage);
    }
}
