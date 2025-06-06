using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    private Animator animator;

    private const string IS_RUNNING = "isRunning";

    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";
    private const string LAST_HORIZONTAL = "LastHorizontal";
    private const string LAST_VERTICAL = "LastVertical";


    private void Awake()
    {
        animator = GetComponent<Animator>();

    }

    private void Update()
    {
        if (Player.Instance.IsMovementBlocked()) { return; }

        Vector2 inputVector = GameInput.Instance.GetMovementVector();
        

        animator.SetFloat(HORIZONTAL, inputVector.x);
        animator.SetFloat(VERTICAL, inputVector.y);

        if (inputVector != Vector2.zero)
        {
            animator.SetFloat(LAST_HORIZONTAL, inputVector.x);
            animator.SetFloat(LAST_VERTICAL, inputVector.y);
        }

    }
}
