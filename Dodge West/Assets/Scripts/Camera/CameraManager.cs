using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CameraControl), typeof(PhysicsPickup))]
public class CameraManager : MonoBehaviour
{
    // Plyaer Camera and UI prefabs
    [Header("Camera Object")]
    public GameObject cameraObject;

    [Header("UI Canvas")]
    public GameObject playerUI;

    // Current player camera and UI references
    public GameObject currentCam { get; protected set; }
    public GameObject currentUI { get; protected set; }

    protected virtual void Start()
    {
        SetupCamera();

        SetupUI();
    }

    protected virtual void SetupCamera()
    {
        // Spawn and setup the Camera
        if (cameraObject != null && GetComponent<CameraControl>().camPos != null)
        {
            // Create camera object
            GameObject camTemp = Instantiate(cameraObject);

            // Set camera modifier script
            camTemp.GetComponent<CameraModifier>().SetPlayer(gameObject);

            // Set camera controls script
            camTemp.GetComponent<CameraFollow>().SetTarget(GetComponent<CameraControl>().camPos);

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
    protected virtual void SetupUI()
    {
        if (playerUI != null && currentCam != null)
        {
            // Spawn UI
            currentUI = Instantiate(playerUI);

            // Assign to camera
            currentUI.GetComponent<Canvas>().worldCamera = currentCam.GetComponent<Camera>();
            currentUI.GetComponent<Canvas>().planeDistance = 1f;

            // Set UI values
            PlayerUIManager puim = currentUI.GetComponent<PlayerUIManager>();

            puim.playerNumberText.text = gameObject.GetComponent<PlayerID>().GetID().ToString();

            gameObject.GetComponent<HealthBar>().SetImage(puim.healthImage);
            gameObject.GetComponent<HealthBar>().SetTextDisplay(puim.playerHealthText);
            gameObject.GetComponent<HealthBar>().Start();

            // Set lives here
            LifeDisplay lD = gameObject.GetComponent<LifeDisplay>();
            if (lD)
            {
                lD.SetTextDisplay(puim.playerLivesText);
                lD.Start();
            }

        }
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
}
