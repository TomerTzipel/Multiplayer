using Fusion;
using HW3;
using UnityEngine;
using static UnityEngine.Rendering.GPUSort;

public class PlayerHealthHandler : NetworkBehaviour
{
    [SerializeField] private PlayerController controller;
    [SerializeField] private BarHandler HealthBar;

    private int _maxHealth;

    [Networked, OnChangedRender(nameof(HealthChanged))] public int Health { get; set; }

    public override void Spawned()
    {
        _maxHealth = controller.Settings.MaxHealth;
        HealthBar.UpdateSlider(1f, _maxHealth, _maxHealth);
    }


    //Make RPC
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void TakeDamage_RPC(int damage)
    {
        //Add Validation
       
        Health -= damage;
       
        //TODO: Add death management
    }

    private void HealthChanged()
    {
        float hpPercentage = (((float)Health) / _maxHealth);
        HealthBar.UpdateSlider(hpPercentage, Health, _maxHealth);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Add effect
    }
}
