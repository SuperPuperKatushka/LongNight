using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    private Animator animator;
    public string levelLoad;
    public Vector3 vector;

    private void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerPrefs.SetString("SceneSave", levelLoad);
            SceneManager.LoadScene(levelLoad);
            PlayerPositionData.Instance.savedPosition = vector;
        }
    }
}

