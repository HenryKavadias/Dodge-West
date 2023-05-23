using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// doesn't work, need to find a different method to make a global string list of scene names
//public static string[] scenes = { "Prototype_1", "Multiplayer-TestScene", "LocalMultiplayerSetup" };

public class PlayerConfigurationManager : MonoBehaviour
{
    // String name must be accurate
    [SerializeField]
    private string nextScene = "Multiplayer-TestScene";
    [SerializeField]
    private string previousScene = "Start-Scene";

    private List<PlayerConfiguration> playerConfigs;
    //[SerializeField]
    //[Range(2, 4)]
    private int maxPlayers = 4;
    private int minPlayers = 2;

    public static PlayerConfigurationManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("[Singleton] Trying to instantiate a seccond instance of a singleton class.");
        }
        else
        {
            // Gets the player limit from the PIM component to keep consistancy with it
            maxPlayers = GetComponent<PlayerInputManager>().maxPlayerCount;
            
            Instance = this;
            DontDestroyOnLoad(Instance);
            playerConfigs = new List<PlayerConfiguration>();
        }

    }

    // Todo?: check if the system semi allows for adding more than 4 players,
    // if greater than the limit, remove/block any extras
    public void HandlePlayerJoin(PlayerInput pi)
    {

        //Debug.Log("player joined " + pi.playerIndex);
        //pi.transform.SetParent(transform);

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

    // needs work
    public void HandlePlayerLeave(PlayerInput pi)
    {
        //Debug.Log("player left " + pi.playerIndex);
        if (playerConfigs.Any(p => p.PlayerIndex == pi.playerIndex))
        {
            Debug.Log("player left " + pi.playerIndex);

            // this line of code is useless, player config object destorys itself with "PlayerLeaves()"
            //Destroy(transform.GetChild(pi.playerIndex).gameObject);
            var player = playerConfigs.Find(p => p.PlayerIndex == pi.playerIndex);
            playerConfigs.Remove(player);
        }

        // if all players backed out load previous scene and destroy player config manager
        if (!playerConfigs.Any())
        {
            SceneManager.LoadScene(previousScene);

            Destroy(gameObject);
        }
    }

    public List<PlayerConfiguration> GetPlayerConfigs()
    {
        return playerConfigs;
    }

    // might not be necessary for later builds
    public void SetPlayerColor(int index, Material color)
    {
        //playerConfigs[index].playerMaterial = color;
        playerConfigs.Find(p => p.PlayerIndex == index).playerMaterial = color;
    }

    // may need work
    public void ReadyPlayer(int index)
    {
        playerConfigs.Find(p => p.PlayerIndex == index).isReady = true;

        //playerConfigs[index].isReady = true;
        //if (playerConfigs.Count == minPlayers)
        if (playerConfigs.Count <= maxPlayers && playerConfigs.Count >= minPlayers)
        {
            //Debug.Log(playerConfigs.All(p => p.isReady == true));

            //foreach (var config in playerConfigs)
            //{
            //    Debug.Log(config.isReady + " " + config.PlayerIndex);
            //}

            if (playerConfigs.All(p => p.isReady == true))
            {
                // This line of code is required to prevent a bug with the player,
                // input manager causing it to respawn the existing players when
                // instantiated in the Game controller script in the next scene.

                // Also prevents adding extra unwanted player configurations into the next scene
                gameObject.GetComponent<PlayerInputManager>().enabled = false;

                SceneManager.LoadScene(nextScene);
                //Debug.Log("Works!!! " + playerConfigs.Count);
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

public class PlayerConfiguration
{
    public PlayerConfiguration(PlayerInput pi)
    {
        PlayerIndex = pi.playerIndex;
        Input = pi;
    }

    public PlayerInput Input { get; private set; }
    //public int PlayerIndex { get; set; } // was Set Private, changed for error prevention
    public int PlayerIndex { get; private set; }
    public bool isReady { get; set; }
    public Material playerMaterial { get; set; } // might not be necessary for later builds
}