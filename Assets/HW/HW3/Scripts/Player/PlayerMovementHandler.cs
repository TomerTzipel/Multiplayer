using Fusion;
using HW3;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerMovementHandler : NetworkBehaviour
{
    private const float OFFSET = 0.25f;
    private const string WALKABLE_LAYER_MASK = "Walkable";

    [SerializeField] private PlayerController controller;
    [SerializeField] private AnimationStateHandler animationStateHandler;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GameObject visualsParent;

    private Vector3 _destiantion;

    private bool _hasPath = false;

    private bool _turnQueued = false;
    private Vector2 _turnDirectionCache;
    private bool _stopQueued = false;
    public override void Spawned()
    {
        agent.enabled = true;
        agent.speed = controller.Settings.BaseSpeed;
        agent.Warp(transform.position);
        agent.updatePosition = false;
        agent.updateRotation = false;
    }

    public void StartMoving()
    {  
        int groundLayerMask = LayerMask.GetMask(WALKABLE_LAYER_MASK);
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 80f, groundLayerMask))
        {
            _destiantion = hit.point;
            Debug.DrawLine(ray.origin, _destiantion, Color.green, 5f);
            agent.SetDestination(_destiantion);
            _hasPath = true;
            animationStateHandler.StartMoveAnimation();
        }
    }
    public void Turn(Vector2 direction)
    {
        _turnQueued = true;
        _turnDirectionCache = direction;
    }
    public void Stop()
    {
        _stopQueued = true;
    }
    public void FixedUpdateNetworkCall()
    {
        HandleTurning();
        HandleMoving();  
    }

    private void HandleTurning()
    {
        if (_turnQueued)
        {
            TurnTowards(_turnDirectionCache);
            _turnQueued = false;
            return;
        }

        if (!_hasPath) { return; }

        Vector2 lookDirection = new Vector2(agent.nextPosition.x - transform.position.x, agent.nextPosition.z - transform.position.z);
        TurnTowards(lookDirection);
    }

    private void HandleMoving()
    {
        if (_stopQueued)
        {
            StopMoving();
            _stopQueued = false;
            return;
        }

        if (!_hasPath) { return; }

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
        animationStateHandler.StopMoveAnimation();
    }

    private void TurnTowards(Vector2 direction)
    {   
        visualsParent.transform.rotation = Quaternion.LookRotation(new Vector3(direction.x,0,direction.y), Vector3.up);
    }

    private bool WasDestinationReached()
    {
        Vector3 playerGroundPosition = new Vector3(transform.position.x, _destiantion.y, transform.position.z);
        return OFFSET >= (_destiantion - playerGroundPosition).sqrMagnitude;
    }
}
