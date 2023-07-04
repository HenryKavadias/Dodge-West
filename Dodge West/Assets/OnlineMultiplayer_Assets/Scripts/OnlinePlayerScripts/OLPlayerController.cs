using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

//enum ButtonList
//{
//    Forward = 0,
//    Backward = 1,
//    Left = 2,
//    Right = 3,
//    Jump = 4,
//    Dash = 5,
//    Crouch = 6,
//    Sprint = 7,
//    Pickup = 8,
//    Throw = 9,
//    LoadItem = 10,
//    Pause = 11
//}

enum ButtonList
{
    Jump = 0,
    Dash = 1,
    Crouch = 2,
    Sprint = 3,
    Pickup = 4,
    Throw = 5,
    LoadItem = 6,
    Pause = 7
}

public struct NetworkInputData : INetworkInput
{
    public Vector2 movementInput;
    public Vector2 lookInput;

    public NetworkButtons buttons;
}

public class OLPlayerController : NetworkBehaviour, INetworkRunnerCallbacks
{
    private OLPlayerInputHandler playerInputHandler = null;
    private OLCameraManager cameraManager = null;

    private PlayerControls actionMap;

    // Might not need this variable
    //[Networked] public NetworkButtons ButtonsPrevious { get; set; }

    private void OnEnable()
    {
        // NetworkInputData
        if (Runner != null)
        {
            // Enabling the input map
            actionMap.Player.Enable();
            Runner.AddCallbacks(this);
        }
    }
    private void OnDisable()
    {
        if (Runner != null)
        {
            // Disabling the input map
            actionMap.Player.Disable();
            Runner.RemoveCallbacks(this);
        }
    }

    public override void Spawned()
    {
        //Debug.Log("State Authority (OnlinePlayerController): " + HasStateAuthority);

        actionMap = new PlayerControls();
        OnEnable(); // This is critical for INetworkInput to be detected

        if (HasStateAuthority)
        {
            Debug.Log("Setting up player");
            
            playerInputHandler = GetComponent<OLPlayerInputHandler>();

            playerInputHandler.GetInputs();

            playerInputHandler.SetupInputs();

            cameraManager = GetComponent<OLCameraManager>();
            //cameraManager.SetupCameraAndUI();
        }
        else
        {
            Debug.Log("Player doesn't have state Authority");
        }

        // Add a set camera target here (might not need)
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var playerActions = actionMap.Player;
        var data = new NetworkInputData();

        // Set "network input data" inputs

        // Movement
        data.movementInput.Set(
            playerActions.Movement.ReadValue<Vector2>().x,
            playerActions.Movement.ReadValue<Vector2>().y);

        // Look / Aiming
        data.lookInput.Set(
            playerActions.Look.ReadValue<Vector2>().x,
            playerActions.Look.ReadValue<Vector2>().y);


        //Jump
        data.buttons.Set(ButtonList.Jump, playerActions.Jump.IsInProgress());

        // Crouch (might be a bit tricky)
        data.buttons.Set(ButtonList.Crouch, playerActions.Crouch.IsPressed());

        // Sprint
        data.buttons.Set(ButtonList.Sprint, playerActions.Sprint.IsPressed());

        // Dash
        data.buttons.Set(ButtonList.Dash, playerActions.Dash.IsPressed());

        // Pickup
        data.buttons.Set(ButtonList.Pickup, playerActions.Pickup.IsPressed());

        // Throw
        data.buttons.Set(ButtonList.Throw, playerActions.Throw.IsPressed());

        // Pause / Menu
        data.buttons.Set(ButtonList.Pause, playerActions.Pause.IsPressed());

        // Load Item (still doesn't exist)
        data.buttons.Set(ButtonList.LoadItem, playerActions.LoadItem.IsPressed());

        input.Set(data);
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority == false)
        {
            //Debug.Log("SA (OnlinePC) is " + HasStateAuthority);
            
            return;
        }
        //Debug.Log("SA (OnlinePC) is " + HasStateAuthority);
        //InputSystem.Update();

        InputSystem.Update();

        if (GetInput(out NetworkInputData data))
        {
            //Debug.Log("Got Input");

            // Manage player inputs
            playerInputHandler.RefreshInputs(data);
        }

        // Update the player with those inputs
        playerInputHandler.UpdateInputs();

    }

    #region unused Fusion callbacks
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }


    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    #endregion
}
