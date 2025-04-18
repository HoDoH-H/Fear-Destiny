using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Ledge : MonoBehaviour
{
    [SerializeField, Range(-1, 1)] int xDir;
    [SerializeField, Range(-1, 0)] int yDir;

    public bool TryToJump(Character character, Vector2 moveDir)
    {
        if (moveDir.x == xDir && moveDir.y == yDir)
        {
            StartCoroutine(Jump(character));
            return true; 
        }
        return false;
    }

    IEnumerator Jump(Character character)
    {
        GameController.Instance.PauseGame(true);
        character.Animator.IsJumping = true;

        var jumpDest = character.transform.position + new Vector3(xDir, yDir) * 2;
        yield return character.transform.DOJump(jumpDest, .3f, 1, .5f).WaitForCompletion();

        character.Animator.IsJumping = false;
        GameController.Instance.PauseGame(false);
    }
}
