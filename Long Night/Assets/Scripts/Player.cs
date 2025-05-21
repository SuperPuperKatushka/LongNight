using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    private Rigidbody2D rb;
    [SerializeField] private float movingSpeed = 5f;
    private float minMovingSpeed = 0.1f;
    private bool isRunning = false;

    private bool isMovementBlocked = false;

    public void BlockMovement() => isMovementBlocked = true;
    public bool IsMovementBlocked() => isMovementBlocked;

    public void UnblockMovement() => isMovementBlocked = false;

    // ������� �� ������� Start 
    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (PlayerPositionData.Instance != null)
        {
            transform.position = PlayerPositionData.Instance.GetSavedPosition();
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();

    }

    private void HandleMovement()
    {
        if (isMovementBlocked)
        {
            rb.linearVelocity = Vector2.zero;
            isRunning = false;
            return;
        }


        Vector2 inputVector = GameInput.Instance.GetMovementVector();
        rb.MovePosition(rb.position + inputVector * (movingSpeed * Time.fixedDeltaTime));

        // �������� ��� �������������
        if (Mathf.Abs(inputVector.x) > minMovingSpeed || Mathf.Abs(inputVector.y) > minMovingSpeed) {
            isRunning = true;
        } else
        {
            isRunning = false;
        }
    }

    public bool IsRunning()
    {
        return isRunning;
    }
}
