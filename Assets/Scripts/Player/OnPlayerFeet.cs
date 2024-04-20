using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPlayerFeet : MonoBehaviour
{
    bool onSeul ;

    public bool OnSeul { get => onSeul;}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Ground" || collision.tag == "Takable")
            onSeul = true;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Ground" || collision.tag == "Takable")
            onSeul = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ground" || collision.tag == "Takable")
            onSeul = false;
    }
}
