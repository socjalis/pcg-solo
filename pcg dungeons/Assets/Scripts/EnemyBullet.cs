using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public int lifespan;
    bool down;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, lifespan);
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.position.y < 0.05f)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        GetComponent<Rigidbody>().useGravity = true;
        if (col.gameObject.tag == "Player" && !down)
        {
            Destroy(this.gameObject);
            GameInfo.PlayerShot();
        }
        down = true;
    }
}