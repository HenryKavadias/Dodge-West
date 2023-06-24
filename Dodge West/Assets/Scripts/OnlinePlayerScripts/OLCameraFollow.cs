using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OLCameraFollow : CameraFollow
{
    // Set the camera target
    public override void SetTarget(Transform target)
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            cameraPosition = target;
        }
    }

    // Camera position and rotation matches the targets
    protected override void Update()
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            // May change this to smoothly track player
            if (cameraPosition)
            {
                transform.position = cameraPosition.position;
                transform.rotation = cameraPosition.rotation;
            }
        }
    }
}
