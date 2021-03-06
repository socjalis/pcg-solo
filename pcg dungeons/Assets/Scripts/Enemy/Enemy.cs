﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    public float speed;
    public Vector3 direction;
    public float directionOffset = 2f;
    public Transform deathPrefab;


    Transform player;
    float directionTimer;
    Rigidbody myRigidBody;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        direction = new Vector3();
        directionOffset += new System.Random().Next(0, 2) - 1;
        directionTimer = directionOffset;
        myRigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        myRigidBody.MovePosition(myRigidBody.position + direction * speed * Time.fixedDeltaTime);
        Quaternion q = Quaternion.identity;
        if(!direction.normalized.Equals(Vector3.zero))
        {
            q = Quaternion.LookRotation(direction.normalized, Vector3.up) * Quaternion.Euler(0f, -90f, 0f);
        }
        Vector3 w = q * Vector3.forward;
        Vector3 v = Vector3.RotateTowards(transform.forward, w, 5 * Mathf.Deg2Rad, 1f);
        transform.rotation = Quaternion.LookRotation(v);

        directionTimer -= Time.fixedDeltaTime;
        if (directionTimer < 0)
        { 
            Vector2Int pos = Helper.WorldTo2d(transform.position);
            Vector2Int dest = AI.NextStep(pos, Helper.WorldTo2d(player.position));
            directionTimer = directionOffset;
            direction = new Vector3(dest.x - pos.x, 0.0f, dest.y - pos.y).normalized;
        }
        myRigidBody.velocity = new Vector3(0f, 0f, 0f);
    }

    public void GetShot()
    {
        Instantiate(deathPrefab, transform.position, Quaternion.LookRotation(Vector3.up));
        Destroy(this.gameObject);
    }
}
