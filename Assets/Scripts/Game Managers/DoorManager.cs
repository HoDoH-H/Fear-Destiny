using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorManager : MonoBehaviour
{
    public Animator anim;
    public bool canBeInteracted;

    private PlayerController player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        canBeInteracted = collision.CompareTag("Player");
        player = collision.GetComponent<PlayerController>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canBeInteracted = !collision.CompareTag("Player");
        player = null;
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            if (player.input.y > 0 && canBeInteracted)
            {
                anim.SetTrigger("Open");
            }
        }
    }
}
