using Fusion;
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
    // Needed for online version of crouching
    bool allowCrouch = true;
    public void RefreshInput(
        Vector2 input, 
        bool jump, 
        bool crouch, 
        bool sprint)
    {
        movementInput = input;
        jumped = jump;

        sprinting = sprint;

        // Online controls for crouching needed
        // to be more sophisticated
        if (!allowCrouch && crouch)
        {
            return;
        }
        else
        {
            allowCrouch = true;
        }

        if (crouch && allowCrouch)
        {
            ToggleCrouch();
            allowCrouch = false;
        }
    }

    public void SetupFPM()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        // Used for crouching
        startYScale = transform.localScale.y;

        // Get player character height and set the ground check size
        if (playerModel)
        {
            currentHeight = playerModel.transform.localScale.y * 2;
            // Note: also used for Sphere Cast
            groundCheckBoxSize = new Vector3(
                playerModel.transform.localScale.x * groundCheckBoxSizeMultiplier
                , 0.05f,
                playerModel.transform.localScale.z * groundCheckBoxSizeMultiplier);
        }
        else
        {
            Debug.Log("Error, can't get players scale");
        }

        // Disables cursor
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }
    
    protected override void Start()
    {
        //rb = GetComponent<Rigidbody>();
        //rb.freezeRotation = true;

        //readyToJump = true;

        //// Used for crouching
        //startYScale = transform.localScale.y;

        //// Get player character height and set the ground check size
        //if (playerModel)
        //{
        //    currentHeight = playerModel.transform.localScale.y * 2;
        //    // Note: also used for Sphere Cast
        //    groundCheckBoxSize = new Vector3(
        //        playerModel.transform.localScale.x * groundCheckBoxSizeMultiplier
        //        , 0.05f,
        //        playerModel.transform.localScale.z * groundCheckBoxSizeMultiplier);
        //}
        //else
        //{
        //    Debug.Log("Error, can't get players scale");
        //}

        //// Disables cursor
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    public void UpdateFPM()
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

        // Normally in fixed update
        MovePlayer();
    }

    // Used for player inputs
    protected override void Update()
    {
        //if (GetComponent<OLCameraManager>().CheckForAuthority())
        //{
        //    // Checks if the player is touching the ground
        //    isGrounded = SphereCastCheck();

        //    PlayerInput();
        //    SpeedControl();
        //    StateHandler();

        //    // Handles drag
        //    if (state == MovementState.walking ||
        //        state == MovementState.sprinting ||
        //        state == MovementState.crouching)
        //    {
        //        rb.drag = groundDrag;
        //    }
        //    else
        //    {
        //        rb.drag = 0f;
        //    }
        //}
    }

    // Used for player movement (based on physics applied to the rigidbody)
    protected override void FixedUpdate()
    {
        //if (GetComponent<OLCameraManager>().CheckForAuthority())
        //{
        //    MovePlayer();
        //}
    }
}
