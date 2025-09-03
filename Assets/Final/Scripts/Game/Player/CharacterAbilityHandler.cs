using Fusion;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CharacterAbilityHandler : NetworkBehaviour
{
    [SerializeField] private PlayerCharacterController controller;
    [SerializeField] private Transform spawnPoint;
    
    public event UnityAction<Vector2> OnBasicAttack;

    [Networked] private float _basicAttackCooldown { get; set; } = 0;
    [Networked] private NetworkBool _canAttack { get; set; } = true;
    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        GetInput<PlayerInput>(out var input);

        if (input.Buttons.IsSet(Buttons.BasicAttack) && _canAttack)
        {
            Vector2 direction = GetBasicAttackDirection();
            OnBasicAttack.Invoke(direction);
            BasicAttack(direction);
        }

        _basicAttackCooldown -= Runner.DeltaTime;
        if (_basicAttackCooldown <= 0) _canAttack = true;
    }

    private Vector2 GetBasicAttackDirection()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 playerScreenPosition = controller.MainCamera.WorldToScreenPoint(transform.position);
        return mousePosition - playerScreenPosition;
    }

    private void BasicAttack(Vector2 direction)
    {   
        Runner.Spawn(controller.Settings.ProjectilePrefab, spawnPoint.position, spawnPoint.rotation, onBeforeSpawned: InitializeProjectile);
        _basicAttackCooldown = controller.Settings.AttackSpeed;
        _canAttack = false;
    }

    private void InitializeProjectile(NetworkRunner runner, NetworkObject obj)
    {
        obj.GetComponent<ProjectileHandler>().NetworkInitialize(new ProjectileData { Damage = controller.Settings.Damage,
                                                                                      CritChance = controller.Settings.CritChance, 
                                                                                      PlayerData = controller.PlayerData,
                                                                                      Lifetime = controller.Settings.Range,
                                                                                      Speed = controller.Settings.ProjectileSpeed }); 
    }
}
