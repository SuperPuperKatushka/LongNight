using UnityEngine;
using UnityEngine.SceneManagement; // Для перехода в бой
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
        // Делаем объект видимым только в нужный момент
        Debug.Log("Игрок вошел в область, начинается бой...");
        // Можно воспроизвести анимацию появления или что-то другое
        yield return new WaitForSeconds(1f);
        // Загружаем сцену битвы
        SceneManager.LoadScene("BattleScene"); 
    }
}
