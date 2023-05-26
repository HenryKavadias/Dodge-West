using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    public Image healthImage;

    public TextMeshProUGUI playerNumberText;

    public TextMeshProUGUI playerHealthText;

    public TextMeshProUGUI playerLivesText;

    public GameObject crosshair;

    public GameObject deathMessage;

    public void TriggerDead()
    {
        healthImage.enabled = false;
        playerNumberText.enabled = false;
        playerHealthText.enabled = false;
        playerLivesText.enabled = false;

        crosshair.SetActive(false);

        deathMessage.SetActive(true);
    }

    public void TriggerResurrection()
    {
        healthImage.enabled = true;
        playerNumberText.enabled = true;
        playerHealthText.enabled = true;
        playerLivesText.enabled = true;

        crosshair.SetActive(true);

        deathMessage.SetActive(false);
    }
}
