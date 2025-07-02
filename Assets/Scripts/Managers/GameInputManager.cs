using UnityEngine;

public class GameInputManager : MonoBehaviour
{
    public static System.Action<float> PlayerInputX;
    public static System.Action PlayerJump;

    [Header("Player")]
    public float InputX;
    private GameInput gameInput;

    private void Awake()
    {
        GetInput();
    }
    private void GetInput()
    {
        gameInput = new GameInput();

        gameInput.Enable();

        gameInput.Player.Jump.performed += JumpPerformed;
        gameInput.UI.Escape.performed += EscapePerformed;
    }

    private void EscapePerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (obj.ReadValueAsButton())
        {
            Debug.Log("Pause Screen");
        }
    }

    private void JumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if(obj.ReadValueAsButton())
        {
            PlayerJump?.Invoke();
        }
    }
    private void SetInput()
    {
        //Input X
        InputX = gameInput.Player.Move.ReadValue<Vector2>().x;
        PlayerInputX?.Invoke(InputX);
    }
    private void Update()
    {
        SetInput();
    }
}