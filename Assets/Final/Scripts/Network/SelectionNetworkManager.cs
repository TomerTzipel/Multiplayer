using Fusion;
using Fusion.Sockets;
using HW3;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
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
    [SerializeField] private SelectionUIManager uiManager;

    [Networked, Capacity(8),OnChangedRender(nameof(OnPlayerDataUpdate))]
    private NetworkDictionary<PlayerRef, PlayerData> _playersData  => default;

    public override void Spawned()
    {
        Runner.AddCallbacks(this);
        OnPlayerDataUpdate();
    }
    

    public void OnPlayerDataUpdate()
    {
        uiManager.UpdateUI(_playersData);

        if (!HasStateAuthority) return;
        
        bool EnableStartGameButton = true;
        bool redPlayerExist = false;
        bool bluePlayerExist = false;
        foreach (var kvp in _playersData)
        {
            if (!kvp.Value.IsReady)
            {
                EnableStartGameButton = false;
                break;
            }

            if (kvp.Value.Team == Team.Red) redPlayerExist = true;
            if (kvp.Value.Team == Team.Blue) bluePlayerExist = true;
        }

        if (!redPlayerExist || !bluePlayerExist) EnableStartGameButton = false;

        uiManager.EnableStartGameButton(EnableStartGameButton);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RequestTeamChange_RPC(Team team, PlayerRef player, RpcInfo info = default)
    {
        PlayerData data = _playersData[player];
        data.Team = team;
        data.CharacterIndex = -1;
        data.IsReady = team == Team.Spectator;
        _playersData.Set(player, data);
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RequestReady_RPC(bool value, PlayerRef player, RpcInfo info = default)
    {
        PlayerData data = _playersData[player];
        data.IsReady = value;
        _playersData.Set(player, data);
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RequestCharacter_RPC(int characterIndex, PlayerRef player, RpcInfo info = default)
    {
        foreach (var kvp in _playersData)
        {
            if(kvp.Value.CharacterIndex == characterIndex)
            {
                CharacterRequestResult_RPC(player, false);
                return;
            }
        }

        PlayerData data = _playersData[player];
        data.CharacterIndex = characterIndex;
        _playersData.Set(player, data);
        CharacterRequestResult_RPC(player, true);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void CharacterRequestResult_RPC([RpcTarget] PlayerRef targetPlayer, bool result)
    {
        if (result)
        {
            uiManager.EnableCharacterSelectionPanel(false);
        }
        else uiManager.UpdateUI(_playersData);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RequestName_RPC(string name,PlayerRef player ,RpcInfo info = default)
    {
        foreach (var kvp in _playersData)
        {
            if (kvp.Value.Name == name)
            {
                NameRequestResult_RPC(player, false);
                return;
            }
        }

        PlayerData data = _playersData[player];
        data.Name = name;
        _playersData.Set(player, data);
        NameRequestResult_RPC(player, true);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void NameRequestResult_RPC([RpcTarget] PlayerRef targetPlayer,bool result)
    {
        
        if (result)
        {
            uiManager.EnableNameSelectionPanel(false);
        }
        else
        {
            uiManager.UpdateUI(_playersData);
            uiManager.EnableNameWarning(true);
        }
    }

    public void OnGameStart()
    {
        //ORI
        //Set up passing the data to that scene 
        //Move to game scene
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
