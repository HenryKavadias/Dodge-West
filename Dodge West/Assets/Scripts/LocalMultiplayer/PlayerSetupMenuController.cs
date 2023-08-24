using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;

// Controls the menu functions for a join player in the local multiplayer setup scene
public class PlayerSetupMenuController : MonoBehaviour
{
    private int playerIndex;

    [SerializeField]
    private List<Material> playerColours = new List<Material>();

    // UI elements
    [SerializeField]
    private TextMeshProUGUI titleText;
    [SerializeField]
    private GameObject readyPanel;
    [SerializeField]
    private GameObject menuPanel;
    [SerializeField]
    private Button readyButton;
    [SerializeField]
    private Button redButton;
    [SerializeField]
    private InputSystemUIInputModule uiNavigate;

    private SpawnPlayerSetupMenu spawnScriptRef;

    // Sets the reference of the spawn script for the player menu
    public void SetSpawnerReference(SpawnPlayerSetupMenu spawnRef)
    {
        spawnScriptRef = spawnRef;
    }

    // Controls inital input delay (blocks player input for a time when UI is spawned in)
    private float ignoreInputTime = 1.0f;
    private bool inputEnabled;

    // Set the player index and text for player menu UI
    public void SetPlayerIndex(int pi)
    {
        playerIndex = pi;
        titleText.SetText("Player " + (pi + 1).ToString());
        ignoreInputTime = Time.time + ignoreInputTime;
    }

    void Update()
    {
        if (Time.time > ignoreInputTime)
        {
            inputEnabled = true;
        }

        BackAndCancel();
    }

    // Controls the players' ability to back out of the UI
    void BackAndCancel()
    {
        if (!inputEnabled) { return; }

        //InputSystemUIInputModule uiTest = new InputSystemUIInputModule();

        if (uiNavigate.cancel.action.triggered)
        {
            // total reset at any point in the selection.
            // Doing this until the source of the UI nav-bug can be fixed
            
            spawnScriptRef.PlayerLeaves();


            // Note: Do Not delete the below code
            //var player = PlayerConfigurationManager.Instance.
            //    GetPlayerConfigs().Find(p => p.PlayerIndex == playerIndex);

            //if (player.isReady)
            //{
            //    UnReadyPlayer();
            //}
            //else if (readyPanel.activeSelf)
            //{
            //    UnSelectColor();
            //}
            //else if (menuPanel.activeSelf && spawnScriptRef)
            //{
            //    // This needs fixing
            //    spawnScriptRef.PlayerLeaves();
            //}
        }
    }

    // Set the player colour (Note: might change to "set model")
    public void SelectColor(Material mat)
    {
        if (!inputEnabled) { return; }

        PlayerConfigurationManager.Instance.SetPlayerColor(playerIndex, mat);
        readyPanel.SetActive(true);
        readyButton.interactable = true;
        menuPanel.SetActive(false);
        readyButton.Select();
    }

    // Undoes selected colour
    public void UnSelectColor()
    {
        if (!inputEnabled) { return; }

        PlayerConfigurationManager.Instance.SetPlayerColor(playerIndex, null);
        readyPanel.SetActive(false);
        readyButton.interactable = false;
        menuPanel.SetActive(true);
        redButton.interactable = true;
        redButton.Select();
    }

    // Set the player to a ready state
    public void ReadyPlayer()
    {
        if (!inputEnabled) { return; }

        PlayerConfigurationManager.Instance.ReadyPlayer(playerIndex);
        readyButton.gameObject.SetActive(false);
    }

    // Unready player
    public void UnReadyPlayer()
    {
        if (!inputEnabled) { return; }

        PlayerConfigurationManager.Instance.UnReadyPlayer(playerIndex);
        readyButton.gameObject.SetActive(true);
    }
}
