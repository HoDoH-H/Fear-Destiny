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
            currVelocity.y += _iSpeed;
        if (Input.GetKey(KeyCode.S))
            currVelocity.y -= _iSpeed;
        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.Z) == false && Input.GetKey(KeyCode.S) == false)
            currVelocity.x += _iSpeed;
        if (Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.Z) == false && Input.GetKey(KeyCode.S) == false)
            currVelocity.x -= _iSpeed;


        GetComponent<Rigidbody2D>().velocity = currVelocity;

    }

}
