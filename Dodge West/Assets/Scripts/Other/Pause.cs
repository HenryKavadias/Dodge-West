using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UI;


[RequireComponent(typeof(GameController))]
public class Pause : MonoBehaviour
{
    public GameObject pausePanel;
    public Button resumeButton;

    public GameObject eventSystem;

    public bool isPaused { get; private set; } = false;

    void Update()
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

    void Start()
    {
        UnPauseGame();
    }

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

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        resumeButton.Select();

        // disable player controls
        GetComponent<GameController>().TogglePlayerControls(false);

        // pause audio

        Time.timeScale = 0.0f; // pauses game
        isPaused = true;
    }

    public void UnPauseGame()
    {
        pausePanel.SetActive(false);

        // enable player controls
        GetComponent<GameController>().TogglePlayerControls(true);

        // unpause audio
        Time.timeScale = 1.0f; // unpauses game
        isPaused = false;
    }
}
