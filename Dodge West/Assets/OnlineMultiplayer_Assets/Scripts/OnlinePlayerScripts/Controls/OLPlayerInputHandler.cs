using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using Fusion;

public class OLPlayerInputHandler : PlayerInputHandler
{
    private OLFirstPersonMovement olMovement;
    private OLCameraControl olLook;
    private OLPhysicsPickup olPickup;
    private OLDash olDash;
    private OLPauseControls olPause;

    private OLCameraManager olCameraManager;

    // Get references to all scripts that have the player controls
    protected override void Awake()
    {
        //olMovement = GetComponent<OLFirstPersonMovement>();
        //olLook = GetComponent<OLCameraControl>();
        //olPickup = GetComponent<OLPhysicsPickup>();
        //olDash = GetComponent<OLDash>();
        //olPause = GetComponent<OLPauseControls>();

        //controls = new PlayerControls();
    }
    // Gets the input scipts

    protected override void OnEnable()
    {
        //controls.Enable();
    }

    protected override void OnDisable()
    {
        //controls.Disable();
    }

    public void GetInputs()
    {
        olCameraManager = GetComponent<OLCameraManager>();
        
        olMovement = GetComponent<OLFirstPersonMovement>();
        olLook = GetComponent<OLCameraControl>();
        olPickup = GetComponent<OLPhysicsPickup>();
        olDash = GetComponent<OLDash>();
        olPause = GetComponent<OLPauseControls>();

        controls = new PlayerControls();
    }    
    // Sets up the input scripts
    public void SetupInputs()
    {
        olCameraManager.SetupCameraManager();

        olMovement.SetupFPM();
        olLook.SetupCameraControl();
        olDash.SetupDash();

        // These two function calls must come last in this function

        if (playerInitialized == false)
        {
            InitializePlayer();
            GetComponent<LifeCounter>().SetLives(1);

            Debug.Log("Back up initialized triggered.");
        }
    }
    // Updates the input scripts
    public void UpdateInputs()
    {
        olMovement.UpdateFPM();

        olLook.UpdateCameraC();

        olDash.UpdateDashMove();

        olPickup.UpdatePPup();

        olPause.UpdatePC();
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
