using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;


public class GameNetworkManager : NetworkBehaviour , INetworkRunnerCallbacks
{
    [SerializeField] private SelectionNetworkManager selectionManager;
    [SerializeField] private GameUIManager gameUIManager;
    [SerializeField] private NetworkRunnerRef networkRunnerRef;
    [SerializeField] private CharactersRef charactersRef;

    [SerializeField] private CamerasRef camerasRef;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineCamera cinemachineCamera;

    [SerializeField] private Transform[] redTeamSpawns;
    [SerializeField] private Transform[] blueTeamSpawns;
    [SerializeField] private Transform spectatorSpawn;

 
    public override void Spawned()
    {
       
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
                    redSpawncount++;                 
                    break;

                case Team.Blue:
                    StartGame_RPC(kvp.Key, redTeamSpawns[redSpawncount].position, kvp.Value);
                    blueSpawncount++;
                    break;

                case Team.Spectator:
                    StartGame_RPC(kvp.Key, spectatorSpawn.position, kvp.Value);
                    break;
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void StartGame_RPC([RpcTarget] PlayerRef targetPlayer,Vector3 spawnPoint,PlayerData playerData)
    {
        Runner.RemoveCallbacks(selectionManager);
        Runner.AddCallbacks(this);

        if(playerData.Team != Team.Spectator)
        {
            PlayerCharacterController playerController = Runner.Spawn(charactersRef.Characters[playerData.CharacterIndex], spawnPoint, onBeforeSpawned: InitializeCharacter);
            playerController.InitializeForLocalPlayer(cinemachineCamera);
        }
        else
        {
            //Spawn a spectator
        }

        gameUIManager.SetUpGameUI();
    }

    private void InitializeCharacter(NetworkRunner runner, NetworkObject obj)
    { 
        obj.GetComponent<PlayerCharacterController>().NetworkInitialize(selectionManager.PlayersData[Runner.LocalPlayer]);
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
        
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        throw new NotImplementedException();
    }
    #endregion



}
