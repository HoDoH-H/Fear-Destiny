using System;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    public Animator anim;
    public bool canBeInteracted;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        canBeInteracted = collision.CompareTag("Player");
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Z) && canBeInteracted)
        {
            anim.SetTrigger("Open");
        }
    }
}
