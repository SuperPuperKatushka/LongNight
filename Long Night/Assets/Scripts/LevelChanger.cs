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
    //private void FadeToLevel()
    //{
    //    animator.SetTrigger("fade"); 
    //}

    //public void OnFadeComplete()
    //{
    //    SceneManager.LoadScene(levelLoad);
    //}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(levelLoad);
            PlayerPositionData.Instance.savedPosition = vector;
        }
    }
}

