using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// Manages the players UI elements
public class PlayerUIManager : MonoBehaviour
{
    // Can be overrided buy the CameraManager
    public bool disableRawNumbers = true;
    
    // Player UI element references
    public Image healthImage;
    public TextMeshProUGUI playerNumberText;
    public GameObject playerColourBanner;
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI playerLivesText;
    public GameObject crosshair;
    public GameObject deathMessage;

    public GameObject damageIndicatorUI;
    public Animator damageIndicatorAnimator;

    // List display that shows the number of remaining player lives
    public GameObject lifeUIPrefab;
    public Transform lifeListHolder;
    private List<GameObject> lifeList = new List<GameObject>();

    // List display for loaded inventory
    public GameObject objectUIBasePrefab; // Might not need
    public GameObject plusUIRef;
    public Transform inventoryObjectListHolder;
    public int inventoryDisplayLimit = 7;
    private List<GameObject> inventoryList = new List<GameObject>();

    private bool playerNumberActive;

    // Refreshes the UI list of lives
    public void RefreshLifeList(int lifeCount)
    {
        if (lifeList.Count > 0)
        {
            foreach (GameObject l in lifeList)
            {
                Destroy(l);
            }
        }
        
        lifeList.Clear();
        for (int i = 0; i < lifeCount; i++)
        {
            var lifeUI = Instantiate(lifeUIPrefab, lifeListHolder);
            lifeList.Add(lifeUI);
        }
    }

    // Adds object to inventory UI
    public void AddObjectToInventory(GameObject obj = null)
    {
        GameObject curObject = null;

        if (obj)
        {
            curObject = obj;
        }
        else
        {
            curObject = objectUIBasePrefab;
        }

        if (inventoryList.Count >= (inventoryDisplayLimit + 1))
        {
            var objectUI = Instantiate(curObject, inventoryObjectListHolder);
            inventoryList.Add(objectUI);
            objectUI.SetActive(false);
        }
        else
        {
            var objectUI = Instantiate(curObject, inventoryObjectListHolder);
            inventoryList.Add(objectUI);

            if (inventoryList.Count >= (inventoryDisplayLimit + 1))
            {
                objectUI.SetActive(false);

                // Enable plus reference
                plusUIRef.SetActive(true);
            }
        }
    }

    // Removes object to inventory UI
    public void RemoveObjectFromInventory(GameObject obj = null)
    {
        if (inventoryList.Count > 0)
        {
            Destroy(inventoryList[0]);
            inventoryList.RemoveAt(0);
        }

        if (inventoryList.Count >= inventoryDisplayLimit)
        {
            if (!inventoryList[inventoryDisplayLimit - 1].activeSelf)
            {
                inventoryList[inventoryDisplayLimit - 1].SetActive(true);
            }

            if (inventoryList.Count < (inventoryDisplayLimit + 1))
            {
                // Disable plus reference
                plusUIRef.SetActive(false);
            }
        }
    }

    // Clears the inventory UI
    public void ClearInventoryUI()
    {
        plusUIRef.SetActive(false);

        if (inventoryList.Count > 0)
        {
            foreach(GameObject obj in inventoryList)
            {
                Destroy(obj);
            }
        }
        inventoryList.Clear();
    }

    public void SetPlayerColour(Color color)
    {
        Color ogColor = playerColourBanner.GetComponent<Image>().color;
        float alpha = ogColor.a;

        playerColourBanner.GetComponent<Image>().color = color;

        // Keep the Alpha level the same
        Color colorChange = playerColourBanner.GetComponent<Image>().color;
        colorChange.a = alpha;
        playerColourBanner.GetComponent<Image>().color = colorChange;
    }

    // Starts the animation for the damage indicator
    public void StartDamageIndication()
    {
        Debug.Log("Oof");
        
        if (damageIndicatorAnimator && !CheckIfInAnimation())
        {
            damageIndicatorUI.SetActive(true);
            damageIndicatorAnimator.SetTrigger("Start");
        }
    }

    // Checks if damage indication animation is in progress
    private bool CheckIfInAnimation()
    {
        return damageIndicatorAnimator.GetCurrentAnimatorStateInfo(0).length >
           damageIndicatorAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    private void Start()
    {
        if (disableRawNumbers)
        {
            if (playerNumberActive)
            {
                playerNumberText.enabled = true;
            }
            else
            {
                playerNumberText.enabled = false;
            }
            playerHealthText.enabled = false;
            playerLivesText.enabled = false;
        }
    }

    public void DisableRawNumbersForPlayer(bool playerNumber = false)
    {
        disableRawNumbers = true;
        playerNumberActive = playerNumber;

        if (playerNumberActive)
        {
            playerNumberText.enabled = true;
        }
        else
        {
            playerNumberText.enabled = false;
        }
        playerHealthText.enabled = false;
        playerLivesText.enabled = false;
    }

    // Disable all elements, enable death message
    public void TriggerDead()
    {
        healthImage.enabled = false;

        playerNumberText.enabled = false;
        playerHealthText.enabled = false;
        playerLivesText.enabled = false;

        crosshair.SetActive(false);

        deathMessage.SetActive(true);

        lifeListHolder.gameObject.SetActive(false);
        inventoryObjectListHolder.gameObject.SetActive(false);
        playerColourBanner.SetActive(false);
    }

    // Enable all elements, disable death message
    public void TriggerResurrection()
    {
        healthImage.enabled = true;

        if (!disableRawNumbers)
        {
            playerNumberText.enabled = true;
            playerHealthText.enabled = true;
            playerLivesText.enabled = true;
        }
        else if (playerNumberActive)
        {
            playerNumberText.enabled = true;
        }
        else
        {
            playerNumberText.enabled = false;
        }

        crosshair.SetActive(true);

        deathMessage.SetActive(false);

        lifeListHolder.gameObject.SetActive(true);
        inventoryObjectListHolder.gameObject.SetActive(true);
        playerColourBanner.SetActive(true);
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

        if (plusUIRef.activeSelf)
        {
            plusUIRef.SetActive(false);
        }

        lifeListHolder.gameObject.SetActive(false);
        inventoryObjectListHolder.gameObject.SetActive(false);
        playerColourBanner.SetActive(false);
    }
}
