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
    private string tutorialScene = "Tutorial-Scene";
    [SerializeField]
    private string localMultiplayerScene = "LocalMultiplayerSetup";

    [SerializeField]
    private string[] levelSceneNames;

    // UI elements for start scene
    [SerializeField]
    private GameObject startPanel;
    [SerializeField]
    private GameObject levelSelectPanel;
    [SerializeField]
    private GameObject controlsPanel;
    [SerializeField]
    private GameObject creditsPanel;

    [SerializeField]
    private GameObject[] backButtons;

    [SerializeField]
    private GameObject levelSelectButtonOne;

    // Buttons for start scenes
    [SerializeField]
    private Button[] menuButtons;

    // Input delay duration variables
    private float ignoreInputTime = 1.0f;
    private bool inputEnabled;

    // Set beginning state of start menu UI
    private void Start()
    {
        startPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
        controlsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        menuButtons[0].Select();
    }

    // Delay player input after scene loads
    void Update()
    {
        if (Time.time > ignoreInputTime)
        {
            inputEnabled = true;
        }
    }

    // Go to single player tutorial scene
    public void SelectTutorial()
    {
        if (!inputEnabled) { return; }

        transitionHandler.GetComponent<SceneTransition>().LoadNextScene(tutorialScene);
    }

    // Go to local multiplayer setup scene
    public void SelectLocalMultiplayer()
    {
        if (!inputEnabled) { return; }

        //transitionHandler.GetComponent<SceneTransition>().LoadNextScene(localMultiplayerScene);
        startPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
        levelSelectButtonOne.GetComponent<Button>().Select();
    }

    // Select a level to play
    public void SelectLevel(int scene)
    {
        switch (scene)
        {
            case 0:
                StoreLevelData(levelSceneNames[0]);
                break; 
            case 1:
                StoreLevelData(levelSceneNames[1]);
                break;
            case 2:
                StoreLevelData(levelSceneNames[2]);
                break;
            default: 
                break;
        }
    }

    void StoreLevelData(string scene)
    {
        GameObject data = GameObject.FindGameObjectWithTag("DataContainer");

        if (data)
        {
            data.GetComponent<LevelDataContainer>().selectedLevel = scene;

            transitionHandler.GetComponent<SceneTransition>().LoadNextScene(localMultiplayerScene);
        }
        else
        {
            Debug.Log("Error, data container is missing!!!");
        }
    }    

    // Show Control Panel
    public void SelectControls()
    {
        if (!inputEnabled) { return; }

        startPanel.SetActive(false);
        controlsPanel.SetActive(true);
        backButtons[1].GetComponent<Button>().Select();
    }

    // Show Credits Panel
    public void SelectCredits()
    {
        if (!inputEnabled) { return; }

        startPanel.SetActive(false);
        creditsPanel.SetActive(true);
        backButtons[2].GetComponent<Button>().Select();
    }

    // Returns user to start screen
    public void BackToStartMenu()
    {
        if (!inputEnabled) { return; }

        if (levelSelectPanel.activeSelf)
        {
            startPanel.SetActive(true);
            levelSelectPanel.SetActive(false);
            menuButtons[1].Select(); // Local multiplayer button
        }
        else if (controlsPanel.activeSelf)
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
