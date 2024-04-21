using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script : MonoBehaviour
{

    public float _iSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 currVelocity = new Vector2(0, 0);

        float deltaTime = Time.deltaTime;
        if (Input.GetKey(KeyCode.Z))
        {
            currVelocity.y += _iSpeed;
            GetComponent<Animator>().SetBool("MoveUp", true);
            GetComponent<Animator>().SetBool("Idle", false);
        }
        if (Input.GetKeyUp(KeyCode.Z))
        {
            GetComponent<Animator>().SetBool("Idle", true);
            GetComponent<Animator>().SetBool("MoveUp", false);
        }
        if (Input.GetKey(KeyCode.S))
        {
            currVelocity.y -= _iSpeed;
            GetComponent<Animator>().SetBool("MoveBottom", true);
            GetComponent<Animator>().SetBool("Idle", false);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            GetComponent<Animator>().SetBool("Idle", true);
            GetComponent<Animator>().SetBool("MoveBottom", false); 
        }
        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.Z) == false && Input.GetKey(KeyCode.S) == false)
        {
            currVelocity.x += _iSpeed;
            GetComponent<Animator>().SetBool("MoveRight", true);
            GetComponent<Animator>().SetBool("Idle", false);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            GetComponent<Animator>().SetBool("Idle", true);
            GetComponent<Animator>().SetBool("MoveRight", false);
        }
        if (Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.Z) == false && Input.GetKey(KeyCode.S) == false)
        {
            currVelocity.x -= _iSpeed;
            GetComponent<Animator>().SetBool("MoveLeft", true);
            GetComponent<Animator>().SetBool("Idle", false);
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            GetComponent<Animator>().SetBool("Idle", true);
            GetComponent<Animator>().SetBool("MoveLeft", false);
        }


        GetComponent<Rigidbody2D>().velocity = currVelocity;

    }

}
