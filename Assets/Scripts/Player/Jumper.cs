using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : MonoBehaviour
{
    // Start is called before the first frame update
    public int powerJump;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Players")
        {
            Rigidbody2D RB = collision.transform.parent.GetComponent<Rigidbody2D>(); 
            RB.velocity = new Vector2(RB.velocity.x, 0);
            RB.AddForce(Vector2.up * powerJump);
            
        }
        if (collision.tag == "Takable")
        {
            Rigidbody2D RB = collision.transform.GetComponent<Rigidbody2D>();
            RB.velocity = new Vector2(RB.velocity.x, 0);
            RB.AddForce(Vector2.up * powerJump * 9);
            print(collision);

        }
    }
}
