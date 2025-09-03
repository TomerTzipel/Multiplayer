using Fusion;
using UnityEngine;

public class CharacterHealthHandler : NetworkBehaviour
{
    [SerializeField] private PlayerCharacterController controller;
    [SerializeField] private BarHandler healthBar;
    [SerializeField] private ParticleSystem bloodEffect;

    private int _maxHealth;

    [Networked, OnChangedRender(nameof(HealthChanged))] public int Health { get; set; }

    public override void Spawned()
    {
        _maxHealth = controller.Settings.MaxHealth;
        healthBar.UpdateSlider(1f, _maxHealth, _maxHealth);
    }

    public bool CompareTeam(Team team)
    {
        return team == controller.PlayerData.Team;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void TakeDamage_RPC(string combatJson, RpcInfo info = default)
    {
        CombatData combatData = JsonUtility.FromJson<CombatData>(combatJson);
        int damage = combatData.Damage;

        if (combatData.WasCrit) 
            damage *= 2;

        TakeDamage(damage, combatData.AttackerName);
    }

    public void PlayHitEffect()
    {
        bloodEffect.Play();
    }

    private void TakeDamage(int damage,string AttackerName)
    {
        Health -= damage;
        //TODO: Handle Death
    }
    private void HealthChanged()
    {
        float hpPercentage = (((float)Health) / _maxHealth);
        healthBar.UpdateSlider(hpPercentage, Health, _maxHealth);
    }
}
