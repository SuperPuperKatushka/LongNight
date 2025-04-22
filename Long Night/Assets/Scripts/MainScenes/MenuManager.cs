using UnityEngine;
using UnityEngine.SceneManagement; // Для перехода в бой


public class MenuManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
