using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.InputSystem;

public class OnlinePlayerController : NetworkBehaviour
{
    private OLPlayerInputHandler playerInputHandler = null;
    private OLCameraManager cameraManager = null;

    public override void Spawned()
    {
        //Debug.Log("State Authority (OnlinePlayerController): " + HasStateAuthority);

        if (HasStateAuthority)
        {
            Debug.Log("Setting up player");
            
            playerInputHandler = GetComponent<OLPlayerInputHandler>();

            playerInputHandler.GetInputs();

            playerInputHandler.SetupInputs();

            cameraManager = GetComponent<OLCameraManager>();
            //cameraManager.SetupCameraAndUI();
        }
        else
        {
            Debug.Log("Player doesn't have state Authority");
        }
        

        // Add a set camera target here

        // Note: second player can't be controlled
    }

    //private void Awake()
    //{
    //    playerInputHandler = GetComponent<OLPlayerInputHandler>();
    //}

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority == false)
        {
            //Debug.Log("SA (OnlinePC) is " + HasStateAuthority);
            
            return;
        }
        //Debug.Log("SA (OnlinePC) is " + HasStateAuthority);
        //InputSystem.Update();

        playerInputHandler.UpdateInputs();

    }
}
