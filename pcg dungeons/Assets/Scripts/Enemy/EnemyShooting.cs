using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public Transform bulletPrefab;
    public float timeout;
    public float velocity;
    public List<Transform> bullets;

    float timer;
    // Start is called before the first frame update
    void Start()
    {
        timer = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer -= Time.fixedDeltaTime;
        if (timer <= 0f)
        {
            Transform newBullet = Instantiate(bulletPrefab, transform.position, bulletPrefab.rotation) as Transform;
            Rigidbody rb = newBullet.GetComponentInChildren<Rigidbody>();

            rb.velocity = Quaternion.Euler(0, 90.0f, 0) * (transform.forward) * velocity;
            newBullet.position += Quaternion.Euler(0, 90.0f, 0) * transform.forward / 3;
            bullets.Add(newBullet);
            timer = timeout;
        }
    }
}
