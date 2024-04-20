using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Feet : MonoBehaviour
{
    public bool JumpEnable;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            JumpEnable = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            JumpEnable = false;
        }
    }
}
