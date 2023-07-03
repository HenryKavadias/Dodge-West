using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameController onlineGC = null;
    
    public NetworkObject playerPrefab;

    void Awake()
    {
        if (onlineGC == null )
        {
            //Debug.Log("No game controller!!!");
        }
    }

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            var spawnPosition = new Vector3 ( -0.1f, 1.5f, 0.2f );
            
            if (onlineGC)
            {
                spawnPosition = onlineGC.spawnPosition[0].transform.position;
            }

            // Look into "Is Local PlayerObject" under network object
            // (It's false for the none basic prototype scenes)

            Runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);

            //var curPlayer = Runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);

            //curPlayer.GetComponent<OLPlayerInputHandler>().InitializePlayer();
            //curPlayer.GetComponent<LifeCounter>().SetLives(1);

            //Runner.SetPlayerAlwaysInterested(player, curPlayer, true);

            // Add player to online list on game controller
        }
    }
}
