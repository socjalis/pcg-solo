using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class PlayerController : MonoBehaviour
{
    Vector3 velocity;
    Vector3 lookDirection;
    Rigidbody myRigidBody;

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 _velocity, Vector3 _lookDirection)
    {
        velocity = _velocity;
        lookDirection = _lookDirection;
    }

    public void FixedUpdate()
    {
        myRigidBody.MovePosition(myRigidBody.position + velocity * Time.fixedDeltaTime);
        if (lookDirection != Vector3.zero) {
            //Quaternion q = Quaternion.LookRotation(lookDirection.normalized, Vector3.up) * Quaternion.Euler(0f, -90f, 0f);
            //Vector3 v = Vector3.RotateTowards(transform.forward, lookDirection, 20 * Mathf.Deg2Rad, 1f);

            transform.rotation = Quaternion.LookRotation(lookDirection.normalized, Vector3.up)*Quaternion.Euler(0f, -90f, 0f);
        }
        myRigidBody.velocity = new Vector3(0f, 0f, 0f);
    }
}
