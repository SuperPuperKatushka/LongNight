using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    private PlayerInputActions playerInputActions;

    public void Awake()
    {
        Instance = this;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        DontDestroyOnLoad(gameObject);
    }

    public Vector2 GetMovementVector()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        return inputVector;
    }

    public bool IsInventoryPressed() => playerInputActions.UI.Inventory.WasPressedThisFrame();
    public bool IsMenuPressed() => playerInputActions.UI.Menu.WasPressedThisFrame();
    public bool IsDiaryPressed() => playerInputActions.UI.Diary.WasPressedThisFrame();
    public bool IsExitPressed() => playerInputActions.UI.Exit.WasPressedThisFrame();


}
