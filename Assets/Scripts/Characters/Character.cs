using System;
using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float moveSpeed;

    public bool IsMoving {  get; private set; }

    [SerializeField] CharacterAnimator animator;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<CharacterAnimator>();
        SetPositionAndSnapToTile(transform.position);
    }

    public void SetPositionAndSnapToTile(Vector2 pos)
    {
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f;

        transform.position = pos;
    }

    public IEnumerator Move(Vector2 moveVec, Action OnMoveOver=null)
    {
        var targetPos = transform.position;
        if (!IsMoving)
        {
            animator.MoveX = Mathf.Clamp(moveVec.x, -1f, 1f);
            animator.MoveY = Mathf.Clamp(moveVec.y, -1f, 1f);

            targetPos.x += moveVec.x;
            targetPos.y += moveVec.y;
        }

        var ledge = CheckForLedge(targetPos);
        if (ledge != null)
        {
            if (ledge.TryToJump(this, moveVec))
                yield break;
        }

        if (!IsPathClear(targetPos))
            yield break;


        IsMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        IsMoving = false;

        OnMoveOver?.Invoke();
    }

    public void HandleUpdate()
    {
        animator.IsMoving = IsMoving;
    }

    private bool IsPathClear(Vector3 targetPos)
    {
        var diff = targetPos - transform.position;
        var dir = diff.normalized;

        if (Physics2D.BoxCast(transform.position + dir/1.65f, new Vector2(0.2f, 0.2f), 0f, dir, diff.magnitude - 1, GameLayers.i.SolidLayer | GameLayers.i.InteractableLayer | GameLayers.i.PlayerLayer) == true || Physics2D.BoxCast(transform.position + dir / 1.85f, new Vector2(0.2f, 0.2f), 0f, dir, diff.magnitude - 1, GameLayers.i.SolidLayer) == true)
        {
            return false;
        }
        return true;
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.4f, GameLayers.i.SolidLayer | GameLayers.i.InteractableLayer) != null)
        {
            return false;
        }

        return true;
    }

    Ledge CheckForLedge(Vector3 targetPos)
    {
        var collider = Physics2D.OverlapCircle(targetPos, 0.15f, GameLayers.i.LedgeLayer);
        return collider?.GetComponent<Ledge>();
    }

    public void LookTowards(Vector3 targetPos)
    {
        var xdiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        var ydiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);

        if (xdiff == 0 || ydiff == 0)
        {
            animator.MoveX = Mathf.Clamp(xdiff, -1f, 1f);
            animator.MoveY = Mathf.Clamp(ydiff, -1f, 1f);
        }
        else
        {
            Debug.LogError($"Error in LookTowards: You can't ask character to look diagonally. Current Value: {targetPos}");
        }
    }

    public CharacterAnimator Animator { get => animator; }
}
