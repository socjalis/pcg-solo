using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PlayerController))]
public class Player : MonoBehaviour
{
    public float moveSpeed = 5;
    PlayerController controller;


    void Start()
    {
        controller = GetComponent<PlayerController>();
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        float angle2 = 0.25f*Mathf.PI;
        float x1 = x * Mathf.Cos(angle2) + y * Mathf.Sin(angle2);
        float y1 = -x * Mathf.Cos(angle2) + y * Mathf.Sin(angle2);
        Vector3 lookDirection = new Vector3(x1, 0, y1);

        float angle = Mathf.PI / 4f;
        float x2 = x * Mathf.Cos(angle) + y * Mathf.Sin(angle);
        float y2 = -x * Mathf.Cos(angle) + y * Mathf.Sin(angle);
        Vector3 moveDirection = new Vector3(x2, 0, y2);

        Vector3 moveVelocity = moveDirection.normalized * moveSpeed;
        controller.Move(moveVelocity, lookDirection);
    }
}
