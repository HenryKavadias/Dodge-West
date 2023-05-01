using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;

public class PlayerSetupMenuController : MonoBehaviour
{
    private int playerIndex;

    [SerializeField]
    private TextMeshProUGUI titleText;
    [SerializeField]
    private GameObject readyPanel;
    [SerializeField]
    private GameObject menuPanel;
    [SerializeField]
    private Button readyButton;

    // needs work (note)
    [SerializeField]
    private Button redButton;
    [SerializeField]
    private InputSystemUIInputModule uiNavigate;

    private SpawnPlayerSetupMenu spawnScriptRef;

    public void SetSpawnerReference(SpawnPlayerSetupMenu spawnRef)
    {
        spawnScriptRef = spawnRef;
    }

    private float ignoreInputTime = 1.5f;
    private bool inputEnabled;
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

    void BackAndCancel()
    {
        if (!inputEnabled) { return; }

        //InputSystemUIInputModule uiTest = new InputSystemUIInputModule();

        if (uiNavigate.cancel.action.triggered)
        {
            var player = PlayerConfigurationManager.Instance.
                GetPlayerConfigs().Find(p => p.PlayerIndex == playerIndex);

            if (player.isReady)
            {
                UnReadyPlayer();
            }
            else if (readyPanel.activeSelf)
            {
                UnSelectColor();
            }
            else if (menuPanel.activeSelf && spawnScriptRef)
            {
                // This needs fixing
                spawnScriptRef.PlayerLeaves();
            }
        }
    }

    public void SelectColor(Material mat)
    {
        if (!inputEnabled) { return; }

        PlayerConfigurationManager.Instance.SetPlayerColor(playerIndex, mat);
        readyPanel.SetActive(true);
        readyButton.interactable = true;
        menuPanel.SetActive(false);
        readyButton.Select();
    }

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

    public void ReadyPlayer()
    {
        if (!inputEnabled) { return; }

        PlayerConfigurationManager.Instance.ReadyPlayer(playerIndex);
        readyButton.gameObject.SetActive(false);
    }

    public void UnReadyPlayer()
    {
        if (!inputEnabled) { return; }
        PlayerConfigurationManager.Instance.UnReadyPlayer(playerIndex);
        readyButton.gameObject.SetActive(true);
    }
}
