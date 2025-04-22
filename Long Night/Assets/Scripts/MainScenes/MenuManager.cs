using UnityEngine;
using UnityEngine.SceneManagement; // Для перехода в бой


public class MenuManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void QuitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Остановка Play Mode в редакторе
    #else
            Application.Quit(); // Выход из собранной версии игры
    #endif
    }
}
