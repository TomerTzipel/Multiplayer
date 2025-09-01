using Fusion;
using Fusion.Sockets;
using HW2;
using HW3;
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;


public class GameNetworkManager : NetworkBehaviour , INetworkRunnerCallbacks
{
    [SerializeField] private NetworkRunnerRef networkRunnerRef;
    [SerializeField] private CharactersRef charactersRef;

    [SerializeField] private CamerasRef camerasRef;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineCamera cineCam;

    [SerializeField] private Transform[] redTeamSpawns;
    [SerializeField] private Transform[] blueTeamSpawns;
    [SerializeField] private Transform spectatorSpawn;

    [Networked, Capacity(8)]
    private NetworkDictionary<PlayerRef, PlayerData> _playersData => default;

    public override void Spawned()
    {
        Debug.Log("Spawning Game Manager");
        Runner.AddCallbacks(this);

        if (!HasStateAuthority) return;
        Debug.Log("Impossible");
        int redSpawncount = 0, blueSpawncount = 0;

        foreach (var kvp in networkRunnerRef.PlayerData)
        {
            _playersData.Add(kvp.Key, kvp.Value);

            switch (kvp.Value.Team)
            {
                case Team.Red:
                    Runner.Spawn(charactersRef.Characters[kvp.Value.CharacterIndex], redTeamSpawns[redSpawncount].position,inputAuthority: kvp.Key, onBeforeSpawned: InitializeCharacter);
                    redSpawncount++;
                    break;

                case Team.Blue:
                    Runner.Spawn(charactersRef.Characters[kvp.Value.CharacterIndex], blueTeamSpawns[blueSpawncount].position, inputAuthority: kvp.Key, onBeforeSpawned: InitializeCharacter);
                    blueSpawncount++;
                    break;
                case Team.Spectator:
                    //Create a spectator
                    break;
            }

        }     
    }

    private void InitializeCharacter(NetworkRunner runner, NetworkObject obj)
    { 
        obj.GetComponent<PlayerCharacterController>().NetworkInitialize(_playersData[obj.InputAuthority]);
    }

    #region Network Runner Callbacks

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

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
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
        
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        throw new NotImplementedException();
    }
    #endregion



}
