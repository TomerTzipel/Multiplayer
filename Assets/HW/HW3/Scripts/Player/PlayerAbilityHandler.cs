using Fusion;
using HW2;
using HW3;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAbilityHandler : NetworkBehaviour
{
    [SerializeField] private PlayerController controller;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private AnimationStateHandler animationStateHandler;

    private bool _rangedAttackQueued = false;

    public override void FixedUpdateNetwork()
    {
        if (_rangedAttackQueued) HandleRangedAttack();

    }

    public void RangedAttack()
    {
        _rangedAttackQueued = true;
    }

    private void HandleRangedAttack()
    {
        animationStateHandler.StartThrowAnimation();
        Runner.Spawn(controller.Settings.ProjectilePrefab, spawnPoint.position, spawnPoint.rotation, onBeforeSpawned: InitializeProjectile);
        _rangedAttackQueued = false;
    }

    private void InitializeProjectile(NetworkRunner runner, NetworkObject obj)
    {
        obj.GetComponent<ProjectileHandler>().NetworkInitialize(controller.Settings.Damage);
    }
}
