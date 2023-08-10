using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

// Holds the player ID for the player
public class PlayerID : MonoBehaviour
{
    [Header("Player Number")]
    [Range(0,4)]
    [SerializeField] private int playerNumber = 0;
    [SerializeField] private Color playerColor = Color.red;

    public void ChangePlayerNumber(int pN)
    {
        if (pN >= 0 && pN <= 4)
        {
            playerNumber = pN;
        }
    }

    public void ChangePlayerColour(Color color)
    {
        if (playerColor != null && playerColor != color)
        {
            playerColor = color;
        }
    }

    public int GetID() 
    { 
        return playerNumber;
    }

    public Color GetPlayerColor()
    {
        return playerColor;
    }
}
