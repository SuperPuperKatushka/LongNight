using UnityEngine;
using UnityEngine.SceneManagement; // ��� �������� � ���


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
        UnityEditor.EditorApplication.isPlaying = false; // ��������� Play Mode � ���������
    #else
            Application.Quit(); // ����� �� ��������� ������ ����
    #endif
    }
}
