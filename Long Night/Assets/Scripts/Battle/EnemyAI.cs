using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyAI : MonoBehaviour
{
    public Animator animator;
    public void TakeTurn()
    {
        animator.SetBool("isAttacking", true);

        int damage = EnemyStats.Instance.attackPower;
        int extraDamagePoints = Random.Range((damage / 2) * -1, damage / 2);
        damage = damage + extraDamagePoints;

        // Наносим урон игроку
        BattleManager.Instance.PlayerTakeDamage(damage);
        PlayerStats.Instance.currentHP = Mathf.Max(0, PlayerStats.Instance.currentHP);

        // Обновляем UI
        BattleManager.Instance.ui.UpdateUI();

        // Получаем силу атаки из EnemyStats (а не случайным образом)
        // Проверяем завершение боя
        BattleManager.Instance.CheckBattleEnd();
       
    }
}

