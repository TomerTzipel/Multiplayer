using Fusion;
using UnityEngine;

public class CharacterHealthHandler : NetworkBehaviour
{
    private const string PROJECTILE_TAG = "Projectile";

    [SerializeField] private PlayerCharacterController controller;
    [SerializeField] private BarHandler HealthBar;
    [SerializeField] private ParticleSystem BloodEffect;

    private int _maxHealth;

    [Networked, OnChangedRender(nameof(HealthChanged))] public int Health { get; set; }

    public override void Spawned()
    {
        _maxHealth = controller.Settings.MaxHealth;
        HealthBar.UpdateSlider(1f, _maxHealth, _maxHealth);
    }

    private void TakeDamage(int damage)
    {
        Health -= damage;

        //TODO: Handle Death
    }
    private void HealthChanged()
    {
        //TODO: Check if the health was lowered and only then play the blood effect
        BloodEffect.Play();
        float hpPercentage = (((float)Health) / _maxHealth);
        HealthBar.UpdateSlider(hpPercentage, Health, _maxHealth);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(PROJECTILE_TAG)) return;

        if(!HasStateAuthority) return;

        ProjectileHandler projectile = other.GetComponent<ProjectileHandler>();

        if (projectile.OwnerName == controller.PlayerData.Name) return;

        TakeDamage(projectile.Damage);
        Runner.Despawn(projectile.Object);
    }

}
