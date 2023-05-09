using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(CameraFollow))]
public class CameraModifier : MonoBehaviour
{
    public Camera cam;

    public void SetPlayer(GameObject player)
    {
        GameObject gameController = GameObject.FindGameObjectWithTag("GameController");

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

    private void ModifyCamera(int playerID, int playerCount)
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

    // Camera value sets need to be change
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

    private void FourPlayerSetUp(int playerID)
    {
        // Camera value sets need to be change
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
