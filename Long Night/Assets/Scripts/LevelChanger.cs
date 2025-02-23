using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    private Animator animator;
    public int levelLoad;

    private void Start()
    {
        
    }
    private void FadeToLevel()
    {
        animator.SetTrigger("fade"); 
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(levelLoad);
    }
}

