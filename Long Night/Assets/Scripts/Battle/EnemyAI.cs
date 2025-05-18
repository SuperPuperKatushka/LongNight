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

        // ������� ���� ������
        BattleManager.Instance.PlayerTakeDamage(damage);
        PlayerStats.Instance.currentHP = Mathf.Max(0, PlayerStats.Instance.currentHP);

        // ��������� UI
        BattleManager.Instance.ui.UpdateUI();

        // �������� ���� ����� �� EnemyStats (� �� ��������� �������)
        // ��������� ���������� ���
        BattleManager.Instance.CheckBattleEnd();
       
    }
}

