using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class LobbyManager : MonoBehaviour,INetworkRunnerCallbacks
{
    [SerializeField] private NetworkRunner networkRunnerPrefab;
    [SerializeField] private GameObject lobbiesPanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private TMP_Text lobbyTitle;
    [SerializeField] private TMP_Text lobbySessionsCount;

    private List<SessionInfo> _currentLobbySessions; 
    
    public NetworkRunner CurrentRunner { get; private set; }
    
    private void Awake()
    {
        ShowLobbiesList();
        GenerateNetworkRunner();
    }
    public async void JoinLobby(string lobbyID)
    {
        StartGameResult result = await CurrentRunner.JoinSessionLobby(SessionLobby.Custom, lobbyID);
        if (result.Ok) 
        {
            //Happens before the OnSessionListUpdated callback!
            ShowLobbySessionsList();
            Debug.Log($"Joined {lobbyID} Lobby");
        }
    }

    public void JoinSession()
    {
        CurrentRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = "Session #" + _currentLobbySessions.Count,
            OnGameStarted = OnSessionStarted
        });
    }

    public async void LeaveLobby()
    {
        await CurrentRunner.Shutdown();
        ShowLobbiesList();
    }

    private void OnSessionStarted(NetworkRunner obj)
    {
        Debug.Log("Joined Session:" + obj.SessionInfo.Name);
    }

    private void ShowLobbiesList()
    {
        lobbyPanel.SetActive(false);
        lobbiesPanel.SetActive(true);
    }
    private void ShowLobbySessionsList()
    {
        lobbyPanel.SetActive(true);
        lobbiesPanel.SetActive(false);
    }
    private void UpdateLobbySessionsList()
    {
        lobbyTitle.text = CurrentRunner.LobbyInfo.Name + " Lobby";
        lobbySessionsCount.text = _currentLobbySessions.Count.ToString();
    }

    private void GenerateNetworkRunner()
    {
        CurrentRunner = Instantiate(networkRunnerPrefab);
        CurrentRunner.AddCallbacks(this);
    }
    //Callbacks:~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


    //Is only called when not in session!
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        _currentLobbySessions = sessionList;
        UpdateLobbySessionsList();
        Debug.Log(sessionList.Count);
    }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Destroy(CurrentRunner.gameObject);
        GenerateNetworkRunner();
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        throw new NotImplementedException();
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        throw new NotImplementedException();
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        throw new NotImplementedException();
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        throw new NotImplementedException();
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        throw new NotImplementedException();
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        throw new NotImplementedException();
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        throw new NotImplementedException();
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }
}
