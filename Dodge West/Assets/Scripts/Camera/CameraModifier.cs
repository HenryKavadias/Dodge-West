using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Modifies the player camera depending on the game mode
public class CameraModifier : MonoBehaviour
{
    public Camera cam;

    public virtual void SetPlayer(GameObject player)
    {
        GameObject gameController = GameObject.FindGameObjectWithTag("GameController");

        // Only modifies the camera if the game mode is local multiplayer
        if (player && 
            player.GetComponent<PlayerID>() && 
            gameController && 
            gameController.GetComponent<GameController>().gameMode == GameType.LocalMultiplayer)
        {
            ModifyCamera(
                player.GetComponent<PlayerID>().GetID(), 
                gameController.GetComponent<GameController>().LivePlayerCount());
        }
    }

    // Changes the camera dimensions and position relative to the players ID
    protected void ModifyCamera(int playerID, int playerCount)
    {
        switch (playerCount)
        {
            case 2:
                TwoPlayerSetUp(playerID);
                break;
            case 3:
                ThreePlayerSetUp(playerID);
                break;
            case 4:
                FourPlayerSetUp(playerID);
                break;
            default:
                break;
        }
    }

    // Sets the camera for two players
    private void TwoPlayerSetUp(int playerID)
    {
        switch (playerID)
        {
            case 1:
                // X, Y, Width, Height
                cam.rect = new Rect(0f, 0f, 0.5f, 1f);
                break;
            case 2:
                // X, Y, Width, Height
                cam.rect = new Rect(0.5f, 0f, 0.5f, 1f);
                break;
            default:
                break;
        }
    }

    // Sets the camera for three players
    private void ThreePlayerSetUp(int playerID)
    {
        switch (playerID)
        {
            case 1:
                // X, Y, Width, Height
                cam.rect = new Rect(0f, 0.5f, 0.5f, 0.5f);
                break;
            case 2:
                // X, Y, Width, Height
                cam.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                break;
            case 3:
                // X, Y, Width, Height
                cam.rect = new Rect(0.25f, 0f, 0.5f, 0.5f);
                break;
            default:
                break;
        }
    }

    // Sets the camera for four players
    private void FourPlayerSetUp(int playerID)
    {
        switch (playerID)
        {
            case 1:
                // X, Y, Width, Height
                cam.rect = new Rect(0f, 0.5f, 0.5f, 0.5f);
                break;
            case 2:
                // X, Y, Width, Height
                cam.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                break;
            case 3:
                // X, Y, Width, Height
                cam.rect = new Rect(0f, 0f, 0.5f, 0.5f);
                break;
            case 4:
                // X, Y, Width, Height
                cam.rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
                break;
            default:
                break;
        }
    }
}
