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
    private NetworkDictionary<PlayerRef, PlayerData> _playersData  => default;

    public override void Spawned()
    {
        Debug.Log("Spawned");
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
        data.CharacterIndex = NO_CHARACTER;
        data.IsReady = team == Team.Spectator;
        _playersData.Set(player, data);
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RequestReady_RPC(bool value, PlayerRef player, RpcInfo info = default)
    {
        PlayerData data = _playersData[player];
        if (data.CharacterIndex == NO_CHARACTER)
        {
            RequestReadyResult_RPC(player, false);
            return;
        }

        data.IsReady = value;
        _playersData.Set(player, data);
        RequestReadyResult_RPC(player, true);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RequestReadyResult_RPC([RpcTarget] PlayerRef targetPlayer, bool result)
    {
        if (!result) OnPlayerDataUpdate();
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
        else OnPlayerDataUpdate();
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
            OnPlayerDataUpdate();
            uiManager.EnableNameWarning(true);
        }
    }

    public void OnGameStart()
    {
        Dictionary <PlayerRef, PlayerData> playerData = new Dictionary<PlayerRef, PlayerData>(8);
        foreach (var kvp in _playersData)
        {
            playerData.Add(kvp.Key, kvp.Value);
        }
        networkRunnerRef.PlayerData = playerData;

        networkRunnerRef.RemoveCallbacks(this);
        
        Debug.Log($"Selection - {networkRunnerRef.PlayerData.Count}");
        Runner.LoadScene("GameScene");
    }

    public void OnSelectionLeave()
    {
        Runner.Shutdown();
        SceneManager.LoadScene("MainMenuScene");
    }

    #region Network Runner Callbacks
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!HasStateAuthority) return;
        
        PlayerDataRequest_RPC(player);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void PlayerDataRequest_RPC([RpcTarget] PlayerRef targetPlayer)
    {
        Debug.Log($"PlayerDataRequest_RPC {PlayerPrefs.GetString("GameSession")}");
        if (!PlayerPrefs.HasKey("GameSession") && PlayerPrefs.GetString("GameSession") != networkRunnerRef.CurrentNetworkRunner.SessionInfo.Name)
        {
            PlayerPrefs.SetString("GameSession", networkRunnerRef.CurrentNetworkRunner.SessionInfo.Name);
            PlayerPrefs.SetString("Name", $"Player{targetPlayer.PlayerId}");
            PlayerPrefs.Save();
        }
        PlayerDataResponse_RPC(PlayerPrefs.GetString("Name"), targetPlayer);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void PlayerDataResponse_RPC(NetworkString<_8> name, PlayerRef player)
    {
        PlayerRef oldPlayerRef = player; //Initialize to be able to compile
        foreach (var kvp in _playersData)
        {
            if (kvp.Value.Name == PlayerPrefs.GetString("Name"))
            {
                oldPlayerRef = kvp.Key;
                PlayerData tempPlayerData = _playersData[oldPlayerRef];
                _playersData.Remove(oldPlayerRef);
                _playersData.Add(player, tempPlayerData);
                return;
            }
        }
        _playersData.Add(player, new PlayerData() { CharacterIndex = NO_CHARACTER, IsReady = true, Team = Team.Spectator,Name = $"Player{player.PlayerId}"});
    }
    
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        if (shutdownReason == ShutdownReason.Ok)
        {
            PlayerPrefs.DeleteKey("GameSession");
            PlayerPrefs.DeleteKey("Name");
            PlayerPrefs.Save();
        }
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (!HasStateAuthority) return;

        Debug.Log($"Player {player.PlayerId} left");
        _playersData.Remove(player);
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

    public async void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.Log("Migrated");
        await runner.Shutdown(shutdownReason: ShutdownReason.HostMigration);
        
        networkRunnerRef.GenerateNewRunner();
        
        StartGameResult result = await networkRunnerRef.CurrentNetworkRunner.StartGame(new StartGameArgs() {
            HostMigrationToken = hostMigrationToken,
            HostMigrationResume = HostMigrationResume
        });
    }

    private void HostMigrationResume(NetworkRunner runner)
    {
        Debug.Log("Resumed");
        foreach (var resumeNO in runner.GetResumeSnapshotNetworkObjects())
        {
            runner.Spawn(resumeNO, onBeforeSpawned: (runner, newNO) =>
            {
                newNO.CopyStateFrom(resumeNO);

                if (resumeNO.TryGetBehaviour<SelectionNetworkManager>(out var networkManager))
                {
                    newNO.GetComponent<SelectionNetworkManager>().CopyStateFrom(networkManager);
                }
            });
        }
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
