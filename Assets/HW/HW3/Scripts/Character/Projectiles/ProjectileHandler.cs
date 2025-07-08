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
            Debug.Log("Proj" + Damage);
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
}


