using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;


public class GameNetworkManager : NetworkBehaviour , INetworkRunnerCallbacks
{

    [SerializeField] private int winningScore;

    [SerializeField] private SelectionNetworkManager selectionManager;
    [SerializeField] private GameUIManager gameUIManager;
    [SerializeField] private NetworkRunnerRef networkRunnerRef;
    [SerializeField] private CharactersRef charactersRef;
    [SerializeField] private SpectatorHandler spectatorPrefab;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineCamera cinemachineCamera;

    [SerializeField] private Transform[] redTeamSpawns;
    [SerializeField] private Transform[] blueTeamSpawns;
    [SerializeField] private Transform spectatorSpawn;

    [Networked] public NetworkBool IsGameRunning { get; set; } = false;

    [Networked, Capacity(8), OnChangedRender(nameof(OnPlayerScoreUpdate))]
    public NetworkDictionary<NetworkString<_8>, ScoreData> PlayersScoreData => default;
    [Networked,OnChangedRender(nameof(OnRedTeamScoreUpdate))] private int _redTeamScore { get; set; } = 0;
    [Networked, OnChangedRender(nameof(OnBlueTeamScoreUpdate))] private int _blueTeamScore { get; set; } = 0;
    [Networked, OnChangedRender(nameof(OnTimeUpdate))] private int _gameTimeInSeconds { get; set; } = 0;
    [Networked] private float _ticker { get; set; } = 1;

    public override void Spawned()
    {
        Runner.AddCallbacks(this);
    }

    public override void FixedUpdateNetwork()
    {
        if (!IsGameRunning) return;
        _ticker -= Runner.DeltaTime;
        if(_ticker <= 0)
        {
            _gameTimeInSeconds++;
            _ticker = 1;
        }
    }

