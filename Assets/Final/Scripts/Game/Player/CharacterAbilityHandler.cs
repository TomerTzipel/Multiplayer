using Fusion;
using UnityEngine;

public class CharacterAbilityHandler : NetworkBehaviour
{
    [SerializeField] private PlayerCharacterController controller;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private AnimationStateHandler animationStateHandler;

    [Networked] private bool _rangedAttackQueued { get; set; } = false;

    public void FixedUpdateNetworkCall()
    {
        if (_rangedAttackQueued) HandleRangedAttack();

    }

    public void DoRangedAttack()
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
        obj.GetComponent<ProjectileHandler>().NetworkInitialize(controller.Settings.Damage,(string)controller.OwnerName);
    }
}
