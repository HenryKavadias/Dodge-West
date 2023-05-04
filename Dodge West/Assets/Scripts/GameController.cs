using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;

public enum GameMode
{
    SinglePlayer,
    LocalMultiplayer,
    OnlineMultiplayer
}

public class GameController : MonoBehaviour
{
    public GameMode gameMode { get; private set; } = GameMode.SinglePlayer;

    public GameObject playerObject;

    public GameObject[] spawnPosition;

    private Quaternion spawnRot = Quaternion.identity;

    // list of live Players
    private List<GameObject> livePlayers = new List<GameObject>();

    // Add player to player list
    public void AddPlayer(GameObject player)
    {
        livePlayers.Add(player);
    }

    // Remove player from player list
    public void RemovePlayer(GameObject player)
    {
        livePlayers.Remove(player);
    }

    public int LivePlayerCount()
    {
        return livePlayers.Count;
    }

    private void Start()
    {
        NewSystem();
    }
    void NewSystem()
    {
        // Single player instance if the scene is loaded directly, local multiplayer if player 
        // configuration manager exsists
        if (!PlayerConfigurationManager.Instance)
        {
            gameMode = GameMode.SinglePlayer;
            
            var player = Instantiate(
                playerObject,
                spawnPosition[0].GetComponent<Transform>().position,
                spawnRot);
            player.GetComponent<PlayerInputHandler>().InitializePlayer();

        }
        else if (PlayerConfigurationManager.Instance)
        {
            gameMode = GameMode.LocalMultiplayer;

            var playerConfigs = PlayerConfigurationManager.Instance.GetPlayerConfigs().ToArray();

            for (int i = 0; i < playerConfigs.Length; i++)
            {
                var player = Instantiate(
                    playerObject,
                    spawnPosition[i].GetComponent<Transform>().position,
                    spawnRot);
                player.GetComponent<PlayerInputHandler>().InitializePlayer(playerConfigs[i], i + 1);

                AddPlayer(player);
            }
        }
    }
}
