using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// - Tracks the sequence of scenes the player has taken to get to level select scene
// - Tracks the next scene the player is going to after level select scene
// - Records the level scene selected by the player that will be loaded 
public class LevelDataContainer : MonoBehaviour
{
    public string selectedLevel { get; set; } = string.Empty;
    //public string previousScene { get; set; } = string.Empty;

    // The scene AFTER level select scene. Previous scene must tell this
    // script this before going to level select scene
    public string nextScene { get; set; } = string.Empty;

    public List<string> previousScenes;

    // Add player to player list
    public void AddScene(string scene)
    {
        previousScenes.Add(scene);
    }

    // Remove player from player list
    public void RemoveScene(string scene)
    {
        previousScenes.Remove(scene);
        // Note: maybe try to set up a way to pop the last scene in the list
    }

    public int GetSceneListCount()
    {
        return previousScenes.Count;
    }

    //public List<string> GetSceneList()
    //{
    //    return previousScenes;
    //}

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

            LvlInstance = this;
            DontDestroyOnLoad(LvlInstance);
        }
    }

    public void ResetSceneVariables()
    {
        selectedLevel = string.Empty;
        //previousScene = string.Empty;
        nextScene = string.Empty;

        previousScenes.Clear();
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
