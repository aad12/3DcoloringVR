using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    Color bg_color = new Color(0.2f, 0.2f, 0.2f);

    public Camera cam;

    // Use this for initialization
    void Start()
    {
        cam = GetComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
    }

    // Update is called once per frame
    void Update()
    {
        cam.backgroundColor = bg_color;
    }
}
