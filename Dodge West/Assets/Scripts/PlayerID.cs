using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerID : MonoBehaviour
{
    [Header("Player Number")]
    [Range(0,2)]
    [SerializeField] private int playerNumber = 0;

    public void ChangePlayerNumber(int pN)
    {
        if (playerNumber >= 0 && playerNumber <= 2)
        {
            playerNumber = pN;
        }
    }

    public int Get() 
    { 
        return playerNumber;
    }
}
