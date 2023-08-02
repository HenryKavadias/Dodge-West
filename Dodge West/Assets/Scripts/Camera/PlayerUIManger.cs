using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Manages the players UI elements
public class PlayerUIManager : MonoBehaviour
{
    // Player UI element references
    public Image healthImage;
    public TextMeshProUGUI playerNumberText;
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI playerLivesText;
    public GameObject crosshair;
    public GameObject deathMessage;

    // Disable all elements, enable death message
    public void TriggerDead()
    {
        healthImage.enabled = false;
        playerNumberText.enabled = false;
        playerHealthText.enabled = false;
        playerLivesText.enabled = false;

        crosshair.SetActive(false);

        deathMessage.SetActive(true);
    }

    // Enable all elements, disable death message
    public void TriggerResurrection()
    {
        healthImage.enabled = true;
        playerNumberText.enabled = true;
        playerHealthText.enabled = true;
        playerLivesText.enabled = true;

        crosshair.SetActive(true);

        deathMessage.SetActive(false);
    }

    // Disable all UI elements
    public void DisablePlayerUI()
    {
        healthImage.enabled = false;
        playerNumberText.enabled = false;
        playerHealthText.enabled = false;
        playerLivesText.enabled = false;

        crosshair.SetActive(false);

        deathMessage.SetActive(false);
    }
}
