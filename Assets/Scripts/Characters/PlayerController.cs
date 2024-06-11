using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] string name;
    [SerializeField] Sprite sprite;

    public event Action OnEncountered;
    public event Action<Collider2D> OnEnterTrainersView;

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
        CheckForEncounter();
        CheckIfInTrainersView();
    }

    private void CheckForEncounter()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.45f, GameLayers.i.GrassLayer) != null)
        {
            if(UnityEngine.Random.Range(1, 101) <= 10)
            {
                move.Reset();
                input = Vector2.zero;
                character.Animator.IsMoving = false;
                OnEncountered();
            }
        }
    }

    public void CheckIfInTrainersView()
    {
        var collider = Physics2D.OverlapCircle(transform.position, 0.45f, GameLayers.i.FovLayer);
        if (collider != null)
        {
            character.Animator.IsMoving = false;
            OnEnterTrainersView?.Invoke(collider);
        }
    }

    public string Name { get => name; }
    public Sprite Sprite { get => sprite; }
}
