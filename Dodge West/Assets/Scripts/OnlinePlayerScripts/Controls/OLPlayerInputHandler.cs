using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class OLPlayerInputHandler : PlayerInputHandler
{
    // Get references to all scripts that have the player controls
    protected override void Awake()
    {
        movement = GetComponent<OLFirstPersonMovement>();
        look = GetComponent<OLCameraControl>();
        pickup = GetComponent<OLPhysicsPickup>();
        dash = GetComponent<OLDash>();
        pause = GetComponent<OLPauseControls>();

        controls = new PlayerControls();
    }

    protected override void OnEnable()
    {
        if (GetComponent<OLCameraManager>().CheckForPhotonView())
        {
            controls.Enable();
        }
    }

    protected override void OnDisable()
    {
        if (GetComponent<OLCameraManager>().CheckForPhotonView())
        {
            controls.Enable();
        }
    }
}
