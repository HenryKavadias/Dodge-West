using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (playerObject)
        {
            GameObject initPlayer = null;

            if (gameMode == GameMode.LocalMultiplayer && localMultiplayerLimit == 2)
            {
                if (spawnPosition[0] && spawnPosition[1])
                {
                    // Spawn player one
                    initPlayer = Instantiate(playerObject,
                        spawnPosition[0].GetComponent<Transform>().position, spawnRot);
                    initPlayer.GetComponent<PlayerID>().ChangePlayerNumber(1);

                    AddPlayer(initPlayer);

                    // Spawn player two
                    initPlayer = Instantiate(playerObject,
                        spawnPosition[1].GetComponent<Transform>().position, spawnRot);
                    initPlayer.GetComponent<PlayerID>().ChangePlayerNumber(2);

                    AddPlayer(initPlayer);
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
