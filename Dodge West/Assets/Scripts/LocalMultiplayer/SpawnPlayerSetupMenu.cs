using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;

public class SpawnPlayerSetupMenu : MonoBehaviour
{
    
    public GameObject playerSetupMenuPrefab;

    private GameObject rootMenu;
    public PlayerInput input;

    private GameObject spawnedMenu;

    private void Awake()
    {
        // Ensure the parent object in the scene has the same exact name
        rootMenu = GameObject.Find("MainLayout"); 
        if (rootMenu != null)
        {
            // Spawns the player setup menu under a parent object and sets its variables
            spawnedMenu = Instantiate(playerSetupMenuPrefab, rootMenu.transform);
            input.uiInputModule = spawnedMenu.GetComponentInChildren<InputSystemUIInputModule>();
            spawnedMenu.GetComponent<PlayerSetupMenuController>().SetPlayerIndex(input.playerIndex);
            spawnedMenu.GetComponent<PlayerSetupMenuController>().SetSpawnerReference(this);
        }

    }

    // Controls when player leaves/backs out of local multiplayer setup scene
    public void PlayerLeaves()
    {
        if (rootMenu != null)
        {
            Destroy(spawnedMenu);
            Destroy(gameObject); 
        }
    }
}
