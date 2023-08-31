using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class EndGameUI : MonoBehaviour
{
    public GameObject playerScorePrefab = null;

    private float ignoreInputTime = 1.0f;
    private bool inputEnabled;
    private GameObject rootMenu = null;

    public GameObject eventSystem;  // Reference to event system

    // Take player back to start Menu
    public void SelectMenu()
    {

    }
    
    // Replay the previous or a random level with the same players
    public void SelectReplay(bool random = false)
    {

    }

    void Awake()
    {
        rootMenu = GameObject.Find("PlayerScores");

        // Spawn player scores under rootMenu
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > ignoreInputTime)
        {
            inputEnabled = true;
        }

        // Back button
        if (eventSystem.GetComponent<InputSystemUIInputModule>().
            cancel.action.triggered)
        {
            SelectMenu();
        }
    }

    // Go to level select scene for a game with new players (not implemented)
    public void SelectLevel()
    {

    }
}
