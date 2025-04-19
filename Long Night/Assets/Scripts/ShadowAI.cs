using UnityEngine;
using UnityEngine.SceneManagement; // Для перехода в бой
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public EnemyTemplate enemyTemplate;
    public EnemyOnMap enemy;

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
        yield return new WaitForSeconds(1f);
        // Загружаем сцену битвы
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerPositionData.Instance.SavePosition(player.transform.position);
            EnemyDataTransfer.Instance.SetEnemyTemplate(enemyTemplate);
            EnemyDataTransfer.Instance.SetEnemyId(enemy.enemyID);
            Debug.Log("SavedPosition" + PlayerPositionData.Instance.GetSavedPosition());
        }
        SceneManager.LoadScene("BattleScene");
    }
}
