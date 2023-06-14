using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

// Manages the controls for the dash ability for the player
public class Dash : MonoBehaviour
{
    // Dash script variabels
    [Header("References")]
    public Transform orientation;
    private Transform playerCam;
    private Rigidbody rb;
    private FirstPersonMovement fpm;

    [Header("Dashing")]
    public float dashForce;
    public float dashUpwardForce;
    public float maxDashYSpeed;
    public float dashDuration;

    [Header("CameraEffects")]
    public CameraControl cam;
    public float dashFov;
    public float defaultFov = 60f;

    [Header("Settings")]
    public bool useCameraForward = true;
    public bool allowAllDirections = true;
    public bool disableGravity = false;
    public bool resetVel = true;
    public bool camEffects = true;

    [Header("Cooldown")]
    public float dashCd;
    private float dashCdTimer;

    private bool dashing = false;

    // Gets input values from dash control inputs
    public void OnDash(InputAction.CallbackContext context)
    {
        dashing = context.action.triggered;
        // reset after activation
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        fpm = GetComponent<FirstPersonMovement>();
    }

    // Set camera reference
    public void SetPlayerCamera(Transform cam)
    {
        playerCam = cam;
    }

    // Only able to dash when it's off cooldown
    void Update()
    {
        if (dashing)
        {
            DoDash();
            dashing = false;
        }

        // Controls dash cooldown
        if (dashCdTimer > 0) { dashCdTimer -= Time.deltaTime; }
    }

    // Manages the dash ability behaviour
    private void DoDash()
    {
        // Checks dash cooldown
        if (dashCdTimer > 0) { return; }
        else { dashCdTimer = dashCd; }

        fpm.dashing = true;
        fpm.maxYSpeed = maxDashYSpeed;

        // Controls the camera effect when dodging
        if (camEffects)
        {
            cam.DoFov(dashFov);
        }

        Transform forwardT;

        // Uses either the player camera direction or the physical player orientation
        if (useCameraForward)
        { 
            forwardT = playerCam; 
        }
        else
        { 
            forwardT = orientation; 
        }

        // Get the direction of the dash, then apply the force
        Vector3 direction = GetDirection(forwardT);
        Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;

        // Controls if the gravity is enabled
        if (disableGravity)
        {
            rb.useGravity = false;
        }

        // Applies the force and invokes the required functions under a delay
        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayedDashForce), dashDuration);
        Invoke(nameof(ResetDash), dashDuration);
    }

    // Holds force to apply to player character for next dash
    private Vector3 delayedForceToApply;

    // Applies an impulse dash force to player character 
    private void DelayedDashForce()
    {
        if (resetVel) { rb.velocity = Vector3.zero; }
        
        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    // Resets the Dash ability (not cooldown)
    private void ResetDash()
    {
        fpm.dashing = false;
        fpm.maxYSpeed = 0;

        if (camEffects)
        {
            cam.DoFov(defaultFov);
        }

        if (disableGravity) { rb.useGravity = true; }
    }

    // Controls and gets the direction for the dash ability
    private Vector3 GetDirection(Transform forwardT)
    {
        Vector2 moveInput = gameObject.GetComponent<FirstPersonMovement>().currentMoveInput;

        float horizontalInput = moveInput.x;
        float verticalInput = moveInput.y;

        Vector3 direction = new Vector3();
        
        if (allowAllDirections)
        {
            direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;
        }
        else
        {
            direction = forwardT.forward;
        }

        if (verticalInput == 0 && horizontalInput == 0)
        {
            direction = forwardT.forward;
        }

        return direction.normalized;
    }
}
