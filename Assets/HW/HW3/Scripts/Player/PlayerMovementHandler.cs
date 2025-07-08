using Fusion;
using HW3;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerMovementHandler : NetworkBehaviour
{
    private const float OFFSET = 0.1f;
    private const string WALKABLE_LAYER_MASK = "Walkable";

    [SerializeField] private PlayerController controller;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GameObject visualsParent;

    private Vector3 _destiantion;

    private bool _hasPath = false;

    private bool _turnQueued = false;
    private Vector2 _turnDirectionCache;
    private bool _stopQueued = false;
    public override void Spawned()
    {
        agent.speed = controller.Settings.BaseSpeed;
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
    public override void FixedUpdateNetwork()
    {
        HandleMoving();
        HandleTurning();
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

        Vector2 direction = new Vector2(agent.nextPosition.x, agent.nextPosition.z);
        TurnTowards(direction);
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
    }

    private void TurnTowards(Vector2 direction)
    {
        visualsParent.transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, transform.position.y, direction.y) - transform.position);
    }

    private bool WasDestinationReached()
    {
        Vector3 playerGroundPosition = new Vector3(transform.position.x, _destiantion.y, transform.position.z);
        return OFFSET >= (_destiantion - playerGroundPosition).sqrMagnitude;
    }
}
