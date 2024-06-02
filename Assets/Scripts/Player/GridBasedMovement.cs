using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridBasedMovement : MonoBehaviour
{

    public bool isWalking = false;
    public bool canMove = true;

    public Transform movePoint;

    public LayerMask whatStopsMovement;
    public LayerMask tallGrass;

    public event Action OnEncountered;

    public Animator anim;

    public float moveSpeed;

    private Vector2 direction;

    public InputSystem_Actions playerControls;
    private InputAction move;

    private void Awake()
    {
        playerControls = new InputSystem_Actions();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movePoint.parent = null;
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
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        direction = move.ReadValue<Vector2>();
        if(direction.x != 0 && direction.y != 0)
        {
            if(direction.x > 0)
            {
                direction.x = 1;
            }
            else
            {
                direction.x = -1;
            }
            direction.y = 0;
        }

        if(canMove)
        {
            if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
            {
                if (isWalking)
                {
                    CheckForEncounter();
                }

                if (isWalking && direction == Vector2.zero)
                {
                    anim.SetBool("IsWalking", false);
                    isWalking = false;
                }
                if (direction != Vector2.zero)
                {
                    anim.SetFloat("DirectionX", direction.x);
                    anim.SetFloat("DirectionY", direction.y);
                }

                if (Mathf.Abs(direction.x) > 0)
                {
                    if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(direction.x, 0, 0), 0.45f, whatStopsMovement))
                    {
                        movePoint.position += new Vector3(direction.x, 0, 0);
                    }
                }

                if (Mathf.Abs(direction.y) > 0)
                {
                    if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(0, direction.y, 0), 0.45f, whatStopsMovement))
                    {
                        movePoint.position += new Vector3(0, direction.y, 0);
                    }
                }
            }
            else
            {
                if (!isWalking)
                {
                    isWalking = true;
                    anim.SetBool("IsWalking", true);
                }
            }
        }
        else
        {
            if (isWalking) isWalking = false; anim.SetBool("IsWalking", false);
        }
    }

    private void CheckForEncounter()
    {
        if (Physics2D.OverlapCircle(movePoint.position, 0.45f, tallGrass))
        {
            if(UnityEngine.Random.Range(1, 101) <= 10)
            {
                move.Reset();
                direction = Vector2.zero;
                anim.SetBool("IsWalking", false);
                isWalking = false;
                OnEncountered();
            }
        }
    }
}
