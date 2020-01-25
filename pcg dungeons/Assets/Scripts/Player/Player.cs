using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PlayerController))]
public class Player : MonoBehaviour
{
    public float moveSpeed = 5;
    FloatingJoystick joystick1;
    FloatingJoystick joystick2;
    PlayerController controller;
    

    void Start()
    {
        controller = GetComponent<PlayerController>();
        joystick1 = GameObject.FindWithTag("joystick1").GetComponent<FloatingJoystick>();
        joystick2 = GameObject.FindWithTag("joystick2").GetComponent<FloatingJoystick>();
    }

    void Update()
    {
        //float x = Input.GetAxisRaw("Horizontal");
        //float y = Input.GetAxisRaw("Vertical");
        float x = joystick1.Horizontal;
        float y = joystick1.Vertical;

        float i = joystick2.Horizontal;
        float j = joystick2.Vertical;

        //float x = joystick
        //float y = 
        float angle2 = 0.25f*Mathf.PI;
        float x1 = i * Mathf.Cos(angle2) + j * Mathf.Sin(angle2);
        float y1 = -i * Mathf.Cos(angle2) + j * Mathf.Sin(angle2);
        Vector3 lookDirection = new Vector3(x1, 0, y1);

        float angle = Mathf.PI / 4f;
        float x2 = x * Mathf.Cos(angle) + y * Mathf.Sin(angle);
        float y2 = -x * Mathf.Cos(angle) + y * Mathf.Sin(angle);
        Vector3 moveDirection = new Vector3(x2, 0, y2);

        Vector3 moveVelocity = moveDirection.normalized * moveSpeed;
        controller.Move(moveVelocity, lookDirection);
    }
}
