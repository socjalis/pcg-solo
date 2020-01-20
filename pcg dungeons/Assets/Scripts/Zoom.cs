using UnityEngine;
using System.Collections;

public class Zoom : MonoBehaviour
{
    Camera cam;
    public static GameObject player;
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
        if (player != null)
        {
            cam.transform.position = player.transform.position + new Vector3(-5.5f, 8.51f, -5.5f);
        }
    }

}