using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class TestPlayerPhys : NetworkBehaviour, INetworkRunnerCallbacks
{
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

    // NetworkInputData
    private PlayerControls _actionMap;
    // NetworkInputData
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

    [Networked] public NetworkButtons ButtonsPrevious { get; set; }
    // NetworkInputData


    private Rigidbody _rb;

    public float PlayerSpeed = 2f;

    private PlayerControls controls;

    private PlayerInput inputs;

    public override void Spawned()
    {
        Debug.Log("HasInputAuthority: " + HasInputAuthority);
        _actionMap = new PlayerControls();  // NetworkInputData

        //controls = new PlayerControls();
        OnEnable();

        //inputs = GetComponent<PlayerInput>();

        //inputs.onActionTriggered += InputActions;
    }

    private void OnEnable()
    {
        if (controls != null)
        {
            controls.Enable();
        }

        // NetworkInputData
        if (Runner != null)
        {
            // enabling the input map
            _actionMap.Player.Enable();

            Runner.AddCallbacks(this);
        }
    }
    private void OnDisable()
    {
        if (controls != null)
        {
            controls.Disable();
        }

        // NetworkInputData
        if (Runner != null)
        {
            // Disabling the input map
            _actionMap.Player.Disable();

            Runner.RemoveCallbacks(this);
        }
    }

    void InputActions(CallbackContext obj)
    {
        string actionName = obj.action.name;

        if (actionName == controls.Player.Movement.name)
        {
            OnMove(obj);
        }
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
    }

    //public void OnInput(NetworkRunner runner, NetworkInput input)
    //{

    //}

    public override void FixedUpdateNetwork()
    {
        if (HasInputAuthority == false)
        {
            return;
        }

        InputSystem.Update();

        // NetworkInputData
        if (GetInput(out NetworkInputData data))
        {
            horizontalInput = data.movementInput.x;
            verticalInput = data.movementInput.y;
            Debug.Log("Movement: NIS_NetworkData ");
        }

        //PlayerInput();
        SpeedControl();

        MovePlayer();
    }

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;
    public Transform orientation;

    void PlayerInput()
    {   
        if (GetComponent<PlayerInput>())
        {
            horizontalInput = movementInput.x;
            verticalInput = movementInput.y;
            Debug.Log("Movement: NIS");
        }
        else
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
            Debug.Log("Movement: OIS");
        }
        
    }

    void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        _rb.AddForce(moveDirection.normalized * PlayerSpeed * 10f, ForceMode.Force);

        //Debug.Log("Moving");
    }

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > PlayerSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * PlayerSpeed;
            _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
        }
    }

    // Movement control variables
    private Vector2 movementInput = Vector2.zero;

    public void OnMove(CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

}
