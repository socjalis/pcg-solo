using UnityEngine;
using System.Collections;

public class Zoom : MonoBehaviour
{
    Camera cam;
    public static GameObject player;
    float camHeight = 4f;
    void Start()
    {
        cam = GetComponent<Camera>();
    } 
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            camHeight *= 1.2f;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            camHeight *= 0.833333f;
        }
        if (player != null)
        {
            cam.transform.position = player.transform.position + new Vector3(-2.3f, camHeight, -2.3f);
        }
    }

}