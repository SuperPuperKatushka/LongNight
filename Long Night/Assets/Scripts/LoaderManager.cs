using UnityEngine;
using UnityEngine.SceneManagement;

public class LoaderManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        if (PlayerPrefs.HasKey("StartOrContinue"))
        {
            string startOrContinue = PlayerPrefs.GetString("StartOrContinue");
            if (startOrContinue == "Start")
            { 
                QuestSystem.Instance?.Reset(); 
                QuestInitializer.Instance?.ForceInitialize();
                PlayerPrefs.DeleteAll();
                GameManager.Instance.NewGame();
                PlayerPrefs.SetString("SceneSave", "TempLocation");
                SceneManager.LoadScene("TempLocation");
            } 
            else if (startOrContinue == "Continue")
            {
                Debug.Log(PlayerPrefs.GetString("SceneSave"));
                GameManager.Instance.LoadGame();
                if (PlayerPrefs.HasKey("SceneSave"))
                {
                    string sceneToLoad = PlayerPrefs.GetString("SceneSave");
                    SceneManager.LoadScene(sceneToLoad);
                }
            }

        }

        
    }
}
