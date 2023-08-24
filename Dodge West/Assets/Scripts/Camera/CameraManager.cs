using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CameraControl), typeof(PhysicsPickup))]
public class CameraManager : MonoBehaviour
{
    public bool disableRawNumbers = true;
    public bool enablePlayerNumber = true;
    
    // Plyaer Camera and UI prefabs
    [Header("Camera Object")]
    public GameObject cameraObject;

    [Header("UI Canvas")]
    public GameObject playerUI;

    [Header("Camera Target")]
    public Transform cameraTarget = null;

    // Current player camera and UI references
    public GameObject currentCam { get; private set; }
    public GameObject currentUI { get; private set; }

    void SetupCamera()
    {
        // Spawn and setup the Camera
        if (cameraObject != null && GetComponent<CameraControl>().camPos != null)
        {
            // Create camera object
            GameObject camTemp = Instantiate(cameraObject);

            // Set camera modifier script
            camTemp.GetComponent<CameraModifier>().SetPlayer(gameObject);

            // Set camera controls script
            //camTemp.GetComponent<CameraFollow>().SetTarget(GetComponent<CameraControl>().camPos);
            camTemp.GetComponent<CameraFollow>().SetTarget(cameraTarget);

            // Camera needs to be the first child object
            Camera camReal = camTemp.transform.GetChild(0).GetComponent<Camera>();

            // ...Same here
            gameObject.GetComponent<Dash>().SetPlayerCamera(camTemp.transform.GetChild(0).transform);

            // Keep the camera object as a reference
            currentCam = camTemp.transform.GetChild(0).gameObject;

            if (camReal != null)
            {
                // set camera to the pickup script
                gameObject.GetComponent<PhysicsPickup>().SetCamera(camReal);

                // Note: The code below may be unnecessary

                // Set camera to player input component
                //if (gameObject.GetComponent<PlayerInput>())
                //{
                //    gameObject.GetComponent<PlayerInput>().camera = camReal;
                //}

            }
            else
            {
                Debug.Log("Camera object has no object or compenent/objects are ill-positioned");
            }
        }
        else
        {
            Debug.Log("Player is without camera!!!");
        }
    }

    // Spawn and setup the Player UI
    void SetupUI()
    {
        if (playerUI != null && currentCam != null)
        {
            currentUI = Instantiate(playerUI);  // Create

            // Assign to camera
            currentUI.GetComponent<Canvas>().worldCamera = currentCam.GetComponent<Camera>();
            currentUI.GetComponent<Canvas>().planeDistance = 1f;

            // Set UI values
            PlayerUIManager puim = currentUI.GetComponent<PlayerUIManager>();

            puim.playerNumberText.text = gameObject.GetComponent<PlayerID>().GetID().ToString();

            gameObject.GetComponent<HealthBar>().SetImage(puim.healthImage);
            gameObject.GetComponent<HealthBar>().SetTextDisplay(puim.playerHealthText);
            gameObject.GetComponent<HealthBar>().Start();

            UpdatePlayerColour(gameObject.GetComponent<PlayerID>().GetPlayerColor());

            // Set lives here
            LifeDisplay lD = gameObject.GetComponent<LifeDisplay>();
            if (lD)
            {
                lD.SetTextDisplay(puim.playerLivesText);
                lD.Start();
            }

        }
    }
    
    public List<LayerMask> layerMasks = new List<LayerMask>();
    [SerializeField]
    private GameObject playerBody = null;

    private int playerNumber = -1;

    // Sets the layer of the player model/body (not the same one as the collider)
    // and deselects that layer from the culling selection on the camera.
    // This prevents the player from seeing themselves
    void SetCullingOfSelf()
    {
        // Player layer mask IDs:
        // - P1 -> 8
        // - P2 -> 9
        // - P3 -> 10
        // - P4 -> 11

        playerNumber = gameObject.GetComponent<PlayerID>().GetID();

        var cam = currentCam.GetComponent<Camera>();
        
        if (cam != null)
        {
            string layerName = "empty";

            switch (playerNumber)
            {
                case 0:
                    cam.cullingMask = layerMasks[0];

                    layerName = LayerMask.LayerToName(8);
                    break;
                case 1:
                    cam.cullingMask = layerMasks[1];

                    layerName = LayerMask.LayerToName(8);
                    break;
                case 2:
                    cam.cullingMask = layerMasks[2];

                    layerName = LayerMask.LayerToName(9);
                    break;
                case 3:
                    cam.cullingMask = layerMasks[3];

                    layerName = LayerMask.LayerToName(10);
                    break;
                case 4:
                    cam.cullingMask = layerMasks[4];

                    layerName = LayerMask.LayerToName(11);
                    break;
                default:
                    cam.cullingMask = layerMasks[0];

                    layerName = LayerMask.LayerToName(8);
                    break;
            }

            // Set layer of player model so it is culled
            if (layerName != "empty")
            {
                SetLayerOfChildren(layerName, playerBody);
            }
        }
    }

    private void SetLayerOfChildren(string layerName, GameObject currentObject)
    {
        foreach (Transform child in currentObject.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer(layerName);

            if (child.childCount > 0)
            {
                SetLayerOfChildren(layerName, child.gameObject);
            }
        }
    }

    public void UpdateLifeUI(int lifeCount)
    {
        currentUI.GetComponent<PlayerUIManager>().RefreshLifeList(lifeCount);
    }

    public void UpdateInventoryUI(bool loading = false, GameObject objectUI = null)
    {
        if (loading)
        {
            currentUI.GetComponent<PlayerUIManager>().AddObjectToInventory(objectUI);
        }
        else
        {
            currentUI.GetComponent<PlayerUIManager>().RemoveObjectFromInventory();
        }
    }

    public void EmptyInventoryUI()
    {
        currentUI.GetComponent<PlayerUIManager>().ClearInventoryUI();
    }

    public void UpdatePlayerColour(Color color)
    {
        currentUI.GetComponent<PlayerUIManager>().SetPlayerColour(color);
    }

    // Triggers death state for player UI
    public void TriggerPlayerDeathUI()
    {
        currentUI.GetComponent<PlayerUIManager>().TriggerDead();
    }
    
    // Disables player UI
    public void DisableUI()
    {
        currentUI.GetComponent<PlayerUIManager>().DisablePlayerUI();
    }

    public void EnableUI()
    {
        currentUI.GetComponent<PlayerUIManager>().TriggerResurrection();
    }

    public void TriggerDamageIndicator()
    {
        currentUI.GetComponent<PlayerUIManager>().StartDamageIndication();
    }

    void Start()
    {
        SetupCamera();

        SetupUI();

        if (disableRawNumbers && currentUI)
        {
            currentUI.GetComponent<PlayerUIManager>().DisableRawNumbersForPlayer(enablePlayerNumber);
        }

        SetCullingOfSelf();
    }
}