    public void OnGameStart()
    {
        int redSpawncount = 0, blueSpawncount = 0;

        foreach (var kvp in selectionManager.PlayersData)
        {
            switch (kvp.Value.Team)
            {
                case Team.Red:
                    StartGame_RPC(kvp.Key, redTeamSpawns[redSpawncount].position, kvp.Value);
                    PlayersScoreData.Add(kvp.Value.Name, new ScoreData { charachterIndex = kvp.Value.CharacterIndex, Kills = 0, Deaths = 0, Team = Team.Red });
                    redSpawncount++;                 
                    break;

                case Team.Blue:
                    StartGame_RPC(kvp.Key, blueTeamSpawns[blueSpawncount].position, kvp.Value);
                    PlayersScoreData.Add(kvp.Value.Name, new ScoreData { charachterIndex = kvp.Value.CharacterIndex, Kills = 0, Deaths = 0, Team = Team.Blue });
                    blueSpawncount++;
                    break;

                case Team.Spectator:
                    StartGame_RPC(kvp.Key, spectatorSpawn.position, kvp.Value);
                    break;
            }
        }

        if (Runner.IsSharedModeMasterClient)
        {
            IsGameRunning = true;
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void StartGame_RPC([RpcTarget] PlayerRef targetPlayer,Vector3 spawnPoint,PlayerData playerData, RpcInfo info = default)
    {
        Runner.AddCallbacks(this);

        if(playerData.Team != Team.Spectator)
        {  
            PlayerCharacterController playerController = Runner.Spawn(charactersRef.Characters[playerData.CharacterIndex], spawnPoint,inputAuthority: targetPlayer, onBeforeSpawned: InitializeCharacter);
            playerController.OnDeath += HandlePlayerDeath;
            playerController.InitializeForLocalPlayer(cinemachineCamera);
        }
        else
        {
            SpectatorHandler spectator = Instantiate(spectatorPrefab, spectatorSpawn);
            spectator.Init(cinemachineCamera);
        }

        gameUIManager.SetUpGameUI();
    }

    private void HandlePlayerDeath(DeathData data)
    {
        PlayerDeath_RPC(data);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void PlayerDeath_RPC(DeathData data, RpcInfo info = default)
    { 
        ScoreData killerScoreData = PlayersScoreData[data.KillerName];
        ScoreData deadScoreData = PlayersScoreData[data.DeadName];

        killerScoreData.Kills++;
        deadScoreData.Deaths++;

        if (data.DeadTeam == Team.Red)
            _blueTeamScore++;
        else
            _redTeamScore++;

        PlayersScoreData.Set(data.KillerName, killerScoreData);
        PlayersScoreData.Set(data.DeadName, deadScoreData);
        CheckGameOver();
    }
    private void OnPlayerScoreUpdate()
    {
        gameUIManager.UpdateScoreboard(PlayersScoreData);
    }
    private void OnRedTeamScoreUpdate()
    {
        gameUIManager.UpdateScore(Team.Red, _redTeamScore);
    }
    private void OnBlueTeamScoreUpdate()
    {
        gameUIManager.UpdateScore(Team.Blue, _blueTeamScore);
    }

    private void CheckGameOver()
    {
        if(_blueTeamScore == winningScore)
        {
            Debug.Log("Blue Team Won");   
            IsGameRunning = false;
            GameOver_RPC(Team.Blue);
        }

        if (_redTeamScore == winningScore)
        {
            Debug.Log("Red Team Won");
            IsGameRunning = false;
            GameOver_RPC(Team.Red);
        }
    }

    [Rpc]
    private void GameOver_RPC(Team winningTeam, RpcInfo info = default)
    {
        Debug.Log("Game Is Over");
        Team localPlayerTeam = selectionManager.PlayersData[Runner.LocalPlayer].Team;
        gameUIManager.ActivateGameOverPanel(winningTeam, localPlayerTeam);
        //Runner.Shutdown();
    }

    private void OnTimeUpdate()
    {
        int minutes = _gameTimeInSeconds/60;
        int seconds = _gameTimeInSeconds%60;
        gameUIManager.UpdateTime(minutes, seconds);
    }
    private void InitializeCharacter(NetworkRunner runner, NetworkObject obj)
    { 
        obj.GetComponent<PlayerCharacterController>().NetworkInitialize(selectionManager.PlayersData[Runner.LocalPlayer]);
    }

    private void Reconnect()
    {
        if (!PlayerPrefs.HasKey("Session") || !PlayerPrefs.HasKey("Name"))
        {
            //Enter as spectator
        }
        
        string session = PlayerPrefs.GetString("Session");
        if (session != Runner.SessionInfo.Name)
        {
            //Enter as spectator
        }
            
        string name = PlayerPrefs.GetString("Name");
        
        foreach (var kvp in selectionManager.PlayersData)
        {
            if (kvp.Value.Name == name && kvp.Value.Team != Team.Spectator)
            {
                PlayerData tempPlayerData = kvp.Value;
                selectionManager.PlayersData.Remove(kvp.Key);
                selectionManager.PlayersData.Add(Runner.LocalPlayer, tempPlayerData);
                //reconnect as player
                return;
            }
        }
        
        //enter as spectator
    }

    #region Network Runner Callbacks
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!Runner.IsSharedModeMasterClient) return;

        if (IsGameRunning)
        {
            selectionManager.PlayersData.Add(player, new PlayerData() { CharacterIndex = SelectionNetworkManager.NO_CHARACTER, IsReady = true, Team = Team.Spectator, Name = $"Player{player.PlayerId}" });
            StartGame_RPC(player, spectatorSpawn.position, selectionManager.PlayersData[player]);
            return;
        }

        selectionManager.PlayersData.Add(player, new PlayerData() { CharacterIndex = SelectionNetworkManager.NO_CHARACTER, IsReady = true, Team = Team.Spectator, Name = $"Player{player.PlayerId}" });
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player {player.PlayerId} left");
        StartCoroutine(gameUIManager.ShowPlayerLeftPrompt((string)selectionManager.PlayersData[player].Name));

        if (!Runner.IsSharedModeMasterClient) return;

        if (IsGameRunning) return;

        selectionManager.PlayersData.Remove(player);
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
        
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
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
        Debug.Log("Scene Load Done");
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        throw new NotImplementedException();
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        /*PlayerPrefs.DeleteKey("Session");
        PlayerPrefs.DeleteKey("Name");
        PlayerPrefs.Save();*/
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        throw new NotImplementedException();
    }
    #endregion



}
