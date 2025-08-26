using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    Red, Blue, Spectator
}

public struct PlayerData : INetworkStruct
{
    public NetworkString<_8> Name;
    public Team Team;
    public int CharacterIndex;
    public NetworkBool IsReady;   
}

public class SelectionNetworkManager : NetworkBehaviour, INetworkRunnerCallbacks
{

    [Networked, Capacity(8),OnChangedRender(nameof(OnPlayerDataUpdate))]
    private NetworkDictionary<PlayerRef, PlayerData> _playersData  => default;

    public override void Spawned()
    {
        Debug.Log("In SelectionNetworkManager");
        Runner.AddCallbacks(this);
    }
    public void OnPlayerJoinTeam(Team team)
    {
        PlayerRef localPlayer = Runner.LocalPlayer;
        //Assign the player to the team [Network]
        //Update the proper UI
    }

    public void OnPlayerDataUpdate(NetworkBehaviourBuffer previous)
    {
        var priorPlayerData = GetDictionaryReader<PlayerRef, PlayerData>(nameof(_playersData)).Read(previous);
        Debug.Log("CVhange");
        //Check if someone chose a name and show him in the spectator list.

        //Check if someone changeed a team, if so handle the change
        //Check if someone who is on a team is now Ready, and update UI
        //Check if someone chose a character

        //Check if all players are ready, we can turn on the Host start button, if not turn it off
    }

    #region Network Runner Callbacks
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        _playersData.Add(player,new PlayerData() { CharacterIndex = -1,IsReady = true, Team = Team.Spectator,Name = $"Player{player.PlayerId}"}); 
    }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        //Handle Player Shutdown
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        //TODO: Handle Player Leave
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
        
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        throw new NotImplementedException();
    }



    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        throw new NotImplementedException();
    }



    #endregion


}
