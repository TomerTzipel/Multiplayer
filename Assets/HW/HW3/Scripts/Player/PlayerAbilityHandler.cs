using Fusion;
using HW2;
using HW3;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAbilityHandler : NetworkBehaviour
{
    [SerializeField] private PlayerController controller;

    private bool _rangedAttackQueued = false;
    private Vector2 _directionCache;

    public override void FixedUpdateNetwork()
    {
        if (_rangedAttackQueued) HandleRangedAttack();

    }

    public void RangedAttack(Vector2 direction)
    {
        _rangedAttackQueued = true;
        _directionCache = direction;
    }

    private void HandleRangedAttack()
    {
        Runner.Spawn(controller.Settings.ProjectilePrefab,transform.position, onBeforeSpawned: InitializeProjectile);
        _rangedAttackQueued = false;
    }

    private void InitializeProjectile(NetworkRunner runner, NetworkObject obj)
    {
        Vector3 direction = new Vector3(_directionCache.x, transform.position.y, _directionCache.y);
        obj.GetComponent<ProjectileHandler>().NetworkInitialize(controller.Settings.Damage, direction);
    }
}
