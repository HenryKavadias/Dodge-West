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
    public GameObject transitionHandler;
    
    [SerializeField]
    private string tutorialScene = "Tutorial-Scene";

    [SerializeField]
    private string localMultiplayerScene = "LocalMultiplayerSetup";

    [SerializeField]
    private GameObject startPanel;
    [SerializeField]
    private GameObject controlsPanel;
    [SerializeField]
    private GameObject creditsPanel;
    [SerializeField]
    private GameObject backButton;

    [SerializeField]
    private Button[] menuButtons;

    private float ignoreInputTime = 1.5f;
    private bool inputEnabled;

    private void Start()
    {
        // Begining setup
        startPanel.SetActive(true);
        controlsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        backButton.SetActive(false);
        menuButtons[0].Select();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > ignoreInputTime)
        {
            inputEnabled = true;
        }
    }

    public void SelectTutorial()
    {
        if (!inputEnabled) { return; }

        transitionHandler.GetComponent<SceneTransition>().LoadNextScene(tutorialScene);

        //SceneManager.LoadScene(tutorialScene);
    }

    public void SelectLocalMultiplayer()
    {
        if (!inputEnabled) { return; }

        transitionHandler.GetComponent<SceneTransition>().LoadNextScene(localMultiplayerScene);
        //SceneManager.LoadScene(localMultiplayerScene);
    }

    public void SelectControls()
    {
        if (!inputEnabled) { return; }

        // Show Control Panel

        startPanel.SetActive(false);
        controlsPanel.SetActive(true);
        backButton.SetActive(true);
        backButton.GetComponent<Button>().Select();
    }

    public void SelectCredits()
    {
        if (!inputEnabled) { return; }

        // Show Credits Panel

        startPanel.SetActive(false);
        creditsPanel.SetActive(true);
        backButton.SetActive(true);
        backButton.GetComponent<Button>().Select();
    }

    // Returns user to start screen
    public void BackToStartMenu()
    {
        if (!inputEnabled) { return; }

        if (backButton.activeSelf)
        {
            if (controlsPanel.activeSelf)
            {
                startPanel.SetActive(true);
                controlsPanel.SetActive(false);
                backButton.SetActive(false);
                menuButtons[2].Select(); // Controls button
            }
            else if (creditsPanel.activeSelf)
            {
                startPanel.SetActive(true);
                creditsPanel.SetActive(false);
                backButton.SetActive(false);
                menuButtons[3].Select(); // Credits button
            }
            else
            {
                backButton.SetActive(true);
                menuButtons[0].Select();
            }
        }
    }

    public void SelectQuit()
    {
        if (!inputEnabled) { return; }

        Application.Quit();
    }
}
