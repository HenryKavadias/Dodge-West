using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

// Manages the controls for the dash ability for the player
public class OLDash : Dash
{
    public void SetupDash()
    {
        rb = GetComponent<Rigidbody>();
        fpm = GetComponent<FirstPersonMovement>();
    }

    public void RefreshInput(bool dash)
    {
        dashing = dash;
    }

    protected override void Start()
    {
        //rb = GetComponent<Rigidbody>();
        //fpm = GetComponent<FirstPersonMovement>();
    }
    public void UpdateDashMove()
    {
        if (dashing)
        {
            DoDash();
            dashing = false;
        }

        // Controls dash cooldown
        if (dashCdTimer > 0) { dashCdTimer -= Time.deltaTime; }
    }    
    
    // Only able to dash when it's off cooldown
    protected override void Update()
    {
        //if (dashing)
        //{
        //    DoDash();
        //    dashing = false;
        //}

        //// Controls dash cooldown
        //if (dashCdTimer > 0) { dashCdTimer -= Time.deltaTime; }
    }
}
