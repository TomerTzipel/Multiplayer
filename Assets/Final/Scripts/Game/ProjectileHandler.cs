using Fusion;
using System.Collections;
using UnityEngine;
public struct ProjectileData : INetworkStruct
{
    public PlayerData PlayerData;
    public int Damage;
    public int CritChance;
    public float Speed;
    public float Lifetime;
}
public struct CombatData 
{
    public string AttackerName;
    public int Damage;
    public bool WasCrit;
}

public class ProjectileHandler : NetworkBehaviour
{
    private const float DESPAWN_DELAY = 5f;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject visuals;

    [Networked] private ProjectileData _projectileData { get; set; }
    private float _lifetime;
    private bool _canHit = true;
    private bool _isDespawning = false;
    public void NetworkInitialize(ProjectileData data)
    {
        _projectileData = data;
    }

    public override void Spawned()
    {
        _lifetime = _projectileData.Lifetime;
    }
    public override void FixedUpdateNetwork()
    {
        _lifetime -= Runner.DeltaTime;

        if (_lifetime <= 0 && !_isDespawning) 
            HideProjectile();

        if (HasStateAuthority)
            Move();
    }
    private void Move()
    {
        Vector3 move = Runner.DeltaTime * _projectileData.Speed * transform.forward;
        rb.MovePosition(transform.position + move);
    }

    private void HideProjectile()
    {
        _canHit = false;
        visuals.SetActive(false);

        if (HasStateAuthority)
            StartCoroutine(DespawnOnDelay());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_canHit) return;
        if (!other.CompareTag("Player")) return;

        CharacterHealthHandler healthHandler = other.GetComponent<CharacterHealthHandler>();

        if (healthHandler.CompareTeam(_projectileData.PlayerData.Team)) return;

        HideProjectile();
        healthHandler.PlayHitEffect();
        
        if (Runner.IsSharedModeMasterClient)
        {
            CombatData combatData = new CombatData { Damage = _projectileData.Damage,AttackerName = (string)_projectileData.PlayerData.Name};

            int roll = Random.Range(0, 100);

            combatData.WasCrit = roll < _projectileData.CritChance;
            string json = JsonUtility.ToJson(combatData);
            healthHandler.TakeDamage_RPC(json);
        }
    }

    private IEnumerator DespawnOnDelay()
    {
        _isDespawning = true;
        yield return new WaitForSeconds(DESPAWN_DELAY);
        Runner.Despawn(Object);
    }
}
