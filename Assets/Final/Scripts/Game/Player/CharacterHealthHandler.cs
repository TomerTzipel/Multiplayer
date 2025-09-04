using Fusion;
using UnityEngine;
using UnityEngine.Events;
using static Unity.Cinemachine.InputAxisControllerBase<T>;

public struct DeathData: INetworkStruct
{
    public string KillerName;
    public string DeadName;
    public Team DeadTeam;
}

public class CharacterHealthHandler : NetworkBehaviour
{
    [SerializeField] private PlayerCharacterController controller;
    [SerializeField] private BarHandler healthBar;
    [SerializeField] private ParticleSystem bloodEffect;

    public event UnityAction<DeathData> OnDeath;

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
        
        if(Health <=0)
        {
            Health = _maxHealth;
            OnDeath.Invoke(new DeathData { KillerName = AttackerName, DeadName = (string)controller.PlayerData.Name, DeadTeam = controller.PlayerData.Team });
        }
    }
    private void HealthChanged()
    {
        float hpPercentage = (((float)Health) / _maxHealth);
        healthBar.UpdateSlider(hpPercentage, Health, _maxHealth);
    }
}
