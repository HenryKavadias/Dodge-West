using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Controller for the players ability to pause the game
public class OLPauseControls : PauseControls
{
    protected override void Update()
    {
        if (GetComponent<OLCameraManager>().CheckForPhotonView() && pausing)
        {
            // Re-enable Cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Tells the game controler/manager to pause the game
            GameObject gm = GameObject.FindGameObjectWithTag("GameController");
            if (gm != null && gm.GetComponent<Pause>().isActiveAndEnabled)
            {
                gm.GetComponent<Pause>().TogglePauseState();
            }

            pausing = false;
            return;
        }
    }
}
