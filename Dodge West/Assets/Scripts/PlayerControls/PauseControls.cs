using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseControls : MonoBehaviour
{
    private bool paused = false;

    public void OnPause(InputAction.CallbackContext context)
    {
        paused = context.action.triggered;
    }


    // Update is called once per frame
    void Update()
    {
        if (paused)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // do the stuff
            GameObject gm = GameObject.FindGameObjectWithTag("GameController");
            if (gm != null && gm.GetComponent<Pause>().isActiveAndEnabled)
            {
                gm.GetComponent<Pause>().TogglePauseState();
            }

            paused = false;
            return;
        }
    }
}
