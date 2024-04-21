using UnityEngine;
using UnityEngine.InputSystem;

public class GridBasedMovement : MonoBehaviour
{

    public Transform movePoint;
    public Rigidbody2D rb;

    public float moveSpeed;
    private Vector2 direction;
    private Vector2 oldDirection = new Vector2(0, 0);

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

    private void Update()
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

        if(Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
        {
            if (Mathf.Abs(direction.x) > 0)
            {
                movePoint.position += new Vector3(direction.x, 0, 0);
            }

            if (Mathf.Abs(direction.y) > 0)
            {
                movePoint.position += new Vector3(0, direction.y, 0);
            }
        }
    }
}
