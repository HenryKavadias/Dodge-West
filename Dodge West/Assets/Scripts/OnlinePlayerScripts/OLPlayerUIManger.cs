using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

// Manages the players UI elements
public class OLPlayerUIManager : PlayerUIManager
{
    public bool Setup()
    {
        if (!GetComponent<PhotonView>().IsMine)
        {
            gameObject.SetActive(false);

            return false;
        }

        return true;
    }
    
    // Disable all elements, enable death message
    public override void TriggerDead()
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            healthImage.enabled = false;
            playerNumberText.enabled = false;
            playerHealthText.enabled = false;
            playerLivesText.enabled = false;

            crosshair.SetActive(false);

            deathMessage.SetActive(true);
        }
    }

    // Enable all elements, disnable death message
    public override void TriggerResurrection()
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            healthImage.enabled = true;
            playerNumberText.enabled = true;
            playerHealthText.enabled = true;
            playerLivesText.enabled = true;

            crosshair.SetActive(true);

            deathMessage.SetActive(false);
        }
    }

    // Disable all UI elements
    public override void DisablePlayerUI()
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            healthImage.enabled = false;
            playerNumberText.enabled = false;
            playerHealthText.enabled = false;
            playerLivesText.enabled = false;

            crosshair.SetActive(false);

            deathMessage.SetActive(false);
        }
    }
}
