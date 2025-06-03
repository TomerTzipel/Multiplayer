using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;


public class LobbyManager : MonoBehaviour,INetworkRunnerCallbacks
{
    
    [SerializeField] private NetworkRunner networkRunnerPrefab;

    [SerializeField] private LobbyUiManager lobbyUiManager;
    [SerializeField] private SessionUiManager sessionUiManager;
   
    private List<SessionInfo> _currentLobbySessions; 
    private string _currentLobbyName;

    private List<PlayerRef> _currentSessionPlayers;
    
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
            lobbyUiManager.ShowLobbySessionsList();
            Debug.Log($"Joined {CurrentRunner.LobbyInfo.Name} Lobby");
            _currentLobbyName = CurrentRunner.LobbyInfo.Name;
        }
    }

    public void JoinSession(string name, int playerCount)
    {
        CurrentRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = name,
            PlayerCount = playerCount,
            OnGameStarted = OnSessionStarted,
            CustomLobbyName = _currentLobbyName
        });   
    }
    public void JoinSession(string sessionName)
    {
        CurrentRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = sessionName,
            OnGameStarted = OnSessionStarted,
            CustomLobbyName = _currentLobbyName
        });
    }
    public async void ReturnToLobbySelection()
    {
        await CurrentRunner.Shutdown(); 
        lobbyUiManager.ShowLobbiesList();
        sessionUiManager.HideSessionPanel();
    }

    private void OnSessionStarted(NetworkRunner obj)
    {
        lobbyUiManager.HideLobbyPanels();
        _currentSessionPlayers = new List<PlayerRef>();
        sessionUiManager.ShowSessionPanel(CurrentRunner);
    }

    private void GenerateNetworkRunner()
    {
        CurrentRunner = Instantiate(networkRunnerPrefab);
        CurrentRunner.AddCallbacks(this);
    }

    #region CALLBACKS


    //Is only called when not in session!
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        _currentLobbySessions = sessionList;
        lobbyUiManager.UpdateLobbySessionsList(this,_currentLobbySessions,CurrentRunner);
    }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Destroy(CurrentRunner.gameObject);
        GenerateNetworkRunner(); 

        //This is done so when the player tries to join a session that is full within the lobby, he will stay in that lobby
        if(shutdownReason == ShutdownReason.GameIsFull)
        {
            Debug.Log("Trying to join a full game!");
            JoinLobby(_currentLobbyName);
        }
        else
        {
            _currentLobbyName = CurrentRunner.LobbyInfo.Name;

        }
    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        bool isLocalPlayer = CurrentRunner.LocalPlayer == player;

        Debug.Log($"Player {player.PlayerId} joined, localPlayer: {isLocalPlayer}");

        _currentSessionPlayers.Add(player);
        sessionUiManager.UpdateSessionUserCount(_currentSessionPlayers, CurrentRunner);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        bool isLocalPlayer = CurrentRunner.LocalPlayer == player;

        Debug.Log($"Player {player.PlayerId} left, localPlayer: {isLocalPlayer}");

        _currentSessionPlayers.Remove(player);
        sessionUiManager.UpdateSessionUserCount(_currentSessionPlayers, CurrentRunner);
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

   

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
       
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }

    #endregion
}
