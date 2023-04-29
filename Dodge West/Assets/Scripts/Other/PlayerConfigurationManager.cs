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
        Debug.Log("player joined " + pi.playerIndex);

        if (!playerConfigs.Any(p => p.PlayerIndex == pi.playerIndex))
        {
            pi.transform.SetParent(transform);
            playerConfigs.Add(new PlayerConfiguration(pi));
        }
    }

    // needs work
    public void HandlePlayerLeave(PlayerInput pi)
    {
        Debug.Log("player left " + pi.playerIndex);
        if (playerConfigs.Any(p => p.PlayerIndex == pi.playerIndex))
        {
            // Still need to figure out how to remove a player config
            // from list and scene without breaking things

            Destroy(transform.GetChild(pi.playerIndex).gameObject);
            var player = playerConfigs.Find(p => p.PlayerIndex == pi.playerIndex);
            //playerConfigs.RemoveAt(pi.playerIndex);
            playerConfigs.Remove(player);
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
        //playerConfigs[index].isReady = true;
        playerConfigs.Find(p => p.PlayerIndex == index).isReady = true;
        //Debug.Log(playerConfigs.Count);

        if (playerConfigs.Count <= maxPlayers && playerConfigs.Count >= minPlayers)
        {
            //Debug.Log(playerConfigs.All(p => p.isReady == true));

            foreach (var config in playerConfigs)
            {
                Debug.Log(config.isReady + " " + config.PlayerIndex);
            }

            if (playerConfigs.All(p => p.isReady == true))
            {
                SceneManager.LoadScene(nextScene);
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
    public int PlayerIndex { get; private set; }
    public bool isReady { get; set; }
    public Material playerMaterial { get; set; } // might not be necessary for later builds
}