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
        Vector2 currVelocity = new Vector2(0, 0);

        float deltaTime = Time.deltaTime;
        if (Input.GetKey(KeyCode.D))
            currVelocity.x += _iSpeed*deltaTime;
        if (Input.GetKey(KeyCode.Q))
            currVelocity.x -= _iSpeed * deltaTime;
        if (Input.GetKey(KeyCode.Z))
            currVelocity.y += _iSpeed*deltaTime;
        if (Input.GetKey(KeyCode.S))
            currVelocity.y -= _iSpeed * deltaTime;

        GetComponent<Rigidbody2D>().velocity = currVelocity;

    }

}
