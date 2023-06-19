using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelDataContainer : MonoBehaviour
{
    public string selectedLevel { get; set; }

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

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
