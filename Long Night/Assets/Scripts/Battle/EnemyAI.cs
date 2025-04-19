using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public void TakeTurn()
    {
        int damage = EnemyStats.Instance.attackPower;
        int extraDamagePoints = Random.Range((damage / 2) * -1, damage / 2);
        damage = damage + extraDamagePoints;

        // Наносим урон игроку
        PlayerStats.Instance.currentHP -= damage;
        PlayerStats.Instance.currentHP = Mathf.Max(0, PlayerStats.Instance.currentHP);

        // Показываем сообщение
        BattleManager.Instance.ui.ShowMessage($"{EnemyStats.Instance.enemyName} атакует и наносит {damage} урона!");

        // Обновляем UI
        BattleManager.Instance.ui.UpdateUI();

        // Получаем силу атаки из EnemyStats (а не случайным образом)
        // Проверяем завершение боя
        BattleManager.Instance.CheckBattleEnd();
    }
}

