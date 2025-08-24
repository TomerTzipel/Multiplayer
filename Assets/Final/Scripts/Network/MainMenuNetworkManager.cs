using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuNetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    private const int MIN_PLAYERS = 4;
    private const int MAX_PLAYERS = 10;


    [SerializeField] private NetworkRunnerRef networkRunnerRef;

    private List<SessionInfo> _sessions;

    private NetworkRunner Runner { get { return networkRunnerRef.CurrentNetworkRunner; } }

    private void Awake()
    {
        networkRunnerRef.GenerateRunner(this);
    }

    public bool CreateSession(string name,bool isVisible,int playerCount)
    {

        //Disallow duplication of session names
        foreach (SessionInfo session in _sessions) 
        {
            if (session.Name == name) return false;
        }

        //TODO: Disable buttons
        Runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionName = name,
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

        //TODO: Disable buttons
        Runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = name,
            OnGameStarted = OnSessionStarted
        });

        return true;
    }

    private void OnSessionStarted(NetworkRunner obj)
    {
        if (!Runner.IsSceneAuthority) return;
        Runner.LoadScene("SelectionScene");
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
        _sessions = sessionList;
        //TODO: Update UI
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        throw new NotImplementedException();
    }
    public void OnConnectedToServer(NetworkRunner runner)
    {
        throw new NotImplementedException();
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
