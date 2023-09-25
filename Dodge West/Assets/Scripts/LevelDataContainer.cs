using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// - Tracks the sequence of scenes the player has taken to get to level select scene
// - Tracks the next scene the player is going to after level select scene
// - Records the level scene selected by the player that will be loaded 
public class LevelDataContainer : MonoBehaviour
{
    public bool skipSelect { get; private set; } = false;
    public string selectedLevel { get; private set; } = string.Empty;

    // The scene AFTER level select scene. Previous scene must tell this
    // script this before going to level select scene
    public string nextScene { get; private set; } = string.Empty;

    public List<string> previousScenes { get; private set; }

    public void SelectSkip()
    {
        if (!skipSelect)
        {
            skipSelect = true;
            return;
        }
        skipSelect = false;
    }

    // Add player to player list
    void AddScene(string scene)
    {
        previousScenes.Add(scene);
    }

    // Remove player from player list
    void RemoveSceneAt(int scene)
    {
        previousScenes.RemoveAt(scene);
        // Note: maybe try to set up a way to pop the last scene in the list
    }

    public int GetSceneListCount()
    {
        return previousScenes.Count;
    }

    // When going to next scene
    public void StoreDataToNextScene(string previous, string next = "TBD")
    {
        AddScene(previous);

        if (next != "TBD")
        {
            nextScene = next;
        }

        //DisplaySceneList();
    }

    // When setting / nulling selected level
    public void ChangeSelectedLevel(string scene = "N/A")
    {
        if (scene != "N/A")
        {
            selectedLevel = scene;
        }
        else
        {
            selectedLevel = string.Empty;
        }
    }

    // When back out to previous scene
    public void RemoveDataBackToPreviousScene(string next)
    {
        // Remove last previous scene (player should be returning to this)
        RemoveSceneAt(GetSceneListCount() - 1);

        nextScene = next;

       // DisplaySceneList();
    }    

    public string GetLastScene()
    {
        return previousScenes[GetSceneListCount() - 1];
    }

    private void DisplaySceneList()
    {
        foreach (string scene in previousScenes)
        {
            Debug.Log(scene + " | Previous");
        }

        Debug.Log(nextScene + " | Next");
        Debug.Log(selectedLevel + " | Selected");
    }

    // Reference to the LevelDataContainer (only one LevelDataContainer may be active at a time)
    public static LevelDataContainer LvlInstance {  get; private set; }

    private void Awake()
    {
        // Create instance of LevelDataContainer, store it in static variable,
        // assign object to don't destroy on load instance (destory self if another already exists)
        if (LvlInstance != null)
        {
            Debug.Log("[Singleton] Trying to instantiate a second " +
                "instance of a singleton class (level data container).");
            DestroySelf(); // removes the duplicate 
        }
        else
        {
            selectedLevel = string.Empty;

            GameData gameData = GetComponent<GameData>();
            if (gameData != null)
            {
                gameData.SetGameDataInstance();
            }

            LvlInstance = this;
            DontDestroyOnLoad(LvlInstance);
        }
    }

    public void ResetSceneVariables()
    {
        selectedLevel = string.Empty;
        //previousScene = string.Empty;
        nextScene = string.Empty;

        previousScenes = new List<string>();
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
