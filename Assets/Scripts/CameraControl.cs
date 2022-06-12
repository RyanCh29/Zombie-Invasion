using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script to control movement and zoom of the camera
public class CameraControl : MonoBehaviour
{

    [SerializeField] int maxCameraSize = 1;
    [SerializeField] int minCameraSize = 1;

    [SerializeField] int minSpeedModifier = 1;
    [SerializeField] int maxSpeedModifier = 1;
    int speedModifier;

    private Camera cam;

    Vector3 currentMousePos = Vector3.zero;
    Vector3 previousMousePos = Vector3.zero;

    void Start()
    {
        cam = Camera.main;
        cam.orthographicSize = maxCameraSize;
        speedModifier = minSpeedModifier;
    }

    // Update is called once per frame
    void Update()
    {
        // Apply camera translations via right click and drag of mouse
        
        // Get current mouse position when button is pressed down
        if (Input.GetButtonDown("Fire2"))
        {
            currentMousePos = Input.mousePosition;
        }

        // when mouse button 1 (Right Click) is held down, continually get position
        // and translate camera based on difference(current-previous) in position
        if (Input.GetMouseButton(1))
        {
            previousMousePos = currentMousePos;
            currentMousePos = Input.mousePosition;

            //Debug.Log("right click down");
            //Debug.Log("mouse position: " + Input.mousePosition);
            //Debug.Log("Diff = " + (currentMousePos-previousMousePos));

            transform.Translate(-1 * (currentMousePos - previousMousePos)/speedModifier);
        }

        // Apply camera zoom
        float zoom = Input.mouseScrollDelta.y;

        // when scroll input detected, calculate zoom
        if (zoom != 0.0f)
        {
            cam.orthographicSize += -zoom;
            // speedModifier changes based on zoom level
            speedModifier += 1* (int)zoom;

            if (speedModifier > maxSpeedModifier)
            {
                speedModifier = maxSpeedModifier;
            }
            else if (speedModifier < minSpeedModifier)
            {
                speedModifier = minSpeedModifier;
            }

            // stop camera from zooming in/out too far
            if (cam.orthographicSize > maxCameraSize)
            {
                cam.orthographicSize = maxCameraSize;
                speedModifier = minSpeedModifier;
            }
            else if (cam.orthographicSize < minCameraSize)
            {
                cam.orthographicSize = minCameraSize;
                speedModifier = maxSpeedModifier;
            }
            //Debug.Log(speedModifier);
        }
    }
}
