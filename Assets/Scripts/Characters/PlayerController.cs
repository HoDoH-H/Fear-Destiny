using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] string name;
    [SerializeField] Sprite sprite;

    public Vector2 input;
    public bool CanMove = true;
    private Character character;

    private InputSystem_Actions playerControls;
    private InputAction move;

    private void Awake()
    {
        character = GetComponent<Character>();
        playerControls = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();
    }

    private void OnDisable()
    {
        move = playerControls.Player.Move;
        move.Disable();
    }

    public void HandleUpdate()
    {
        if (!character.IsMoving && CanMove)
        {
            input = move.ReadValue<Vector2>();

            if (input.x != 0) { input.y = 0; input.x = input.normalized.x; } 

            if (input != Vector2.zero)
            {
                StartCoroutine(character.Move(input, OnMoveOver));
            }
        }

        character.HandleUpdate();

        if (InputEvents.Instance.i_Pressed)
            Interact();
    }

    private void Interact()
    {
        InputEvents.Instance.i_Pressed = false;
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPos = transform.position + facingDir;

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    private void OnMoveOver()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, 0.45f, GameLayers.i.TriggerableLayers);

        foreach(var collider in colliders)
        {
            var triggerable = collider.GetComponent<IPlayerTriggerable>();
            if (triggerable != null)
            {
                triggerable.OnPlayerTriggered(this);
                break;
            }
        }
    }

    public string Name { get => name; }
    public Sprite Sprite { get => sprite; }
    public Character Character => character;
}
