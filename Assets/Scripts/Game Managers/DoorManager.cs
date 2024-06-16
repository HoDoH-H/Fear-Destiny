using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorManager : MonoBehaviour
{
    public Animator anim;
    public bool canBeInteracted = false;

    private PlayerController player;

    private void Start()
    {
        canBeInteracted = false;
        player = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(SetInteractable(collision));
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canBeInteracted = !collision.GetComponent<Character>();
        player = null;
    }

    IEnumerator SetInteractable(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>())
        {
            player = collision.GetComponent<PlayerController>();
            yield return new WaitUntil(() => !player.Character.IsMoving);
            yield return new WaitForSeconds(0.1f);
            canBeInteracted = true;
        }
        else
            canBeInteracted = false;
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            if (player.input.y > 0 && canBeInteracted)
            {
                canBeInteracted = false;
                StartCoroutine(EnterHouse());
            }
        }
    }

    IEnumerator EnterHouse()
    {
        anim.SetTrigger("Open");
        yield return new WaitForSeconds(0.5f);
        this.GetComponent<LocationPortal>()?.OnPlayerTriggered(player);
    }
}
