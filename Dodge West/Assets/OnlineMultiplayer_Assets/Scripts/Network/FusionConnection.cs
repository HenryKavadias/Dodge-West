using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Fusion.Photon.Realtime;

public class FusionConnection : MonoBehaviour, INetworkRunnerCallbacks
{
    public void OnConnectedToServer(NetworkRunner runner) { Debug.Log("OnConnectedToServer"); }
    public void OnDisconnectedFromServer(NetworkRunner runner) { Debug.LogWarning("OnDisconnectedFromServer"); }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) 
    { Debug.LogWarning("OnConnectFailed"); }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) 
    {
        Debug.Log("Player: " + player.PlayerId + " has join");

        var spawnPosition = new Vector3(-0.1f, 1.5f, 0.2f);

        if (player == runner.LocalPlayer)
        {
            //runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);

            runner.Spawn(playerPrefab, position: spawnPosition, rotation: Quaternion.identity, player, (runner, obj) => { });
        }
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) 
    {
        Debug.Log("Player: " + player.PlayerId + " has left");
    }

    [Header("Game Arguments")]
    [SerializeField]
    private GameMode gameMode;
    [SerializeField]
    private string roomName;

    //public PhotonAppSettings settings;

    [SerializeField] 
    private NetworkRunner runner;

    [SerializeField]
    private NetworkObject playerPrefab;

    private INetworkSceneManager sceneManager;

    void Awake()
    {
        // Ensure that network runner is on this object
        if (runner == null)
        {
            if (transform.gameObject.GetComponent<NetworkRunner>())
            {
                runner = transform.gameObject.GetComponent<NetworkRunner>();
            }
            else
            {
                runner = gameObject.AddComponent<NetworkRunner>();
            }
        }
    }

    async void Start()
    {
        await Connect();
    }

    public async Task Connect()
    {
        // Create the scene manger if it doesn't exist
        if (sceneManager == null) { sceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(); }

        var args = new StartGameArgs()
        {
            GameMode = gameMode,
            SessionName = roomName,
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = sceneManager,
            //CustomPhotonAppSettings = settings.AppSettings;
        };

        await runner.StartGame(args);
    }


    #region unused Fusion callbacks
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
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
