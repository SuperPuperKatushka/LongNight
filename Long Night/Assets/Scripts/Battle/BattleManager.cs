using UnityEngine;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    public EnemyStats enemy;
    public BattleUI ui;

    public bool isPlayerTurn = true;

    private void Awake()
    {
        Instance = this;
        ui.UpdateUI();
    }

    public void PlayerAttack()
    {

        if (!isPlayerTurn)
        {
            ui.ShowMessage("Ход врага!");
            return;
        };
        int damage = PlayerStats.Instance.attackPower;
        enemy.TakeDamage(damage);
        PlayerStats.Instance.currentMana = Mathf.Min(PlayerStats.Instance.maxMana, PlayerStats.Instance.currentMana + 1);
        ui.UpdateUI();
        Debug.Log("PlayerPositionData" + PlayerPositionData.Instance.GetSavedPosition());
        CheckBattleEnd();
        EndPlayerTurn();
    }

    public void PlayerStrongAttack()
    {
        if (!isPlayerTurn)
        {
            ui.ShowMessage("Ход врага!");
            return;

        };

        if (PlayerStats.Instance.currentMana >= 1)
        {
            int damage = Mathf.RoundToInt(PlayerStats.Instance.attackPower * 1.5f);
            PlayerStats.Instance.currentMana--;
            enemy.TakeDamage(damage);
            ui.UpdateUI();
            CheckBattleEnd();
            EndPlayerTurn();
        }
        else
        {
            ui.ShowMessage("Недостаточно маны для сильной атаки!");
        }
    }


    public void EndPlayerTurn()
    {
        isPlayerTurn = false;
        ui.ShowMessage("Ход врага!");
        StartCoroutine(EnemyTurn());
    }

    IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(1f);
        enemy.GetComponent<EnemyAI>().TakeTurn();
        yield return new WaitForSeconds(1f);
        isPlayerTurn = true;
        ui.ShowMessage("Твой ход!");
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

    public void EndBattle()
    {
        SceneManager.LoadScene("SampleScene");
    }
}

    //public void UseSkill(Skill skill)
    //{
    //    if (PlayerStats.Instance.currentMana < skill.manaCost)
    //    {
    //        ui.ShowMessage("Недостаточно маны для навыка!");
    //        return;
    //    }

    //    PlayerStats.Instance.currentMana -= skill.manaCost;

    //    switch (skill.type)
    //    {
    //        case SkillType.Damage:
    //            enemy.TakeDamage(skill.damageAmount);
    //            break;
    //        case SkillType.Heal:
    //            PlayerStats.Instance.currentHP = Mathf.Min(PlayerStats.Instance.maxHP, PlayerStats.Instance.currentHP + skill.healAmount);
    //            break;
    //        case SkillType.ManaRestore:
    //            PlayerStats.Instance.currentMana = Mathf.Min(PlayerStats.Instance.maxMana, PlayerStats.Instance.currentMana + skill.manaRestoreAmount);
    //            break;
    //        case SkillType.Multi:
    //            enemy.TakeDamage(skill.damageAmount);
    //            PlayerStats.Instance.currentMana = Mathf.Min(PlayerStats.Instance.maxMana, PlayerStats.Instance.currentMana + skill.manaRestoreAmount);
    //            break;
    //    }

    //    ui.UpdateUI();
    //    EndPlayerTurn();
    //}