using Fusion;
using HW3;
using UnityEngine;


public class PlayerHealthHandler : NetworkBehaviour
{
    private const string PROJECTILE_TAG = "Projectile";
    private const float VALIDATION_DIST = 1f;

    [SerializeField] private PlayerController controller;
    [SerializeField] private BarHandler HealthBar;
    [SerializeField] private ParticleSystem BloodEffect;

    private int _maxHealth;

    [Networked, OnChangedRender(nameof(HealthChanged))] public int Health { get; set; }

    public override void Spawned()
    {
        _maxHealth = controller.Settings.MaxHealth;
        HealthBar.UpdateSlider(1f, _maxHealth, _maxHealth);
    }


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void TakeDamage_RPC(int damage,Vector3 projectilePosition, RpcInfo info = default)
    {

        if(VALIDATION_DIST >= (transform.position - projectilePosition).sqrMagnitude)
        {
            Debug.Log("Hit failed validation");
            FailedHitValidation_RPC(info.Source);
            return;
        }

        TakeDamage(damage);     
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;

        //We will handle death for the real game, as for now the point is to see the multiplayer works properly
    }

    //Used for the local Health bar change
    public void UpdateHealthBarByValue(int value)
    {
        int hp = Health + value;
        float hpPercentage = (((float)hp) / _maxHealth);
        HealthBar.UpdateSlider(hpPercentage, hp, _maxHealth);
    }

    //Used to reverse the local health bar changeto keep the UI synced
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void FailedHitValidation_RPC([RpcTarget] PlayerRef target)
    {
        Debug.Log("Hit failed validation");
        UpdateHealthBarByValue(0);
    }

    private void HealthChanged()
    {
        float hpPercentage = (((float)Health) / _maxHealth);
        HealthBar.UpdateSlider(hpPercentage, Health, _maxHealth);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (!other.CompareTag(PROJECTILE_TAG)) return;

         ProjectileHandler projectile = other.GetComponent<ProjectileHandler>();

         //Self hit 
         if (projectile.HasStateAuthority && Object.HasStateAuthority) return;

         //I hit someone else
         if (projectile.HasStateAuthority && !Object.HasStateAuthority)
         {
             PlayHitEffect(projectile);
             return;
         }

         //Someone else hit me
         if (!projectile.HasStateAuthority && Object.HasStateAuthority)
         {
             PlayHitEffect(projectile);
             return;
         }

         //Someone else hit someone else
         if (!projectile.HasStateAuthority && !Object.HasStateAuthority)
         {
             //Someone else hit themselves
             if (projectile.Object.StateAuthority == Object.StateAuthority) return;
             PlayHitEffect(projectile);
             
         }
    }

    private void PlayHitEffect(ProjectileHandler projectile) 
    {
        projectile.TurnOff();
        BloodEffect.Play();
    }
}
