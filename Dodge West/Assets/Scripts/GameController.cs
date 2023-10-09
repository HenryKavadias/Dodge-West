using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UI;

// Game modes for the game
public enum GameMode
{
    SinglePlayer,
    LocalMultiplayer,
    OnlineMultiplayer
}

public class GameController : MonoBehaviour
{
    // Variables for the game controller
    public bool startUpDelay = true;

    public bool startTutorialForSinglePlayer = false;
    public TutorialSequence tutorialSequence = null;

    public string nextScene = "EndGame-Scene";

    // Tracks the current game mode
    public GameMode gameMode { get; private set; } = GameMode.SinglePlayer;

    // Player character object
    public GameObject playerObject;

    public int defaultLifes = 3;

    // Spawn positions for the player
    public GameObject[] spawnPosition;

    // End game UI variables
    public GameObject endGamePanel;
    public TextMeshProUGUI winningPlayerText;
    public GameObject timerDisplay;
    public List<GameObject> timerPositions = new List<GameObject>();

    // Transition handler reference variable
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

    // Returns the number of live players
    public int LivePlayerCount()
    {
        return livePlayers.Count;
    }

    public GameObject GetLivePlayer(int index)
    { 
        return livePlayers[index]; 
    }

    // Switches the players controls on and off
    public void TogglePlayerControls(bool toggle)
    {
        foreach (GameObject player in livePlayers)
        {
            if (player.GetComponent<PlayerInput>())
            {
                // Disables the players input system, WARNING: might not work for
                // local multiplayer (although it is currently switched off for it)
                if (gameMode == GameMode.SinglePlayer)
                {
                    player.GetComponent<PlayerInput>().enabled = toggle;
                }
                
                player.GetComponent<PlayerInputHandler>().ToggleControls(toggle);

                //player.GetComponent<PlayerInput>().enabled = toggle;

                //player.GetComponent<FirstPersonMovement>().enabled = toggle;
                //player.GetComponent<CameraControl>().enabled = toggle;
                //player.GetComponent<Dash>().enabled = toggle;
                //player.GetComponent<PhysicsPickup>().enabled = toggle;
                //player.GetComponent<PlayerInputHandler>().enabled = toggle;
            }
        }
    }

    private GameObject remainingPlayer = null;

    // player might not be useful
    public void PlayerDies(GameObject player)
    {
        if (gameMode != GameMode.SinglePlayer)
        {
            // Multiplayer condition
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

            // Controls the end game scenario
            if (activePlayers == 1)
            {
                DisableAllPlayerUI();

                remainingPlayer = livingPlayer;

                // Win UI and scene transition behavour here
                Debug.Log("Player " + livingPlayer.GetComponent<PlayerID>().GetID() + " Wins!!!");

                string message = "Player " + livingPlayer.GetComponent<PlayerID>().GetID().ToString() + " wins!!!";

                if (timerDisplay != null)
                {
                    timerDisplay.SetActive(false);
                }

                winningPlayerText.text = message;
                endGamePanel.SetActive(true);

                // Save winning player
                winningPlayer = livingPlayer.GetComponent<PlayerID>().GetID();

                if ((transitionHandler.GetComponent<SceneTransition>().timeLeft - saveScoreDelayDifference) > 1)
                {
                    Invoke(nameof(SavePlayerScore),
                        transitionHandler.GetComponent<SceneTransition>().timeLeft - saveScoreDelayDifference);
                }
                else
                {
                    Invoke(nameof(SavePlayerScore), 1f);
                }

                TriggerSceneTransition();
            }
            else if (activePlayers == 0)
            {
                DisableAllPlayerUI();

                remainingPlayer = null;

                string message = "All Players Are Dead!!!";

                if (timerDisplay != null)
                {
                    timerDisplay.SetActive(false);
                }

                winningPlayerText.text = message;
                endGamePanel.SetActive(true);

                // No one gets any score
                allDead = true;

                if ((transitionHandler.GetComponent<SceneTransition>().timeLeft - saveScoreDelayDifference) > 1)
                {
                    Invoke(nameof(SavePlayerScore),
                        transitionHandler.GetComponent<SceneTransition>().timeLeft - saveScoreDelayDifference);
                }
                else
                {
                    Invoke(nameof(SavePlayerScore), 1f);
                }

                TriggerSceneTransition();
            }
            //livePlayers.Find(p => p == player);
        }
        else if (gameMode == GameMode.SinglePlayer)
        {
            DisableAllPlayerUI();

            if (timerDisplay != null)
            {
                timerDisplay.SetActive(false);
            }

            string message = "You Are Dead";

            winningPlayerText.text = message;
            endGamePanel.SetActive(true);

            // Single player doesn't need data container (at least in tutorial)
            if (dataContainer != null)
            {
                dataContainer.GetComponent<Destroyer>().DestroyThis();
            }

            TriggerSceneTransition();
        }
    }

    private bool allDead = false;
    private int winningPlayer = 0;
    private GameObject dataContainer = null;

    public float saveScoreDelayDifference = 1;

