using UnityEngine;
using UnityEngine.SceneManagement; // ��� �������� � ���
using System.Collections;

public class EnemyController : MonoBehaviour
{
    private bool isVisible = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isVisible)
        {
            isVisible = true;
            StartCoroutine(AppearAndStartBattle());
        }
    }

    private IEnumerator AppearAndStartBattle()
    {
        // ������ ������ ������� ������ � ������ ������
        Debug.Log("����� ����� � �������, ���������� ���...");
        // ����� ������������� �������� ��������� ��� ���-�� ������
        yield return new WaitForSeconds(1f);
        // ��������� ����� �����
        SceneManager.LoadScene("BattleScene"); 
    }
}
