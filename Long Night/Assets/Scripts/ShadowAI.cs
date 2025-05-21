using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public EnemyTemplate enemyTemplate;
    public EnemyOnMap enemy;
    public Animator animator;

    private bool isVisible = false;
    private bool playerInTrigger = false;
    private Coroutine battleCoroutine;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isVisible)
        {
            playerInTrigger = true;
            battleCoroutine = StartCoroutine(StartBattleCountdown());

            // Запускаем анимацию появления
            if (animator != null)
            {
                animator.SetTrigger("Appear");
            }
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && playerInTrigger)
        {
            playerInTrigger = false;

            if (battleCoroutine != null)
            {
                StopCoroutine(battleCoroutine);
            }

            // Запускаем анимацию исчезновения 
            if (animator != null)
            {
                animator.SetTrigger("Disappear");
            }

            isVisible = false;
        }
    }

    private IEnumerator StartBattleCountdown()
    {
        yield return new WaitForSeconds(1f);

        if (playerInTrigger) // Проверяем, что игрок всё ещё в триггере
        {
            StartBattle();
        }
    }

    private void StartBattle()
    {
        isVisible = true;
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerPositionData.Instance.SavePosition(player.transform.position);
            EnemyDataTransfer.Instance.SetEnemyTemplate(enemyTemplate);
            EnemyDataTransfer.Instance.SetEnemyId(enemy.enemyID);
        }
        SceneManager.LoadScene("BattleScene");
    }
}