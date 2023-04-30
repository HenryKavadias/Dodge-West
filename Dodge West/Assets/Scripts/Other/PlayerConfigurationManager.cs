using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerConfigurationManager : MonoBehaviour
{
    private string nextScene = "Multiplayer-TestScene";

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
            Instance = this;
            DontDestroyOnLoad(Instance);
            playerConfigs = new List<PlayerConfiguration>();
        }

    }

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

    public List<PlayerConfiguration> GetPlayerConfigs()
    {
        return playerConfigs;
    }

    // might not be necessary for later builds
    public void SetPlayerColor(int index, Material color)
    {
        playerConfigs[index].playerMaterial = color;
    }

    public void ReadyPlayer(int index)
    {
        //playerConfigs[index].isReady = true;
        //if (playerConfigs.Count == minPlayers && playerConfigs.All(p => p.isReady == true))
        //{
        //    SceneManager.LoadScene(nextScene);
        //}

        playerConfigs[index].isReady = true;
        if (playerConfigs.Count <= maxPlayers && playerConfigs.Count >= minPlayers)
        //if (playerConfigs.Count == minPlayers)
        {
            if (playerConfigs.All(p => p.isReady == true))
            {
                // This is required to prevent a bug with the player input manager,
                // causing it to respawn the existing players when instantiated in
                // the Game controller script in the next scene
                gameObject.GetComponent<PlayerInputManager>().enabled = false;
                
                SceneManager.LoadScene(nextScene);
                //Debug.Log("Works!!! " + playerConfigs.Count);
            }
        }
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
    public int PlayerIndex { get; private set; }
    public bool isReady { get; set; }
    public Material playerMaterial { get; set; } // might not be necessary for later builds
}