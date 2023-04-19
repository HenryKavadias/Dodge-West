using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private PlayerInputManager inputManager;
    public int playerLimit = 2;
    // Start is called before the first frame update
    void Start()
    {
        inputManager = GetComponent<PlayerInputManager>();

        for (int i = 0; i < playerLimit; i++)
        {
            inputManager.JoinPlayer();
        }
    }

}
