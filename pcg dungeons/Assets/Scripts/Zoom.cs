using UnityEngine;
using System.Collections;

public class Zoom : MonoBehaviour
{
    Camera cam;
    //public GameObject player;
    void Start()
    {
        cam = GetComponent<Camera>();
    } 
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            cam.orthographicSize--;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            cam.orthographicSize++;
        }

        //cam.transform.position = player.transform.position + new Vector3(0f, 5f, -1.67f);
    }

}