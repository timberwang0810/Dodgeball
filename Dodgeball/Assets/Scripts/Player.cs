﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject ballPrefab;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            Vector2 dir = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y);
            dir.Normalize();
            Debug.Log(dir);

            GameObject ball = Instantiate(ballPrefab, transform.position, Quaternion.identity);
            Rigidbody2D b = ball.GetComponent<Rigidbody2D>();
            b.velocity = dir * 30.0f;
        }
    }
}
