using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class LobbyManager : MonoBehaviour,INetworkRunnerCallbacks
{
    
    [SerializeField] private NetworkRunner networkRunnerPrefab;

    [SerializeField] private LobbyUiManager uiManager;
   
    private List<SessionInfo> _currentLobbySessions; 
    private string _currentLobbyName;
    
    public NetworkRunner CurrentRunner { get; private set; }
    
    private void Awake()
    {
        
        GenerateNetworkRunner();
    }
    public async void JoinLobby(string lobbyID)
    {
        StartGameResult result = await CurrentRunner.JoinSessionLobby(SessionLobby.Custom, lobbyID);
        if (result.Ok) 
        {
            //Happens before the OnSessionListUpdated callback!
            uiManager.ShowLobbySessionsList();
            Debug.Log($"Joined {CurrentRunner.LobbyInfo.Name} Lobby");
            _currentLobbyName = CurrentRunner.LobbyInfo.Name;
        }
    }

    public void JoinSession(string name, int playerCount)
    {
        Debug.Log("In Lobby:" + CurrentRunner.LobbyInfo.Name);
        CurrentRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = name,
            PlayerCount = playerCount,
            OnGameStarted = OnSessionStarted
        });   
    }
    public void JoinSession(string sessionName)
    {
        CurrentRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = sessionName,
            OnGameStarted = OnSessionStarted
        });
    }
    public async void LeaveLobby()
    {
        await CurrentRunner.Shutdown(); 
        uiManager.ShowLobbiesList();
    }

    private void OnSessionStarted(NetworkRunner obj)
    {
        uiManager.HideLobbyPanels();
        Debug.Log("Joined Session:" + obj.SessionInfo.Name);
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
        uiManager.UpdateLobbySessionsList(this,_currentLobbySessions,CurrentRunner);
        Debug.Log(sessionList.Count);
    }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Destroy(CurrentRunner.gameObject);
        GenerateNetworkRunner(); 

        //This is done so when the player tries to join a session that is full within the lobby, he will stay in that lobby
        if(shutdownReason == ShutdownReason.GameIsFull)
        {
            JoinLobby(_currentLobbyName);
        }
        else
        {
            _currentLobbyName = CurrentRunner.LobbyInfo.Name;

        }
    }

    //Left the throws so I could tell which callbacks I have to handle for the exercise
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
