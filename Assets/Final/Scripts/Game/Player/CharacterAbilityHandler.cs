using Fusion;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CharacterAbilityHandler : NetworkBehaviour
{
    [SerializeField] private PlayerCharacterController controller;
    [SerializeField] private Transform spawnPoint;
    

    public event UnityAction<Vector2> OnRangedAttack;

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        GetInput<PlayerInput>(out var input);

        Debug.Log("Attack" + input.Buttons.IsSet(Buttons.Attack));

        if (input.Buttons.IsSet(Buttons.Attack))
        {
            Debug.Log("Ranged Attack");
            RangedAttack();
        }
    }
    public void RangedAttack()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 playerScreenPosition = controller.CamerasRef.MainCamera.WorldToScreenPoint(transform.position);
        Vector2 direction = mousePosition - playerScreenPosition;
        OnRangedAttack.Invoke(direction);
        Runner.Spawn(controller.Settings.ProjectilePrefab, spawnPoint.position, spawnPoint.rotation, onBeforeSpawned: InitializeProjectile);
    }

    private void InitializeProjectile(NetworkRunner runner, NetworkObject obj)
    {
        obj.GetComponent<ProjectileHandler>().NetworkInitialize(controller.Settings.Damage,(string)controller.PlayerData.Name);
    }
}
