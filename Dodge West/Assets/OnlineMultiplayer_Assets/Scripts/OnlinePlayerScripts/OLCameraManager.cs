using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;

//using Photon.Pun;
//using Photon.Pun.Demo.PunBasics;

// NOTE: look into making the OL (online) scripts child objects of the non-online ones
// Note: also might not need to change script name in this case
// note: may only need to make the "Update" functions "virtual" for control scripts this to work

// todo: do and IsMine photonview check when spawning in the camera and its UI

[RequireComponent(typeof(OLCameraControl), typeof(OLPhysicsPickup))]
public class OLCameraManager : CameraManager
{
    private GameObject gameManager = null;

    private NetworkObject obj = null;

    //private void Awake()
    //{
    //    gameManager = GameObject.FindGameObjectWithTag("GameController");

    //    obj = GetComponent<NetworkObject>();
    //}

    protected override void Start()
    {
        //if (CheckForAuthority())
        //{
        //    SetupCamera();

        //    SetupUI();
        //}
    }

    public void SetupCameraManager()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController");
        obj = GetComponent<NetworkObject>();

        SetupCamera();

        SetupUI();
    }

    // Check if this an online game
    bool CheckForOnline()
    {
        if (gameManager && gameManager.GetComponent<GameController>().gameMode == GameType.OnlineMultiplayer)
        {
            return true;
        }

        return false;
    }

    // Check if this is connected to the relavent player
    bool CheckForAuthority()
    {
        // Note: might need to use "HasStateAurthority"
        if (CheckForOnline() && obj && obj.HasStateAuthority)//&& view != null && view.IsMine)
        {
            return true;
        }
        //Debug.Log("State Authority (NetworkObject ref): " + obj.HasStateAuthority);
        return false;
    }

    protected override void SetupCamera()
    {
        // Spawn and setup the Camera
        if (cameraObject != null && GetComponent<OLCameraControl>().camPos != null)
        {
            // Create camera object
            GameObject camTemp = Instantiate(cameraObject);

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

    public override void UpdateID()
    {
        currentUI.GetComponent<OLPlayerUIManager>().playerNumberText.text =
            gameObject.GetComponent<PlayerID>().GetID().ToString();
    }

    // Spawn and setup the Player UI
    protected override void SetupUI()
    {
        if (playerUI != null && currentCam != null)
        {

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
