using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class CharacterMovementHandler : NetworkBehaviour
{
    private const float OFFSET = 0.25f;
    private const string WALKABLE_LAYER = "Walkable";
    private const string RED_WALKABLE_LAYER = "RedWalkable";
    private const string BLUE_WALKABLE_LAYER = "BlueWalkable";
    [SerializeField] private PlayerCharacterController controller;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GameObject visualsParent;

    [Networked] private Vector3 _destiantion { get; set; }

    [Networked] private NetworkBool _hasPath { get; set; } = false;

    public event UnityAction OnStartMoving;
    public event UnityAction OnStopMoving;
    public override void Spawned()
    { 
        agent.enabled = true;
        agent.speed = controller.Settings.BaseSpeed;
        agent.Warp(transform.position);
        agent.updatePosition = false;
        agent.updateRotation = false;
        OnEnable();
    }
    private void OnEnable()
    {
        if (Object == null) return;

        controller.OnBasicAttack += HandleRangedAttack;
        controller.OnDeath += Respawn;
    }
    private void OnDisable()
    {
        if (Object == null) return;

        controller.OnBasicAttack -= HandleRangedAttack;
        controller.OnDeath -= Respawn;
    }

    
    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        bool result = GetInput<PlayerInput>(out var input);

        if (input.Buttons.IsSet(Buttons.Move))
        {
            SetMoveTarget();
        }

        TurnTowardsMoveDirection();
        Move();
    }

    private void SetMoveTarget()
    {
        string[] layers ={ WALKABLE_LAYER };

        if(controller.PlayerData.Team == Team.Red)
        {
            layers = new string[] { WALKABLE_LAYER, RED_WALKABLE_LAYER };
        }
        else
        {
            layers = new string[] { WALKABLE_LAYER, BLUE_WALKABLE_LAYER };
        }

        int mask = LayerMask.GetMask(layers);

        Ray ray = controller.MainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 80f, mask))
        {
            _destiantion = hit.point;
            agent.SetDestination(_destiantion);
            _hasPath = true;
            OnStartMoving.Invoke();
        }

      
    }

    private void HandleRangedAttack(Vector2 direction)
    {
        StopMoving();
        TurnTowards(direction);
    }

    private void TurnTowardsMoveDirection()
    {
        if (!_hasPath) return; 

        Vector2 lookDirection = new Vector2(agent.nextPosition.x - transform.position.x, agent.nextPosition.z - transform.position.z);
        TurnTowards(lookDirection);
    }

    private void Move()
    {
        if (!_hasPath) return; 

        if (WasDestinationReached())
        {
            StopMoving();
            return;
        }

        transform.position = agent.nextPosition;
    }

    private void Respawn(DeathData _)
    {
        StopMoving();
        agent.Warp(controller.SpawnPoint);
        transform.position = controller.SpawnPoint;
    }

    private void StopMoving()
    {
        _hasPath = false;
        agent.ResetPath();
        agent.velocity = Vector3.zero;
        OnStopMoving.Invoke();
    }

    private void TurnTowards(Vector2 direction)
    {
        if (direction == Vector2.zero) return;
        visualsParent.transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y), Vector3.up);
    }

    private bool WasDestinationReached()
    {
        Vector3 playerGroundPosition = new Vector3(transform.position.x, _destiantion.y, transform.position.z);
        return OFFSET >= (_destiantion - playerGroundPosition).sqrMagnitude;
    }

}
