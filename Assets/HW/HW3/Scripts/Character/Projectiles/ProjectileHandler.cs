using Fusion;
using UnityEngine;

namespace HW3
{
    public class ProjectileHandler : NetworkBehaviour
    {
        [SerializeField] private ProjectileSettings settings;

        [Networked] private int Damage { get; set; }

        private float _lifetime;
        public void NetworkInitialize(int damage)
        {
            Damage = damage;
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

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (!HasStateAuthority) return;

            PlayerHealthHandler healthHandler = other.GetComponent<PlayerHealthHandler>();

            //Self Hit Check
            if (healthHandler.HasStateAuthority) return;

            //Feel good damage reduction, will be overriden by RTC if validation failed
            healthHandler.UpdateHealthBarByValue(-Damage);

            healthHandler.TakeDamage_RPC(Damage,transform.position);
            Runner.Despawn(Object);
        }
    }
}


