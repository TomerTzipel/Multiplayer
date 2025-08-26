using Fusion;
using Fusion.Sockets;
using HW3;
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
        Runner.AddCallbacks(this);
    }
    public void OnPlayerJoinTeam(Team team)
    {
        //Call the proper rpc
    }

    public void OnPlayerDataUpdate(NetworkBehaviourBuffer previous)
    {
        var priorPlayerData = GetDictionaryReader<PlayerRef, PlayerData>(nameof(_playersData)).Read(previous);

        List<string> spectators = new List<string>(8);
        List<PlayerData> redTeamPlayers = new List<PlayerData>(2);
        List<PlayerData> blueTeamPlayers = new List<PlayerData>(2);

        foreach (var kvp in _playersData)
        {
            switch (kvp.Value.Team)
            {
                case Team.Red:
                    redTeamPlayers.Add(kvp.Value);
                    break;
                case Team.Blue:
                    blueTeamPlayers.Add(kvp.Value);
                    break;
                case Team.Spectator:
                    spectators.Add((string)kvp.Value.Name);
                    break;
            }

        }
        //Update the spectators UI

        //Update the Teams UI

        //Update character selection buttons

        if (!HasStateAuthority) return;
        
        bool EnableStartGameButton = true;
        foreach (var kvp in _playersData)
        {
            if (!kvp.Value.IsReady)
            {
                EnableStartGameButton = false;
                break;
            }
        }

        //Set The Button interictiable to the flag
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RequestCharacter_RPC(int characterIndex, RpcInfo info = default)
    {
        foreach (var kvp in _playersData)
        {
            if(kvp.Value.CharacterIndex == characterIndex)
            {
                return;
            }
        }

        PlayerData data = _playersData[info.Source];
        data.CharacterIndex = characterIndex;
        _playersData.Set(info.Source, data);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RequestName_RPC(string name, RpcInfo info = default)
    {
        foreach (var kvp in _playersData)
        {
            if (kvp.Value.Name == name)
            {
                NameRequestResult_RPC(info.Source,false);
                return;
            }
        }

        PlayerData data = _playersData[info.Source];
        data.Name = name;
        _playersData.Set(info.Source, data);
        NameRequestResult_RPC(info.Source, true);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void NameRequestResult_RPC([RpcTarget] PlayerRef targetPlayer,bool result)
    {
        if (result)
        {
            //Turn the name selection panel off
        }
        else
        {
            //Turn the name in use notice on
        }
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
        _playersData.Remove(player);
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
