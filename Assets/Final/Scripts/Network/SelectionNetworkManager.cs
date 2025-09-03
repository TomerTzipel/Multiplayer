using Fusion;
using Fusion.Sockets;
using HW3;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField] private NetworkRunnerRef networkRunnerRef;
    private const int NO_CHARACTER = -1;

    [Networked, Capacity(8),OnChangedRender(nameof(OnPlayerDataUpdate))]
    public NetworkDictionary<PlayerRef, PlayerData> PlayersData  => default;

    public override void Spawned()
    {
        Debug.Log("Session Manager Spawned");
        Runner.AddCallbacks(this);
        OnPlayerDataUpdate();
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RequestTeamChange_RPC(Team team, RpcInfo info = default)
    {
        PlayerData data = PlayersData[info.Source];
        data.Team = team;
        data.CharacterIndex = NO_CHARACTER;
        data.IsReady = team == Team.Spectator;
        PlayersData.Set(info.Source, data);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RequestReady_RPC(bool value, RpcInfo info = default)
    {
        PlayerData data = PlayersData[info.Source];
        if (data.CharacterIndex == NO_CHARACTER)
        {
            RequestReadyResult_RPC(info.Source, false);
            return;
        }

        data.IsReady = value;
        PlayersData.Set(info.Source, data);
        RequestReadyResult_RPC(info.Source, true);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RequestCharacter_RPC(int characterIndex, RpcInfo info = default)
    {
        foreach (var kvp in PlayersData)
        {
            if (kvp.Value.CharacterIndex == characterIndex)
            {
                CharacterRequestResult_RPC(info.Source, false);
                return;
            }
        }

        PlayerData data = PlayersData[info.Source];
        data.CharacterIndex = characterIndex;
        PlayersData.Set(info.Source, data);
        CharacterRequestResult_RPC(info.Source, true);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RequestName_RPC(string name, RpcInfo info = default)
    {
        foreach (var kvp in PlayersData)
        {
            if (kvp.Value.Name == name)
            {
                NameRequestResult_RPC(info.Source, false);
                return;
            }
        }

        PlayerData data = PlayersData[info.Source];
        data.Name = name;
        PlayersData.Set(info.Source, data);
        NameRequestResult_RPC(info.Source, true);
    }
   

    public void OnSelectionLeave()
    {
        if (Runner.IsSharedModeMasterClient)
            foreach (var kvp in PlayersData)
            {
                if (kvp.Value.Name != PlayersData[Runner.LocalPlayer].Name) Runner.SetMasterClient(kvp.Key);
            }
        
        Runner.Shutdown();
        SceneManager.LoadScene("MainMenuScene");
    }

    private void OnPlayerDataUpdate()
    {
        uiManager.UpdateUI(PlayersData);

        if (!Runner.IsSharedModeMasterClient)
        {
            uiManager.EnableStartGameButton(false);
            return;
        }

        bool EnableStartGameButton = true;
        bool redPlayerExist = false;
        bool bluePlayerExist = false;
        foreach (var kvp in PlayersData)
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

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RequestReadyResult_RPC([RpcTarget] PlayerRef targetPlayer, bool result)
    {
        if (!result) OnPlayerDataUpdate();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void CharacterRequestResult_RPC([RpcTarget] PlayerRef targetPlayer, bool result)
    {
        if (result)
        {
            uiManager.EnableCharacterSelectionPanel(false);
        }
        else OnPlayerDataUpdate();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void NameRequestResult_RPC([RpcTarget] PlayerRef targetPlayer,bool result)
    {
        
        if (result)
        {
            uiManager.EnableNameSelectionPanel(false);
        }
        else
        {
            OnPlayerDataUpdate();
            uiManager.EnableNameWarning(true);
        }
    }
    
    #region Network Runner Callbacks
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!Runner.IsSharedModeMasterClient) return;

        PlayersData.Add(player, new PlayerData() { CharacterIndex = NO_CHARACTER, IsReady = true, Team = Team.Spectator, Name = $"Player{player.PlayerId}" });
    }
    
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player {player.PlayerId} left");
        PlayersData.Remove(player);
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
        
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
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
