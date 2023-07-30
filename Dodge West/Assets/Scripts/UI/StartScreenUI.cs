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
    public GameObject transitionHandler;    // Reference for scene transition handler
    
    // Scene names the player can go to (MUST be accurate)
    [SerializeField]
    private string[] nextScenes = { "Tutorial-Scene", "LevelSelect-Scene", "LocalMultiplayerSetup" };

    // UI elements for start scene
    [SerializeField]
    private GameObject startPanel;
    //[SerializeField]
    //private GameObject levelSelectPanel;
    [SerializeField]
    private GameObject controlsPanel;
    [SerializeField]
    private GameObject creditsPanel;

    [SerializeField]
    private GameObject[] backButtons;
    // Buttons for start scenes
    [SerializeField]
    private Button[] menuButtons;

    // Input delay duration variables
    private float ignoreInputTime = 1.0f;
    private bool inputEnabled;

    private LevelDataContainer dataContainer = null;

    // Set beginning state of start menu UI
    private void Start()
    {
        GameObject data = GameObject.FindGameObjectWithTag("DataContainer");

        if (data)
        {
            dataContainer = data.GetComponent<LevelDataContainer>();
            dataContainer.ResetSceneVariables();
        }

        startPanel.SetActive(true);
        controlsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        menuButtons[0].Select();
    }

    public GameObject eventSystem;  // Reference to event system

    // Delay player input after scene loads
    void Update()
    {
        if (Time.time > ignoreInputTime)
        {
            inputEnabled = true;
        }

        // Back button
        if (eventSystem.GetComponent<InputSystemUIInputModule>().
            cancel.action.triggered)
        {
            BackToStartMenu();
        }
    }

    // Go to single player tutorial scene
    public void SelectTutorial()
    {
        if (!inputEnabled) { return; }

        transitionHandler.GetComponent<SceneTransition>().LoadNextScene(nextScenes[0]);
    }

    // Go to local multiplayer setup scene
    public void SelectLocalMultiplayer()
    {
        if (!inputEnabled) { return; }

        if (dataContainer)
        {
            dataContainer.StoreDataToNextScene(SceneManager.GetActiveScene().name, nextScenes[2]);
        }

        transitionHandler.GetComponent<SceneTransition>().LoadNextScene(nextScenes[1]);
    }

    // Show Control Panel
    public void SelectControls()
    {
        if (!inputEnabled) { return; }

        startPanel.SetActive(false);
        controlsPanel.SetActive(true);
        backButtons[0].GetComponent<Button>().Select();
    }

    // Show Credits Panel
    public void SelectCredits()
    {
        if (!inputEnabled) { return; }

        startPanel.SetActive(false);
        creditsPanel.SetActive(true);
        backButtons[1].GetComponent<Button>().Select();
    }

    // Returns user to start screen
    public void BackToStartMenu()
    {
        // Note: add for escape button

        if (!inputEnabled) { return; }
        
        if (controlsPanel.activeSelf)
        {
            startPanel.SetActive(true);
            controlsPanel.SetActive(false);
            menuButtons[2].Select(); // Controls button
        }
        else if (creditsPanel.activeSelf)
        {
            startPanel.SetActive(true);
            creditsPanel.SetActive(false);
            menuButtons[3].Select(); // Credits button
        }
        else
        {
            menuButtons[0].Select(); // First button in menu
        }
    }

    // Quits out of the game
    public void SelectQuit()
    {
        if (!inputEnabled) { return; }

        Application.Quit();
    }
}
