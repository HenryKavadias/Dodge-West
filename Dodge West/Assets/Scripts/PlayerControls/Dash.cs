using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dash : MonoBehaviour
{
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
    public MouseLook cam;
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

    public void SetPlayerCamera(Transform cam)
    {
        playerCam = cam;
    }

    void Update()
    {
        if (dashing)
        {
            DoDash();
            dashing = false;
        }

        if (dashCdTimer > 0) { dashCdTimer -= Time.deltaTime; }
    }

    private void DoDash()
    {
        if (dashCdTimer > 0) { return; }
        else { dashCdTimer = dashCd; }

        fpm.dashing = true;
        fpm.maxYSpeed = maxDashYSpeed;

        if (camEffects)
        {
            cam.DoFov(dashFov);
        }

        Transform forwardT;

        if (useCameraForward)
        { 
            forwardT = playerCam; 
        }
        else
        { 
            forwardT = orientation; 
        }

        Vector3 direction = GetDirection(forwardT);

        Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;

        if (disableGravity)
        {
            rb.useGravity = false;
        }

        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayedDashForce), dashDuration);

        Invoke(nameof(ResetDash), dashDuration);
    }

    private Vector3 delayedForceToApply;

    private void DelayedDashForce()
    {
        if (resetVel) { rb.velocity = Vector3.zero; }
        
        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

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
