using Fusion;
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
    private const string PLAYER_TAG = "Player";
    [SerializeField] private Rigidbody rb;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material redTeamMaterial;
    [SerializeField] private Material blueTeamMaterial;
    [SerializeField] private GameObject visuals;

    [Networked] private ProjectileData _projectileData { get; set; }

    private bool _canHit = true;
    [Networked] float _despawnTimer { get; set; }
    [Networked] float _lifetime { get; set; }
    [Networked,OnChangedRender(nameof(HideProjectile))] NetworkBool _isDespawning { get; set; } = false;
    public void NetworkInitialize(ProjectileData data)
    {
        _projectileData = data;
        _lifetime = _projectileData.Lifetime;
    }

    public override void Spawned()
    {  
        meshRenderer.material = blueTeamMaterial;
        if (_projectileData.PlayerData.Team == Team.Red)
        {
            meshRenderer.material = redTeamMaterial;
        }    
    }
    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        _lifetime -= Runner.DeltaTime;

        if (_lifetime <= 0 && !_isDespawning)
            StartDespawn();

        if (_isDespawning)
        {
            _despawnTimer -= Runner.DeltaTime;
            if(_despawnTimer <= 0)
                Runner.Despawn(Object);
        }
        else
        {
            Move();
        }          
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
    }

    private void StartDespawn()
    {
        _isDespawning = true;
        _despawnTimer = DESPAWN_DELAY;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_canHit) return;
        if (!other.CompareTag(PLAYER_TAG)) return;

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
}
