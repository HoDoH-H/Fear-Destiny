using UnityEngine;
using UnityEngine.InputSystem;

public class InputEvents : MonoBehaviour
{
    public bool interact_Pressed = false;
    public bool escape_Pressed = false;

    public static InputEvents Instance;
    public InputSystem_Actions inputs;
    public InputAction interact;
    public InputAction escape;

    private void Awake()
    {
        Instance = this;
        inputs = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        interact = inputs.Player.Interact;
        interact.started += ptti => interact_Pressed = true;
        interact.canceled += ptti => interact_Pressed = false;
        interact.Enable();

        escape = inputs.Player.Escape;
        escape.started += ptti => escape_Pressed = true;
        escape.canceled += ptti => escape_Pressed = false;
        escape.Enable();
    }

    private void OnDisable()
    {

        interact = inputs.Player.Interact;
        interact.Disable();

        escape = inputs.Player.Escape;
        escape.Disable();
    }
}
