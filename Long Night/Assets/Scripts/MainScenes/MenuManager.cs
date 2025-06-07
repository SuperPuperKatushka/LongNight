using UnityEngine;
using UnityEngine.SceneManagement; // Для перехода в бой


public class MenuManager : MonoBehaviour
{
    private void Start()
    {
        
    }
    public void StartGame()
    {
        PlayerPrefs.SetString("StartOrContinue", "Start");
        SceneManager.LoadScene("LoaderScene");
    }

    public void ContinueGame()
    {
        PlayerPrefs.SetString("StartOrContinue", "Continue");
        SceneManager.LoadScene("LoaderScene");
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
