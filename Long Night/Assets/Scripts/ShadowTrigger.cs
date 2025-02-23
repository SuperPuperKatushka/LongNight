using UnityEngine;

public class ShadowTrigger : MonoBehaviour
{
    public Animator animator;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            animator.SetTrigger("isTriggered");
        }
    }
}
