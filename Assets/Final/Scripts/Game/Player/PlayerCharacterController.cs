using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

enum Buttons
{
    Move = 0,
    Attack = 1
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
    [field: SerializeField] public CamerasRef CamerasRef { get; private set; }
  
    [Networked] public PlayerData PlayerData { get; set; }

    public event UnityAction<Vector2> OnRangedAttack { add { abilityHandler.OnRangedAttack += value; } remove { abilityHandler.OnRangedAttack -= value; } }
    public event UnityAction OnStartMoving { add { movementHandler.OnStartMoving += value; } remove { movementHandler.OnStartMoving -= value; } }
    public event UnityAction OnStopMoving { add { movementHandler.OnStopMoving += value; } remove { movementHandler.OnStopMoving -= value; } }
    public void NetworkInitialize(PlayerData data)
    {
        PlayerData = data;
        healthHandler.Health = Settings.MaxHealth;
    }

    public override void Spawned()
    {
        _inputSystemActions = new InputSystem_Actions();
        playerNameText.transform.parent.forward = CamerasRef.MainCamera.transform.forward;
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

        if (Object.HasInputAuthority)
        {
            CamerasRef.CineCam.LookAt = transform;
            CamerasRef.CineCam.Follow = transform;
            OnEnable();
            
        }
    }
    private void OnEnable()
    {
        if (Object == null) return;

        if (Object.HasInputAuthority)
        {
            Runner.AddCallbacks(this);
            _inputSystemActions.Player.Enable();
        }
    }

    private void OnDisable()
    {
        if (Object == null) return;

        if (Object.HasStateAuthority)
        {
            Runner.RemoveCallbacks(this);
            _inputSystemActions.Player.Disable();
        }
    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (!HasInputAuthority) return;

        var playerInput = new PlayerInput();

        playerInput.Buttons.Set(Buttons.Move, _inputSystemActions.Player.MouseMove.IsPressed());
        playerInput.Buttons.Set(Buttons.Move, _inputSystemActions.Player.RangedAttack.IsPressed());
    }

  
    #region Unused Callbacks
    public void OnConnectedToServer(NetworkRunner runner)
    {

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
