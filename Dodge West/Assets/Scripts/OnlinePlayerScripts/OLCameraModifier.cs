using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// Modifies the player camera depending on the game mode
public class OLCameraModifier : CameraModifier
{
    public override void SetPlayer(GameObject player)
    {
        if (true)
        {
            GameObject gameController = GameObject.FindGameObjectWithTag("GameController");

            // Only modifies the camera if the game mode is local multiplayer
            if (player &&
                player.GetComponent<PlayerID>() &&
                gameController &&
                gameController.GetComponent<GameController>().gameMode == GameMode.LocalMultiplayer)
            {
                ModifyCamera(
                    player.GetComponent<PlayerID>().GetID(),
                    gameController.GetComponent<GameController>().LivePlayerCount());
            }
        }
    }
}
