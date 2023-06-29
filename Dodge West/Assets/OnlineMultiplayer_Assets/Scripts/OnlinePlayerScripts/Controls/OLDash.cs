using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

// Manages the controls for the dash ability for the player
public class OLDash : Dash
{
    // Only able to dash when it's off cooldown
    protected override void Update()
    {
        if (GetComponent<OLCameraManager>().CheckForAuthority() && dashing)
        {
            DoDash();
            dashing = false;
        }

        // Controls dash cooldown
        if (dashCdTimer > 0) { dashCdTimer -= Time.deltaTime; }
    }
}
