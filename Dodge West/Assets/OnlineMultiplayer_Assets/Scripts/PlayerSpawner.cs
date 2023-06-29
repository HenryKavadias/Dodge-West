using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameController onlineGC;
    
    public GameObject playerPrefab;

    void Awake()
    {
        if ( onlineGC == null )
        {
            onlineGC = GameObject.FindGameObjectWithTag("GameController").GetComponent<OnlineGameController>();
        }

        if (onlineGC == null )
        {
            Debug.Log("Can't find game controller!!!");
        }
    }

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            var spawnPosition = onlineGC.spawnPosition[0].transform.position;
            
            var curPlayer = Runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);

            curPlayer.GetComponent<OLPlayerInputHandler>().InitializePlayer();
            curPlayer.GetComponent<LifeCounter>().SetLives(1);

            // Add player to online list on game controller
        }
    }
}
