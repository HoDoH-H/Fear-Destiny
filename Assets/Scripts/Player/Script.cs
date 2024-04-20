using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script : MonoBehaviour
{

    public float _iSpeed;
    public float _iPowerJump;
    public Player_Feet _feet;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 currVelocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);

        float deltaTime = Time.deltaTime;
        if (Input.GetKey(KeyCode.D))
            currVelocity.x += _iSpeed*deltaTime;
        if (Input.GetKey(KeyCode.Q))
            currVelocity.x -= _iSpeed * deltaTime;
            
        if (Input.GetKeyDown(KeyCode.Space) && _feet.JumpEnable)
        {
            currVelocity = new Vector2(currVelocity.x, 0);
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * _iPowerJump);
        }
        GetComponent<Rigidbody2D>().velocity = currVelocity;

    }

}
