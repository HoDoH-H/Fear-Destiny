using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPlayerArm : MonoBehaviour
{
    bool onWall;

    public bool OnWall { get => onWall;}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Wall")
            onWall = true;



    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Wall")
            onWall = false;
    }
}
