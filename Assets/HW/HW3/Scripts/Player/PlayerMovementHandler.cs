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

    public override void Spawned()
    {
        agent.speed = controller.Settings.BaseSpeed;
        agent.updatePosition = false;
        agent.updateRotation = false;
    }

    public override void FixedUpdateNetwork()
    {
        if (!_hasPath) { return; }

        if (WasDestinationReached())
        {
            StopMoving();
            return;
        }

        Move();
    }

    private void Move()
    {
        visualsParent.transform.rotation = Quaternion.LookRotation(agent.nextPosition - transform.position);
        transform.position = agent.nextPosition;    
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

    public void StopMoving()
    {
        _hasPath = false;
        agent.ResetPath();
    }

    private bool WasDestinationReached()
    {
        Vector3 playerGroundPosition = new Vector3(transform.position.x, _destiantion.y, transform.position.z);
        return OFFSET >= (_destiantion - playerGroundPosition).sqrMagnitude;
    }
}
