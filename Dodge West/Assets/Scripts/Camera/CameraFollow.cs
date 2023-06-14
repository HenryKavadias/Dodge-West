using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Target camera position
    Transform cameraPosition = null;

    // Set the camera target
    public void SetTarget(Transform target)
    {
        cameraPosition = target;
    }

    // Camera position and rotation matches the targets
    void Update()
    {
        // May change this to smoothly track player
        if (cameraPosition) 
        {
            transform.position = cameraPosition.position;
            transform.rotation = cameraPosition.rotation;
        }
    }
}
