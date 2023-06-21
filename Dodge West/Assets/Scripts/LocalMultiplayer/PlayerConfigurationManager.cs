using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// doesn't work, need to find a different method to make a global string list of scene names
//public static string[] scenes = { "Prototype_1", "Multiplayer-TestScene", "LocalMultiplayerSetup" };

public class PlayerConfigurationManager : MonoBehaviour
{
    // Reference to transition handler
    public GameObject transitionHandler;

    // String name must be accurate for scene names
    [SerializeField]
    private string nextScene = "Multiplayer-TestScene";
    [SerializeField]
    private string previousScene = "Start-Scene";

    // UI element that covers everything when everyone is ready
    [SerializeField]
    private GameObject blankScreen;

    // List of player control configurations (1 per player controller)
    private List<PlayerConfiguration> playerConfigs;
    //[SerializeField]
    //[Range(2, 4)]
    private int maxPlayers = 4;
    private int minPlayers = 2;

    private LevelDataContainer levelData = null;

    // Reference to the configuration manager (only one player configuration manager may be active at a time)
    public static PlayerConfigurationManager Instance { get; private set; }

    private void Awake()
    {
        // Create instance of player configuration manager, store it in static variable,
        // assign object to don't destroy on load instance (destory self if another already exists)
        if (Instance != null)
        {
            Debug.Log("[Singleton] Trying to instantiate a second " +
                "instance of a singleton class (player configuration manager).");
            Destroy(gameObject); // removes the duplicate 
        }
        else
        {
            // Gets the player limit from the PIM component to keep consistancy with it
            maxPlayers = GetComponent<PlayerInputManager>().maxPlayerCount;
            
            Instance = this;
            DontDestroyOnLoad(Instance);
            playerConfigs = new List<PlayerConfiguration>();
        }

        // Set the next level
        SetNextLevel();
    }

    private void SetNextLevel()
    {
        GameObject data = GameObject.FindGameObjectWithTag("DataContainer");

        if (data && data.GetComponent<LevelDataContainer>().selectedLevel != string.Empty)
        {
            levelData = data.GetComponent<LevelDataContainer>();

            nextScene = levelData.selectedLevel;

            // resets it
            levelData.ChangeSelectedLevel();
        }
        else
        {
            Debug.Log("Error, data container is missing!!!");
        }
    }

    // Todo?: check if the system semi allows for adding more than 4 players,
    // if greater than the limit, remove/block any extras

    // Add new player configuration to list
    public void HandlePlayerJoin(PlayerInput pi)
    {
        // avoid adding duplicate, and more than the maximum players
        if (!playerConfigs.Any(p => p.PlayerIndex == pi.playerIndex))
        {
            Debug.Log("player joined " + pi.playerIndex);
            pi.transform.SetParent(transform);

            playerConfigs.Add(new PlayerConfiguration(pi));
        }
        else
        {
            Debug.Log("Can't spawn existing players: " + pi.playerIndex);
            Debug.Log("Player controls list: " + playerConfigs.Count);
        }
    }

    // Remove an existing player when they back out
    public void HandlePlayerLeave(PlayerInput pi)
    {
        //Debug.Log("player left " + pi.playerIndex);
        if (playerConfigs.Any(p => p.PlayerIndex == pi.playerIndex))
        {
            Debug.Log("player left " + pi.playerIndex);

            var player = playerConfigs.Find(p => p.PlayerIndex == pi.playerIndex);
            playerConfigs.Remove(player);
        }

        // If all players backed out load previous scene and destroy player config manager
        if (!playerConfigs.Any())
        {
            // Send back to previous scene
            if (levelData)
            {
                // Set previous scene
                previousScene = levelData.previousScenes[levelData.GetSceneListCount() - 1];

                // Remove previous scene and set next scene
                levelData.RemoveDataBackToPreviousScene(SceneManager.GetActiveScene().name);

                // Reset selected level
                levelData.ChangeSelectedLevel();
            }

            if (transitionHandler)
            {
                transitionHandler.GetComponent<SceneTransition>().LoadNextScene(previousScene);
            }
            else
            {
                
                SceneManager.LoadScene(previousScene);

                Destroy(gameObject);
            }
        }
    }

    // Return list of player configurations 
    public List<PlayerConfiguration> GetPlayerConfigs()
    {
        return playerConfigs;
    }

    // might be change for character model for later builds
    public void SetPlayerColor(int index, Material color)
    {
        // If player configuration exists, set colour
        //playerConfigs[index].playerMaterial = color;
        playerConfigs.Find(p => p.PlayerIndex == index).playerMaterial = color;
    }

    // Handles players going into ready state
    public void ReadyPlayer(int index)
    {
        // If player exists, ready them up
        playerConfigs.Find(p => p.PlayerIndex == index).isReady = true;

        // If player configuration list count is above or at the minimum and is below or at the maximum
        if (playerConfigs.Count <= maxPlayers && playerConfigs.Count >= minPlayers)
        {
            // If all players have readied up
            if (playerConfigs.All(p => p.isReady == true))
            {
                // This line of code is required to prevent a bug with the player,
                // input manager causing it to respawn the existing players when
                // instantiated in the Game controller script in the next scene.

                // Also prevents adding extra unwanted player configurations into the next scene
                gameObject.GetComponent<PlayerInputManager>().enabled = false;

                // Hides previous scene UI
                GameObject bs = Instantiate(blankScreen);

                if (levelData)
                {
                    // This might be redundant in the future as the data container object may have more uses
                    levelData.DestroySelf();
                }

                // Begin scene transition
                if (transitionHandler)
                {
                    transitionHandler.GetComponent<SceneTransition>().LoadNextScene(nextScene, true);

                    //transitionHandler = null;
                }
                else
                {
                    SceneManager.LoadScene(nextScene);
                }
            }
        }
    }

    // needs work (may need work)
    public void UnReadyPlayer(int index)
    {
        //playerConfigs[index].isReady = false;
        playerConfigs.Find(p => p.PlayerIndex == index).isReady = false;
    }
}

// Player configuration object class
public class PlayerConfiguration
{
    public PlayerConfiguration(PlayerInput pi)
    {
        PlayerIndex = pi.playerIndex;
        Input = pi;
    }

    public PlayerInput Input { get; private set; }
    public int PlayerIndex { get; private set; }
    public bool isReady { get; set; }
    public Material playerMaterial { get; set; } // might be change for models in later builds
}