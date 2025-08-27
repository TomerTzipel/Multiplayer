using Fusion;
using UnityEngine;

public class ProjectileHandler : NetworkBehaviour
{
    [SerializeField] private ProjectileSettings settings;

    [Networked] public int Damage { get; set; }
    [Networked] public NetworkString<_8> OwnerName { get; set; }

    private float _lifetime;

    public void NetworkInitialize(int damage,string name)
    {
        Damage = damage;
        OwnerName = name;
    }

    public override void Spawned()
    {
        _lifetime = settings.Lifetime;
    }
    public override void FixedUpdateNetwork()
    {
        _lifetime -= Runner.DeltaTime;
        if (_lifetime <= 0) Runner.Despawn(Object);

        if (HasStateAuthority)
        {
            Move();
        }
    }
    private void Move()
    {
        transform.Translate(Runner.DeltaTime * settings.Speed * Vector3.forward);
    }
}
