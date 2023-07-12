using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Fusion.Photon.Realtime;
using UnityEngine.InputSystem;
using ExitGames.Client.Photon.StructWrapping;

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

//public struct NetworkInputData : INetworkInput
//{
//    public Vector2 movementInput;
//    public Vector2 lookInput;

//    public NetworkButtons buttons;
//}

public class NetworkInputManager : SimulationBehaviour, INetworkRunnerCallbacks
{
    // creating a instance of the Input Action created
    private PlayerControls _actionMap;// = new PlayerControls();

    void Awake()
    {
        _actionMap = new PlayerControls();
    }

    public void OnEnable()
    {
        if (Runner != null)
        {
            // enabling the input map
            _actionMap.Player.Enable();

            Runner.AddCallbacks(this);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input) 
    {
        var _playerActions = _actionMap.Player;
        var _data = new NetworkInputData();

        _data.movementInput.Set(
            _playerActions.Movement.ReadValue<Vector2>().x, 
            _playerActions.Movement.ReadValue<Vector2>().y);
        //_data.buttons.Set(ButtonList.Jump, _playerActions.Jump.triggered);

        input.Set(_data);

        Debug.Log("Inputted");
    }

    public void OnDisable()
    {
        if (Runner != null)
        {
            // enabling the input map
            _actionMap.Player.Disable();

            Runner.RemoveCallbacks(this);
        }
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
