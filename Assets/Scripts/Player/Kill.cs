using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Kill : MonoBehaviour
{
    public Vector3 StartPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Players")
        {
            collision.transform.parent.GetComponent<Rigidbody2D>().gravityScale = 2;
            collision.GetComponentInParent<Animator>().SetTrigger("Hit");
            collision.transform.parent.position = StartPoint;
        }
    }
}
