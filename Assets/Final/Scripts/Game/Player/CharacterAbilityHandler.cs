using Fusion;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CharacterAbilityHandler : NetworkBehaviour
{
    [SerializeField] private PlayerCharacterController controller;
    [SerializeField] private Transform spawnPoint;
    

    public event UnityAction<Vector2> OnBasicAttack;

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        if(GetInput<PlayerInput>(out var input))
        {
            
            if (input.Buttons.IsSet(Buttons.BasicAttack))
            {
                if (Runner.IsForward)
                {
                    OnBasicAttack.Invoke(input.Direction);
                    
                }
                Debug.Log("Ranged Attack");
                BasicAttack(input.Direction);
            }
        }      
    }

    public Vector2 GetBasicAttackDirection()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 playerScreenPosition = controller.MainCamera.WorldToScreenPoint(transform.position);
        return mousePosition - playerScreenPosition;
    }

    private void BasicAttack(Vector2 direction)
    {   
        Runner.Spawn(controller.Settings.ProjectilePrefab, spawnPoint.position, spawnPoint.rotation, onBeforeSpawned: InitializeProjectile);
    }

    private void InitializeProjectile(NetworkRunner runner, NetworkObject obj)
    {
        obj.GetComponent<ProjectileHandler>().NetworkInitialize(controller.Settings.Damage,(string)controller.PlayerData.Name);
    }
}
