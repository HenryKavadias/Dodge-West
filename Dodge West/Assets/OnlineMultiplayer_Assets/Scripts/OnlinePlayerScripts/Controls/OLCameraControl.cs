using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using DG.Tweening;

// Manages the camera controls for the player
public class OLCameraControl : CameraControl
{
    public void SetupCameraControl()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    protected override void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    public void UpdateCameraC()
    {
        LookControls();
    }
    
    protected override void Update()
    {
        //LookControls();
    }

    // Modifies the field of view for the camera
    public override void DoFov(float endValue)
    {
        Camera cam = gameObject.GetComponent<OLCameraManager>().currentCam.GetComponent<Camera>();
        if (cam)
        {
            // Instant way
            //cam.fieldOfView = endValue;

            cam.DOFieldOfView(endValue, 0.40f);
        }
    }
}
