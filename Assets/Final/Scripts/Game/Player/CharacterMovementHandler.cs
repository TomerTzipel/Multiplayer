using Fusion;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CharacterMovementHandler : NetworkBehaviour
{
    private const float OFFSET = 0.25f;
    private const string WALKABLE_LAYER_MASK = "Walkable";

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
    }
    private void OnEnable()
    {
        if (Object == null) return;

        controller.OnRangedAttack += HandleRangedAttack;
    }
    private void OnDisable()
    {
        if (Object == null) return;

        controller.OnRangedAttack -= HandleRangedAttack;
    }
    public void FixedUpdateNetworkCall()
    {
        if (!HasStateAuthority) return;

        GetInput<PlayerInput>(out var input);
 
        if (input.Buttons.IsSet(Buttons.Move))
        {
            StartMoving();
        }

        TurnTowardsMoveDirection();
        Move();
    }

    private void StartMoving()
    {
        int groundLayerMask = LayerMask.GetMask(WALKABLE_LAYER_MASK);
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 80f, groundLayerMask))
        {
            _destiantion = hit.point;
            Debug.DrawLine(ray.origin, _destiantion, Color.green, 5f);
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
