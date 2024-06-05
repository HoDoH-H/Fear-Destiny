using UnityEngine;
using UnityEngine.InputSystem;

public class InputEvents : MonoBehaviour
{
    public bool i_Pressed = false;

    public static InputEvents Instance;
    public InputSystem_Actions inputs;
    public InputAction interact;

    private void Awake()
    {
        Instance = this;
        inputs = new InputSystem_Actions();
    }

    private void OnEnable()
    {

        interact = inputs.Player.Interact;
        interact.started += ptti => i_Pressed = true;
        interact.canceled += ptti => i_Pressed = false;
        interact.Enable();
    }

    private void OnDisable()
    {

        interact = inputs.Player.Interact;
        interact.Disable();
    }
}
