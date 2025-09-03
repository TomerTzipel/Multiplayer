using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MainMenuNetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{

    [SerializeField] private NetworkRunnerRef networkRunnerRef;
    [SerializeField] private MainMenuUIManager uiManager;
    private List<SessionInfo> _sessions = new List<SessionInfo>(4);

    private NetworkRunner Runner { get { return networkRunnerRef.CurrentNetworkRunner; } }
    private const string lobbyName = "MainLobby";
    private void Awake()
    {
        networkRunnerRef.GenerateRunner(this);
        JoinMainLobby();
    }

    public bool CreateSession(string name,bool isVisible,int playerCount)
    {

        //Disallow duplication of session names
        foreach (SessionInfo session in _sessions) 
        {
            if (session.Name == name) return false;
        }

        uiManager.EnableAllButtons(false);

        Runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = name,
            CustomLobbyName = lobbyName,
            PlayerCount = playerCount,
            OnGameStarted = OnSessionStarted,
            IsVisible = isVisible
        });

        return true;
    }
    public bool JoinSession(string name)
    {
        //Make sure the session exists and isn't creating a new session 
        bool doesExist = false;
        foreach (SessionInfo session in _sessions)
        {
            if (session.Name == name) doesExist = true;
        }

        if (!doesExist) return false;

        uiManager.EnableAllButtons(false);
        Runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = name,   
            OnGameStarted = OnSessionStarted
        });

        return true;
    }

    private void OnSessionStarted(NetworkRunner obj)
    {
        Runner.RemoveCallbacks(this);

        if (!Runner.IsSceneAuthority) return;
        Runner.LoadScene("CombinedScene");       
    }

    private async void JoinMainLobby()
    {
        await Runner.JoinSessionLobby(SessionLobby.Custom, lobbyName);
    }

    #region Network Runner Callbacks
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        throw new NotImplementedException();
    }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("Session List Updated");
        _sessions = sessionList;
        uiManager.UpdateSessions(_sessions);
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        networkRunnerRef.GenerateRunner(this);
        uiManager.EnableAllButtons(false);
        JoinMainLobby();
        uiManager.EnableAllButtons(true);
    }
    public void OnConnectedToServer(NetworkRunner runner)
    {
        
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        throw new NotImplementedException();
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        throw new NotImplementedException();
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        throw new NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        throw new NotImplementedException();
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        throw new NotImplementedException();
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        throw new NotImplementedException();
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        throw new NotImplementedException();
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }


    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
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

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        throw new NotImplementedException();
    }
    #endregion

}
