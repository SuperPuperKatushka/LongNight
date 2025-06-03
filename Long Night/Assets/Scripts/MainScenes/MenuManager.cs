using UnityEngine;
using UnityEngine.SceneManagement; // ��� �������� � ���


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
        UnityEditor.EditorApplication.isPlaying = false; // ��������� Play Mode � ���������
    #else
            Application.Quit(); // ����� �� ��������� ������ ����
    #endif
    }
}
