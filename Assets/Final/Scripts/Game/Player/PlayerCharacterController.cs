using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

enum Buttons
{
    Move = 0,
    BasicAttack = 1
}
public struct PlayerInput : INetworkInput
{
    public NetworkButtons Buttons;
}

public class PlayerCharacterController : NetworkBehaviour , INetworkRunnerCallbacks
{
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private CharacterMovementHandler movementHandler;
    [SerializeField] private CharacterHealthHandler healthHandler;
    [SerializeField] private CharacterAbilityHandler abilityHandler;
    [SerializeField] private AnimationStateHandler animationStateHandler;

    private InputSystem_Actions _inputSystemActions;

    [field: SerializeField] public CharacterSettings Settings { get; private set; }

    public Camera MainCamera { get; private set; }

    [Networked] public Vector3 SpawnPoint { get; set; }
    [Networked] public PlayerData PlayerData { get; set; }

    public event UnityAction<Vector2> OnBasicAttack { add { abilityHandler.OnBasicAttack += value; } remove { abilityHandler.OnBasicAttack -= value; } }
    public event UnityAction OnStartMoving { add { movementHandler.OnStartMoving += value; } remove { movementHandler.OnStartMoving -= value; } }
    public event UnityAction OnStopMoving { add { movementHandler.OnStopMoving += value; } remove { movementHandler.OnStopMoving -= value; } }
    public event UnityAction<DeathData> OnDeath { add { healthHandler.OnDeath += value; } remove { healthHandler.OnDeath -= value; } }
    public void NetworkInitialize(PlayerData data)
    {
        SpawnPoint = Object.transform.position;
        PlayerData = data;
        healthHandler.Health = Settings.MaxHealth;      
    }

    public override void Spawned()
    {
        _inputSystemActions = new InputSystem_Actions();

        MainCamera = Camera.main;
        playerNameText.transform.parent.forward = MainCamera.transform.forward;
        playerNameText.text = (string)PlayerData.Name;

        switch (PlayerData.Team)
        {
            case Team.Red:
                playerNameText.color = Color.red;
                break;
            case Team.Blue:
                playerNameText.color = Color.blue;
                break;
            default:
                break;
        }

    }
    public void InitializeForLocalPlayer(CinemachineCamera cinemachineCamera)
    {
        if (!HasStateAuthority) return;

        cinemachineCamera.LookAt = transform;
        cinemachineCamera.Follow = transform;
        OnEnable();

    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        OnDisable();
    }

    private void OnEnable()
    {
        if (Object == null || Runner == null) return;

        if (Object.HasStateAuthority)
        {
            Runner.AddCallbacks(this);
            _inputSystemActions.Player.Enable();
            _inputSystemActions.Player.MouseMove.Enable();
            _inputSystemActions.Player.RangedAttack.Enable();
        }
    }

    private void OnDisable()
    {
        if (Object == null || Runner == null) return;

        if (Object.HasStateAuthority)
        {
            Runner.RemoveCallbacks(this);
            
            if(_inputSystemActions == null) return;

            _inputSystemActions.Player.MouseMove.Disable();
            _inputSystemActions.Player.Scoreboard.Disable();
            _inputSystemActions.Player.RangedAttack.Disable();
            _inputSystemActions.Player.Disable();
        }
    }


    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (!HasStateAuthority) return;

        var playerInput = new PlayerInput();

        playerInput.Buttons.Set(Buttons.Move, _inputSystemActions.Player.MouseMove.IsPressed());
        playerInput.Buttons.Set(Buttons.BasicAttack, _inputSystemActions.Player.RangedAttack.IsPressed());

        input.Set(playerInput);
    }

  
    #region Unused Callbacks
    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Player - Connected To Server");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {

    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
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

    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {

    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("Player - Scene Load Done");
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {

    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {

    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }

  


    #endregion

}
