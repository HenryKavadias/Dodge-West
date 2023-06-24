using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;

// NOTE: look into making the OL (online) scripts child objects of the non-online ones
// Note: also might not need to change script name in this case
// note: may only need to make the "Update" functions "virtual" for control scripts this to work

// todo: do and IsMine photonview check when spawning in the camera and its UI

[RequireComponent(typeof(OLCameraControl), typeof(OLPhysicsPickup))]
public class OLCameraManager : CameraManager
{
    private GameObject gameManager = null;
    private PhotonView view = null;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController");

        view = GetComponent<PhotonView>();
    }

    protected override void Start()
    {
        if (CheckForPhotonView())
        {
            SetupCamera();

            SetupUI();
        }
    }

    // Check if this an online game
    public bool CheckForOnline()
    {
        if (gameManager && gameManager.GetComponent<GameController>().gameMode == GameMode.OnlineMultiplayer)
        {
            return true;
        }

        return false;
    }

    // Check if this is connected to the relavent player
    public bool CheckForPhotonView()
    {
        if (CheckForOnline() && view != null && view.IsMine)
        {
            return true;
        }

        return false;
    }

    protected override void SetupCamera()
    {
        // Spawn and setup the Camera
        if (cameraObject != null && GetComponent<OLCameraControl>().camPos != null)
        {
            // Create camera object
            GameObject camTemp = Instantiate(cameraObject);

            //if (CheckForOnline())
            //{
            //    camTemp = PhotonNetwork.Instantiate(cameraObject.name, transform.position, Quaternion.identity);
            //}
            //else
            //{
            //    camTemp = Instantiate(cameraObject);
            //}

            // Set camera modifier script
            camTemp.GetComponent<CameraModifier>().SetPlayer(gameObject);

            // Set camera controls script
            camTemp.GetComponent<CameraFollow>().SetTarget(GetComponent<OLCameraControl>().camPos);

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
    protected override void SetupUI()
    {
        if (playerUI != null && currentCam != null)
        {
            //if (CheckForOnline())
            //{
            //    currentUI = PhotonNetwork.Instantiate(playerUI.name, transform.position, Quaternion.identity);
            //}
            //else
            //{
            //    currentUI = Instantiate(playerUI);
            //}

            //currentUI = PhotonNetwork.Instantiate(playerUI.name, transform.position, Quaternion.identity);
            currentUI = Instantiate(playerUI);

            // Assign to camera
            currentUI.GetComponent<Canvas>().worldCamera = currentCam.GetComponent<Camera>();
            currentUI.GetComponent<Canvas>().planeDistance = 1f;

            // Set UI values
            OLPlayerUIManager puim = currentUI.GetComponent<OLPlayerUIManager>();

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
}
