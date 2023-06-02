using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseControls : MonoBehaviour
{
    private bool pausing = false;
    //public bool isPaused { get; private set; } = false;

    public void OnPause(InputAction.CallbackContext context)
    {
        pausing = context.action.triggered;
    }


    // Update is called once per frame
    void Update()
    {
        if (pausing)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // do the stuff
            GameObject gm = GameObject.FindGameObjectWithTag("GameController");
            if (gm != null && gm.GetComponent<Pause>().isActiveAndEnabled)
            {
                gm.GetComponent<Pause>().TogglePauseState();
                //isPaused = gm.GetComponent<Pause>().isPaused;
            }

            pausing = false;
            return;
        }
    }
}
