using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSettings : MonoBehaviour
{
    public void SaveGameByButton()
    {
        GameManager.Instance.SaveGame();
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerPositionData.Instance.SavePosition(player.transform.position);
        }
    }

    public void ExitGameByButton()
    {
        SceneManager.LoadScene("StartScene");
    }
}
