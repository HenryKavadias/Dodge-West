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
    public GameMode gameMode = GameMode.SinglePlayer;

    [Range(2, 4)]
    public int localMultiplayerLimit = 2;

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

    private void Start()
    {
        NewSystem();

        //var playera = Instantiate(playerObject);

        //OldSystem();
    }
    void NewSystem()
    {
        //Instantiate(
        //            new GameObject(),
        //            spawnPosition[0].GetComponent<Transform>().position,
        //            spawnRot);

        //var playera = Instantiate(
        //        playerObject,
        //        spawnPosition[0].GetComponent<Transform>().position,
        //        spawnRot);

        //Debug.Log("Bang");

        // Comment out if loading sence from local multiplayer setup
        // Note: this code is temperary
        if (gameMode == GameMode.SinglePlayer)
        {
            var player = Instantiate(
                playerObject,
                spawnPosition[0].GetComponent<Transform>().position,
                spawnRot);
            player.GetComponent<PlayerInputHandler>().InitializePlayer();

        }
        else if (gameMode == GameMode.LocalMultiplayer)
        {
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


    void OldSystem()
    {
        if (playerObject)
        {
            GameObject initPlayer = null;

            if (gameMode == GameMode.LocalMultiplayer && localMultiplayerLimit == 2)
            {
                if (spawnPosition[0] && spawnPosition[1])
                {
                    //Debug.Log(Input.GetJoystickNames());

                    //Input.GetJoystickNames();

                    // Spawn player one
                    initPlayer = Instantiate(playerObject,
                        spawnPosition[0].GetComponent<Transform>().position, spawnRot);
                    initPlayer.GetComponent<PlayerID>().ChangePlayerNumber(1);

                    //string scheme = "Controller";

                    // temp controller scheme assignment
                    //initPlayer.GetComponent<PlayerInput>().SwitchCurrentControlScheme(scheme, Gamepad.current);

                    AddPlayer(initPlayer);

                    // Spawn player two
                    initPlayer = Instantiate(playerObject,
                        spawnPosition[1].GetComponent<Transform>().position, spawnRot);
                    initPlayer.GetComponent<PlayerID>().ChangePlayerNumber(2);

                    AddPlayer(initPlayer);

                    // temp controller scheme assignment
                    //initPlayer.GetComponent<PlayerInput>().SwitchCurrentControlScheme(scheme, Gamepad.current);
                }
            }
            else if (gameMode == GameMode.SinglePlayer)
            {
                if (spawnPosition[0])
                {
                    initPlayer = Instantiate(playerObject,
                    spawnPosition[0].GetComponent<Transform>().position, spawnRot);
                    initPlayer.GetComponent<PlayerID>().ChangePlayerNumber(0);

                    AddPlayer(initPlayer);
                }
            }
        }
    }

}
