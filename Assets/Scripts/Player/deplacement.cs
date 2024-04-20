using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    
    bool DoubleJump;
    bool WallJump;
    public GameObject Feet;
    public List<GameObject> Arm;
    public int PowerJump;
    public float MoveSpeed;
    bool FeetP;
    Rigidbody2D RB;
    Vector2 test;
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        FeetP = Feet.GetComponent<OnPlayerFeet>().OnSeul;
        test = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);
        GetComponent<Animator>().SetFloat("Speed", Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x));
        GetComponent<Animator>().SetFloat("VerticalSpeed", GetComponent<Rigidbody2D>().velocity.y);

        if (FeetP == false && Arm[0].GetComponent<OnPlayerArm>().OnWall == false && Arm[1].GetComponent<OnPlayerArm>().OnWall == false && FeetP == false)
            GetComponent<Animator>().SetBool("NoCollide", true);
        else
            GetComponent<Animator>().SetBool("NoCollide", false);
        if (Arm[1].GetComponent<OnPlayerArm>().OnWall == true && Arm[0].GetComponent<OnPlayerArm>().OnWall == false)
            GetComponent<Animator>().SetTrigger("WallJump");
        else if (Arm[0].GetComponent<OnPlayerArm>().OnWall == true && Arm[1].GetComponent<OnPlayerArm>().OnWall == false && FeetP == false)
            GetComponent<Animator>().SetTrigger("WallJump");

        if (Input.GetKey(KeyCode.D))
        {
            if (FeetP == false)
            {
                GetComponent<Animator>().SetBool("GroundCollide", false);
                if (WallJump == true)
                {
                    GetComponent<SpriteRenderer>().flipX = false;
                    test.x += MoveSpeed;
                }
            }
            else
            {
                GetComponent<Animator>().SetBool("GroundCollide", true);
                GetComponent<SpriteRenderer>().flipX = false;
                test.x += MoveSpeed;
            }
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            if (FeetP == false)
            {
                if (WallJump == true)
                {
                    GetComponent<SpriteRenderer>().flipX = true;
                    test.x += -MoveSpeed;
                }
            }
            else
            {
                GetComponent<SpriteRenderer>().flipX = true;
                test.x += -MoveSpeed;
            }

        }

        if (FeetP == true)
        {
            RB.velocity = test;
        }
        if (WallJump == true && FeetP == false)
        {
            RB.velocity = test;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            
            if(FeetP == true)
            {
                RB.AddForce(Vector2.up * PowerJump);
                DoubleJump = true;
                WallJump = true;
            }
            else if(DoubleJump == true || WallJump == true)
            {
                if (DoubleJump == true && Arm[0].GetComponent<OnPlayerArm>().OnWall == false && Arm[1].GetComponent<OnPlayerArm>().OnWall == false)
                {
                    RB.velocity = new Vector2(RB.velocity.x, 0);
                    RB.AddForce(Vector2.up * PowerJump);
                    DoubleJump = false;
                    GetComponent<Animator>().SetTrigger("DoubleJump");
                }
                if (WallJump == true)
                {
                    if (Arm[0].GetComponent<OnPlayerArm>().OnWall == true)
                    {

                        GetComponent<SpriteRenderer>().flipX = true;
                        RB.velocity = new Vector2(0, 0);
                        RB.AddForce(Vector2.up * PowerJump);
                        RB.AddForce(Vector2.left * 450);
                        WallJump = false;

                    }
                    else if (Arm[1].GetComponent<OnPlayerArm>().OnWall == true)
                    {
                        GetComponent<SpriteRenderer>().flipX = false;
                        RB.velocity = new Vector2(0, 0);
                        RB.AddForce(Vector2.up * PowerJump);
                        RB.AddForce(Vector2.right * 450);
                        WallJump = false;
                    }
                }

            }

        } 
    }
}
