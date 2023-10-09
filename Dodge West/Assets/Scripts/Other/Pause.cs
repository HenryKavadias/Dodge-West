using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UI;

// Controls the pause state of the game (must be attached to game manager/controller)
[RequireComponent(typeof(GameController))]
public class Pause : MonoBehaviour
{
    public GameObject pausePanel;   // Reference to pause panle (holds pause UI)
    public Button resumeButton;     // Reference to resume button (used for pause UI)

    public GameObject eventSystem;  // Reference to event system

    public bool inTutorial { private get; set; } = false;

    public bool isPaused { get; private set; } = false;

    private float ignoreInputTime = 1.5f;
    private bool inputEnabled = false;

    void Update()
    {
        if (Time.time > ignoreInputTime)
        {
            inputEnabled = true;
        }

        if (inputEnabled && !inTutorial)
        {
            // Allows for unpausing with the cancel button
            // when character controls are disabled
            if (isPaused &&
                eventSystem.GetComponent<InputSystemUIInputModule>().
                cancel.action.triggered)
            {
                TogglePauseState();
            }
        }
    }

    // Unpause game when it states
    void Start()
    {
        UnPauseGame();
    }

    // Toggles the pause state of the game
    public void TogglePauseState()
    {
        if (isPaused)
        {
            UnPauseGame();
        }
        else
        {
            PauseGame();
        }
    }

    // Pause game 
    public void PauseGame()
    {
        pausePanel.SetActive(true);
        resumeButton.Select();

        // Disable player controls
        GetComponent<GameController>().TogglePlayerControls(false);

        // Pause audio
        if (GetComponent<MusicContoller>())
        {
            GetComponent<MusicContoller>().PauseMusic();
        }

        Time.timeScale = 0.0f; // Pauses game
        isPaused = true;
    }

    // Unpuase game
    public void UnPauseGame()
    {
        pausePanel.SetActive(false);

        // Enable player controls
        GetComponent<GameController>().TogglePlayerControls(true);

        // Unpause audio
        if (GetComponent<MusicContoller>())
        {
            GetComponent<MusicContoller>().PauseMusic(false);
        }

        Time.timeScale = 1.0f; // Unpauses game
        isPaused = false;
    }
}
