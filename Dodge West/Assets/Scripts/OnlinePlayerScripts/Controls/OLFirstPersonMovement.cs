using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

// Manages basic movement for the player
// (camera controls are done in the camera controls script)

// Note: movement is done with physics manipulation of the
// players rigidbody, NOT manipulation of the players transform
public class OLFirstPersonMovement : FirstPersonMovement
{    
    public override void OnCrouch(InputAction.CallbackContext context)
    {
        // Button Hold system
        //crouching = context.action.triggered;

        // Toggle system
        if (GetComponent<OLCameraManager>().CheckForPhotonView())
        {
            if (crouching)
            {
                crouching = false;
            }
            else
            {
                crouching = true;
            }
        }
    }
    // Used for player inputs
    protected override void Update()
    {
        if (GetComponent<OLCameraManager>().CheckForPhotonView())
        {
            // Checks if the player is touching the ground
            isGrounded = SphereCastCheck();

            PlayerInput();
            SpeedControl();
            StateHandler();

            // Handles drag
            if (state == MovementState.walking ||
                state == MovementState.sprinting ||
                state == MovementState.crouching)
            {
                rb.drag = groundDrag;
            }
            else
            {
                rb.drag = 0f;
            }
        }
    }

    // Used for player movement (based on physics applied to the rigidbody)
    protected override void FixedUpdate()
    {
        if (GetComponent<OLCameraManager>().CheckForPhotonView())
        {
            MovePlayer();
        }
    }
}
