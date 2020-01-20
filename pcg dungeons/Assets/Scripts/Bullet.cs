using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(this.transform.position.y < 0.05f)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        GetComponent<Rigidbody>().useGravity = true;
        if (col.gameObject.layer == 10)
        {
            Debug.Log(col.gameObject.name);
            Destroy(col.gameObject);
            Destroy(this.gameObject);
        }
    }
}
