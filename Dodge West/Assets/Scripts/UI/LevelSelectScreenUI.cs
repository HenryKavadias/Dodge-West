using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectScreenUI : MonoBehaviour
{
    public GameObject transitionHandler;    // Reference for scene transition handler

    // Scene names the player can go to (MUST be accurate)
    [SerializeField]
    private string previousScene = "Start-Scene";
    [SerializeField]
    private string nextScene = "LocalMultiplayerSetup";

    [SerializeField]
    private string[] levelSceneNames;

    // Buttons for start scenes
    [SerializeField]
    private Button[] menuButtons;

    // Input delay duration variables
    private float ignoreInputTime = 1.0f;
    private bool inputEnabled;

    private LevelDataContainer dataContainer = null;

    public GameObject eventSystem;  // Reference to event system

    void Start()
    {
        menuButtons[0].Select();

        GameObject data = GameObject.FindGameObjectWithTag("DataContainer");

        if (data)
        {
            dataContainer = data.GetComponent<LevelDataContainer>();
        }
    }

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
            BackToPreviousScene();
        }
    }

    // Select a level to play
    public void SelectLevel(int scene)
    {
        if (!inputEnabled) { return; }

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
        //GameObject data = GameObject.FindGameObjectWithTag("DataContainer");

        if (dataContainer)
        {
            //dataContainer.selectedLevel = scene;
            //dataContainer.AddScene(SceneManager.GetActiveScene().name);

            dataContainer.ChangeSelectedLevel(scene);

            if (dataContainer.nextScene != string.Empty)
            {
                transitionHandler.GetComponent<SceneTransition>().LoadNextScene(
                dataContainer.nextScene);
            }
            // Use defualt next scene
            else
            {
                transitionHandler.GetComponent<SceneTransition>().LoadNextScene(nextScene);
            }

            dataContainer.StoreDataToNextScene(SceneManager.GetActiveScene().name, scene);
            //dataContainer.nextScene = scene;
        }
        else
        {
            Debug.Log("Error, data container is missing!!!");
        }
    }

    // Note: add for escape button
    public void BackToPreviousScene()
    {
        if (!inputEnabled) { return; }

        if (dataContainer && dataContainer.previousScenes.Any())
        {
            // Set previous scene
            previousScene = dataContainer.GetLastScene();

            // Remove previous scene and set next scene
            dataContainer.RemoveDataBackToPreviousScene(SceneManager.GetActiveScene().name);

            transitionHandler.GetComponent<SceneTransition>().LoadNextScene(previousScene);
        }
        else
        {
            transitionHandler.GetComponent<SceneTransition>().LoadNextScene(previousScene);

            Debug.Log("Error, data container is missing, and/or their aren't any previous scenes!!!" + 
                dataContainer + " | " + dataContainer.previousScenes.Any());
        }

        // Note: might need to be change for navigation purposes
        //dataContainer.GetComponent<LevelDataContainer>().ResetSceneVariables();
    }
}
