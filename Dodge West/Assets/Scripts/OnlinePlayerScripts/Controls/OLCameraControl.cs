using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using DG.Tweening;

// Manages the camera controls for the player
public class OLCameraControl : CameraControl
{
    protected override void Update()
    {
        if (GetComponent<OLCameraManager>().CheckForPhotonView())
        {
            LookControls();
        }
    }

    // Modifies the field of view for the camera
    public override void DoFov(float endValue)
    {
        if (GetComponent<OLCameraManager>().CheckForPhotonView())
        {
            Camera cam = gameObject.GetComponent<CameraManager>().currentCam.GetComponent<Camera>();
            if (cam)
            {
                // Instant way
                //cam.fieldOfView = endValue;

                cam.DOFieldOfView(endValue, 0.40f);
            }
        }
    }
}