    // Add score for winning player
    private void SavePlayerScore()
    {
        if (gameMode != GameMode.SinglePlayer && !allDead)
        {
            dataContainer.GetComponent<GameData>().AddScore(winningPlayer, 1);
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

    // Triggers delayed scene transition
    void TriggerSceneTransition()
    {
        if (transitionHandler)
        {
            // Make it work with end game UI scene
            if (gameMode != GameMode.SinglePlayer)
            {
                transitionHandler.GetComponent<SceneTransition>().SetNextScene(nextScene, true);
                transitionHandler.GetComponent<SceneTransition>().enabled = true;
                return;
            }

            transitionHandler.GetComponent<SceneTransition>().enabled = true;
        }
        else
        {
            Debug.Log("ERROR, transition handler is missing, please fix.");
        }
        
        //GetComponent<SceneTransition>().enabled = true;
    }

    // Triggers immediate scene transition
    public void TriggerInstantSceneTransition()
    {
        if (transitionHandler)
        {
            transitionHandler.GetComponent<SceneTransition>().LoadNextScene();
        }
    }

    private void Start()
    {
        if (gameObject.GetComponent<Timer>().enabled == false)
        {
            timerDisplay.SetActive(false);
        }
        
        endGamePanel.SetActive(false);

        NewSystem();    // game mode is set in this function

        //GetComponent<Pause>().UnPauseGame();

        if (gameMode != GameMode.SinglePlayer)
        {
            GetComponent<Pause>().enabled = false; // works fine if you leave it on for local multiplayer
        }

    }

    private void SetPlayerRotationToSpawn(GameObject player, GameObject spawn)
    {
        //player.GetComponent<PlayerInputHandler>().DisableControls();

        player.GetComponent<PlayerInputHandler>().ToggleControls(false);

        // Must use euler angles, not rotation.y as that won't give the angle of rotation
        player.GetComponent<CameraControl>().SetOrientationYRotation(
                spawn.transform.eulerAngles.y);

        //player.GetComponent<PlayerInputHandler>().EnableControls();

        player.GetComponent<PlayerInputHandler>().ToggleControls(true);
    }

    void NewSystem()
    {
        // Single player instance if the scene is loaded directly, local multiplayer if player 
        // configuration manager exsists
        if (!PlayerConfigurationManager.Instance)
        {
            // Setup for single player

            gameMode = GameMode.SinglePlayer;

            var player = Instantiate(
                playerObject,
                spawnPosition[0].GetComponent<Transform>().position,
                spawnRot);

            // Activate player setup
            player.GetComponent<PlayerInputHandler>().InitializePlayer();
            
            // Set player spawn rotation 
            SetPlayerRotationToSpawn(player, spawnPosition[0]);

            // Set player lifes
            player.GetComponent<LifeCounter>().SetLives(defaultLifes);

            AddPlayer(player);

            // Start Tutorial
            if (startTutorialForSinglePlayer && tutorialSequence != null)
            {
                tutorialSequence.enabled = true;
            }
        }
        else if (PlayerConfigurationManager.Instance)
        {
            // Setup for local multiplayer
            
            gameMode = GameMode.LocalMultiplayer;

            var playerConfigs = PlayerConfigurationManager.Instance.GetPlayerConfigs().ToArray();

            for (int i = 0; i < playerConfigs.Length; i++)
            {
                var player = Instantiate(
                    playerObject,
                    spawnPosition[i].GetComponent<Transform>().position,
                    spawnRot);

                player.GetComponent<PlayerInputHandler>().InitializePlayer(playerConfigs[i], i + 1);

                SetPlayerRotationToSpawn(player, spawnPosition[i]);

                player.GetComponent<LifeCounter>().SetLives(defaultLifes);
                //player.GetComponent<LifeCounter>().SetLives(1); // for test purposes

                AddPlayer(player);
            }

            //dataContainer = GameObject.FindGameObjectWithTag("DataContainer");
            dataContainer = GameData.GameDataInstance.gameObject;
            if (dataContainer != null && dataContainer.GetComponent<GameData>().GetPlayerCount() <= 0)
            {
                Debug.Log("score list set");
                dataContainer.GetComponent<GameData>().SetPlayerList(livePlayers);
            }
        }

        if (startUpDelay)
        {
            TogglePlayerControls(false);

            gameObject.GetComponent<Timer>().TriggerStartDelay();
        }
        else if (gameMode != GameMode.SinglePlayer)
        {
            gameObject.GetComponent<Timer>().BeginRainCountDown();
        }
        else
        {
            timerDisplay.SetActive(false);
        }
    }

    void AlterTimerPosition()
    {
        Debug.Log(livePlayers.Count);
        switch (livePlayers.Count)
        {
            case 2:
                if (timerPositions.Count > 0)
                {
                    timerDisplay.GetComponent<RectTransform>().anchoredPosition =
                        timerPositions[0].GetComponent<RectTransform>().anchoredPosition;
                    timerDisplay.GetComponent<RectTransform>().position =
                        timerPositions[0].GetComponent<RectTransform>().position;
                }
                break;

            case 3:
                if (timerPositions.Count > 1)
                {
                    timerDisplay.GetComponent<RectTransform>().anchoredPosition =
                        timerPositions[1].GetComponent<RectTransform>().anchoredPosition;
                    timerDisplay.GetComponent<RectTransform>().position =
                        timerPositions[1].GetComponent<RectTransform>().position;
                }
                break;

            case 4:
                if (timerPositions.Count > 2)
                {
                    timerDisplay.GetComponent<RectTransform>().anchoredPosition =
                        timerPositions[2].GetComponent<RectTransform>().anchoredPosition;
                    timerDisplay.GetComponent<RectTransform>().position =
                        timerPositions[2].GetComponent<RectTransform>().position;
                }
                break;

            default:
                timerDisplay.GetComponent<RectTransform>().position = new Vector3(0, -100, 0);
                break;
        }
    }
}
