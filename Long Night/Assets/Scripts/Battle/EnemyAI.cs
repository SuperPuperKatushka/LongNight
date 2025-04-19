using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public void TakeTurn()
    {
        int damage = EnemyStats.Instance.attackPower;
        int extraDamagePoints = Random.Range((damage / 2) * -1, damage / 2);
        damage = damage + extraDamagePoints;

        // ������� ���� ������
        PlayerStats.Instance.currentHP -= damage;
        PlayerStats.Instance.currentHP = Mathf.Max(0, PlayerStats.Instance.currentHP);

        // ���������� ���������
        BattleManager.Instance.ui.ShowMessage($"{EnemyStats.Instance.enemyName} ������� � ������� {damage} �����!");

        // ��������� UI
        BattleManager.Instance.ui.UpdateUI();

        // �������� ���� ����� �� EnemyStats (� �� ��������� �������)
        // ��������� ���������� ���
        BattleManager.Instance.CheckBattleEnd();
    }
}

