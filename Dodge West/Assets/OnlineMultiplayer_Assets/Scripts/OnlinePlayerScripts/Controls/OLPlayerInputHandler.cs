using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using Fusion;

//public struct NetworkInputData : INetworkInput
//{
//    public bool pausing;
//    public bool pickedup;
//    public bool thrown;
//    public bool loadedItem;
//    public bool dashing;

//    public Vector2 movementInput;
//    public bool jumped;
//    public bool sprinting;
//    public bool crouching;

//    public Vector2 lookInput;
//}

public class OLPlayerInputHandler : PlayerInputHandler
{
    private OLFirstPersonMovement olMovement;
    private OLCameraControl olLook;
    private OLPhysicsPickup olPickup;
    private OLDash olDash;
    private OLPauseControls olPause;

    // Get references to all scripts that have the player controls
    protected override void Awake()
    {
        olMovement = GetComponent<OLFirstPersonMovement>();
        olLook = GetComponent<OLCameraControl>();
        olPickup = GetComponent<OLPhysicsPickup>();
        olDash = GetComponent<OLDash>();
        olPause = GetComponent<OLPauseControls>();

        controls = new PlayerControls();
    }

    public void UpdateInputs()
    {
        olMovement.UpdateFPM();
    }

    public OLFirstPersonMovement GetMovement()
    {
        return olMovement;
    }

    public OLCameraControl GetLook()
    {
        return olLook;
    }

    public OLPhysicsPickup GetPickup()
    {
        return olPickup;
    }

    public OLDash GetDash()
    {
        return olDash;
    }

    public OLPauseControls GetPauseControls()
    {
        return olPause;
    }

    //protected override void OnEnable()
    //{
    //    if (GetComponent<OLCameraManager>().CheckForAuthority())
    //    {
    //        controls.Enable();
    //    }
    //}

    //protected override void OnDisable()
    //{
    //    if (GetComponent<OLCameraManager>().CheckForAuthority())
    //    {
    //        controls.Disable();
    //    }
    //}
}
