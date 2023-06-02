using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UI;

public enum GameMode
{
    SinglePlayer,
    LocalMultiplayer,
    OnlineMultiplayer
}

public class GameController : MonoBehaviour
{
    public GameMode gameMode { get; private set; } = GameMode.SinglePlayer;

    public GameObject playerObject;

    public GameObject[] spawnPosition;

    public GameObject endGamePanel;
    public TextMeshProUGUI winningPlayerText;

    public GameObject transitionHandler;

    private Quaternion spawnRot = Quaternion.identity;

    // list of live Players
    private List<GameObject> livePlayers = new List<GameObject>();

    // Add player to player list
    void AddPlayer(GameObject player)
    {
        livePlayers.Add(player);
    }

    // Remove player from player list
    void RemovePlayer(GameObject player)
    {
        livePlayers.Remove(player);
    }

    public int LivePlayerCount()
    {
        return livePlayers.Count;
    }

    public void TogglePlayerControls(bool toggle)
    {
        foreach (GameObject player in livePlayers)
        {
            if (player.GetComponent<PlayerInput>())
            {
                player.GetComponent<PlayerInput>().enabled = toggle;

                //player.GetComponent<FirstPersonMovement>().enabled = toggle;
                //player.GetComponent<MouseLook>().enabled = toggle;
                //player.GetComponent<Dash>().enabled = toggle;
                //player.GetComponent<PhysicsPickup>().enabled = toggle;
                //player.GetComponent<PlayerInputHandler>().enabled = toggle;
            }
        }
    }

    // player might not be useful
    public void PlayerDies(GameObject player)
    {
        if (gameMode != GameMode.SinglePlayer)
        {
            int activePlayers = 0;

            GameObject livingPlayer = null;

            foreach (GameObject p in livePlayers)
            {
                if (p.activeSelf)
                {
                    activePlayers++;
                    livingPlayer = p;
                }
            }

            if (activePlayers == 1)
            {
                DisableAllPlayerUI();

                // Win UI and scene transition behavour here
                Debug.Log("Player " + livingPlayer.GetComponent<PlayerID>().GetID() + " Wins!!!");

                string message = "Player " + livingPlayer.GetComponent<PlayerID>().GetID().ToString() + " wins!!!";

                winningPlayerText.text = message;
                endGamePanel.SetActive(true);

                TriggerSceneTransition();
            }
            else if (activePlayers == 0)
            {
                DisableAllPlayerUI();

                string message = "All Players Are Dead!!!";

                winningPlayerText.text = message;
                endGamePanel.SetActive(true);

                TriggerSceneTransition();
            }
            //livePlayers.Find(p => p == player);
        }
        else if (gameMode == GameMode.SinglePlayer)
        {
            DisableAllPlayerUI();

            string message = "You Are Dead";

            winningPlayerText.text = message;
            endGamePanel.SetActive(true);

            TriggerSceneTransition();
        }
    }

    // if making online multiplayer, this may need to be modified for it
    void DisableAllPlayerUI()
    {
        foreach (GameObject p in livePlayers)
        {
            p.GetComponent<CameraManager>().DisableUI();
        }
    }

    void TriggerSceneTransition()
    {
        if (transitionHandler)
        {
            transitionHandler.GetComponent<SceneTransition>().enabled = true;
        }
        else
        {
            Debug.Log("ERROR, transition handler is missing, please fix.");
        }
        
        //GetComponent<SceneTransition>().enabled = true;
    }

    public void TriggerInstantSceneTransition()
    {
        if (transitionHandler)
        {
            transitionHandler.GetComponent<SceneTransition>().LoadNextScene();
        }
    }

    private void Start()
    {
        endGamePanel.SetActive(false);

        NewSystem();    // game mode is set in this function

        if (gameMode != GameMode.SinglePlayer)
        {
            GetComponent<Pause>().enabled = false; // works fine if you leave it on for local multiplayer
        }

    }
    void NewSystem()
    {
        // Single player instance if the scene is loaded directly, local multiplayer if player 
        // configuration manager exsists
        if (!PlayerConfigurationManager.Instance)
        {
            gameMode = GameMode.SinglePlayer;

            var player = Instantiate(
                playerObject,
                spawnPosition[0].GetComponent<Transform>().position,
                spawnRot);
            // if you spawn the player with a spawn position with altered rotation,
            // then the camera needs to be aware and respond to the change

            //var player = Instantiate(
            //    playerObject,
            //    spawnPosition[0].GetComponent<Transform>().position,
            //    spawnPosition[0].GetComponent<Transform>().rotation);
            player.GetComponent<PlayerInputHandler>().InitializePlayer();

            player.GetComponent<LifeCounter>().SetLives(1);

            AddPlayer(player);
        }
        else if (PlayerConfigurationManager.Instance)
        {
            gameMode = GameMode.LocalMultiplayer;

            var playerConfigs = PlayerConfigurationManager.Instance.GetPlayerConfigs().ToArray();

            for (int i = 0; i < playerConfigs.Length; i++)
            {
                var player = Instantiate(
                    playerObject,
                    spawnPosition[i].GetComponent<Transform>().position,
                    spawnRot);
                //var player = Instantiate(
                //    playerObject,
                //    spawnPosition[i].GetComponent<Transform>().position,
                //    spawnPosition[i].GetComponent<Transform>().rotation);
                player.GetComponent<PlayerInputHandler>().InitializePlayer(playerConfigs[i], i + 1);

                //player.GetComponent<LifeCounter>().SetLives(1); // for test purposes

                AddPlayer(player);
            }
        }
    }
}
