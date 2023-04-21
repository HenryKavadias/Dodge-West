using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Target camera position
    Transform cameraPosition = null;

    public void SetTarget(Transform target)
    {
        cameraPosition = target;
    }

    // Camera position and rotation matches the targets
    void Update()
    {
        if (cameraPosition) 
        {
            transform.position = cameraPosition.position;
            transform.rotation = cameraPosition.rotation;
        }
    }
}
