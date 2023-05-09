using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerID : MonoBehaviour
{
    [Header("Player Number")]
    [Range(0,4)]
    [SerializeField] private int playerNumber = 0;

    public void ChangePlayerNumber(int pN)
    {
        if (pN >= 0 && pN <= 4)
        {
            playerNumber = pN;
        }
    }

    public int GetID() 
    { 
        return playerNumber;
    }
}
