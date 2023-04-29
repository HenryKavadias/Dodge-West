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
            //var menu = Instantiate(playerSetupMenuPrefab, rootMenu.transform);
            //input.uiInputModule = menu.GetComponentInChildren<InputSystemUIInputModule>();
            //menu.GetComponent<PlayerSetupMenuController>().SetPlayerIndex(input.playerIndex);

            spawnedMenu = Instantiate(playerSetupMenuPrefab, rootMenu.transform);
            input.uiInputModule = spawnedMenu.GetComponentInChildren<InputSystemUIInputModule>();
            spawnedMenu.GetComponent<PlayerSetupMenuController>().SetPlayerIndex(input.playerIndex);
            spawnedMenu.GetComponent<PlayerSetupMenuController>().SetSpawnerReference(this);
        }

    }

    // needs work
    public void PlayerLeaves()
    {
        if (rootMenu != null)
        {
            // This needs fixing
            Destroy(spawnedMenu);
            Destroy(gameObject);
        }
    }
}
