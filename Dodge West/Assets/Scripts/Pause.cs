using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public GameObject pausePanel;
    public Button resumeButton;

    private bool isPaused = false;

    // Start is called before the first frame update
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
        // pause audio

        Time.timeScale = 0.0f; // pauses game
        isPaused = true;
    }

    public void UnPauseGame()
    {
        pausePanel.SetActive(false);

        // enable player controls
        // unpause audio
        Time.timeScale = 1.0f; // unpauses game
        isPaused = false;
    }
}
