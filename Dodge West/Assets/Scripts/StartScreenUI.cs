using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StartScreenUI : MonoBehaviour
{
    [SerializeField]
    private string LocalMultiplayerScene = "LocalMultiplayerSetup";

    private float ignoreInputTime = 1.5f;
    private bool inputEnabled;

    // Update is called once per frame
    void Update()
    {
        if (Time.time > ignoreInputTime)
        {
            inputEnabled = true;
        }
    }

    public void SelectLocalMultiplayer()
    {
        if (!inputEnabled) { return; }

        SceneManager.LoadScene(LocalMultiplayerScene);
    }

    public void SelectControls()
    {
        if (!inputEnabled) { return; }

        // Show Control Panel
    }

    public void SelectCredits()
    {
        if (!inputEnabled) { return; }

        // Show Credits Panel
    }

    public void SelectQuit()
    {
        if (!inputEnabled) { return; }

        Application.Quit();
    }
}
