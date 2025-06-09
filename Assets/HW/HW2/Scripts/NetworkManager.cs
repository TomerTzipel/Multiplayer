using Fusion;
using Fusion.Sockets;
using HW1;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace HW2
{
    public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        public static NetworkManager Instance { get; private set; }
        private const int MAX_PLAYERS = 10;

        [SerializeField] private NetworkRunner networkRunnerPrefab;

        private List<SessionInfo> _currentLobbySessions;
        private string _currentLobbyName;

        private List<PlayerRef> _currentSessionPlayers;

        public NetworkRunner CurrentRunner { get; private set; }

        public event UnityAction OnJoinLobby;
        public event UnityAction<List<SessionInfo>> OnSessionListUpdate;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            GenerateNetworkRunner();
            _currentSessionPlayers = new List<PlayerRef>();
        }
        public async void JoinLobby(string lobbyID)
        {
            StartGameResult result = await CurrentRunner.JoinSessionLobby(SessionLobby.Custom, lobbyID);
            if (result.Ok)
            {
                //Happens before the OnSessionListUpdated callback!
                OnJoinLobby.Invoke();
                _currentLobbyName = CurrentRunner.LobbyInfo.Name;
            }
        }

        public void JoinSession(string name)
        {
            CurrentRunner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Shared,
                SessionName = name,
                PlayerCount = MAX_PLAYERS,
                OnGameStarted = OnSessionStarted,
                CustomLobbyName = _currentLobbyName
            });
        }
        private void OnSessionStarted(NetworkRunner obj)
        {
            Debug.Log("JOINED THE SESSION");
            //Move to game scene?
            
        }
        private void GenerateNetworkRunner()
        {
            CurrentRunner = Instantiate(networkRunnerPrefab);
            CurrentRunner.AddCallbacks(this);
        }


        #region CALLBACKS


        //Is only called when not in session!
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            _currentLobbySessions = sessionList;
            OnSessionListUpdate.Invoke(sessionList);
        }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Destroy(CurrentRunner.gameObject);
            GenerateNetworkRunner();

            //This is done so when the player tries to join a session that is full within the lobby, he will stay in that lobby
            if (shutdownReason == ShutdownReason.GameIsFull)
            {
                Debug.Log("Trying to join a full game!");
                JoinLobby(_currentLobbyName);
            }
            else
            {
                _currentLobbyName = CurrentRunner.LobbyInfo.Name;

            }
        }
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            bool isLocalPlayer = CurrentRunner.LocalPlayer == player;

            Debug.Log($"Player {player.PlayerId} joined, localPlayer: {isLocalPlayer}");

            _currentSessionPlayers.Add(player);
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            bool isLocalPlayer = CurrentRunner.LocalPlayer == player;

            Debug.Log($"Player {player.PlayerId} left, localPlayer: {isLocalPlayer}");

            _currentSessionPlayers.Remove(player);
        }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {

        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {

        }



        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {

        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {

        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {

        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {

        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
        {

        }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {

        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {

        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {

        }

        public void OnConnectedToServer(NetworkRunner runner)
        {

        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {

        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {

        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {

        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {

        }

        #endregion
    }
}



