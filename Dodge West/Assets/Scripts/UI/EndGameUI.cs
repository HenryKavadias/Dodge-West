using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class EndGameUI : MonoBehaviour
{
    public GameObject playerScorePrefab = null;

    public string startScene = "Start-Scene";
    public List<string> gameScenes = new List<string>();

    private float ignoreInputTime = 1.0f;
    private bool inputEnabled;
    private GameObject rootMenu = null;

    public GameObject eventSystem;  // Reference to event system
    public SceneTransition SceneTransition = null;

    // Take player back to start Menu
    public void SelectMenu()
    {
        if (SceneTransition != null)
        {
            if (GameData.GameDataInstance != null)
            {
                GameData.GameDataInstance.DestroySelf();
            }
            
            SceneTransition.LoadNextScene(startScene);
        }
    }
    
    // Replay the previous or a random level with the same players
    public void SelectReplay(bool random = false)
    {
        if (random && gameScenes.Count > 0)
        {
            System.Random rand = new System.Random();
            
            int randomScene = rand.Next(0, gameScenes.Count);

            SceneTransition.LoadNextScene(gameScenes[randomScene], true);
        }
        else
        {
            // Condition for random
        }
    }

    void Awake()
    {
        rootMenu = GameObject.Find("PlayerScores");
        var players = GameData.GameDataInstance.GetPlayers();

        // Spawn player scores under rootMenu
        foreach (var p in players)
        {
            var playerScore = Instantiate(playerScorePrefab, rootMenu.transform);

            playerScore.GetComponent<PlayerScoreData>().SetScoreData(
                p.Key.ToString(), p.Value.ToString());
        }
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
