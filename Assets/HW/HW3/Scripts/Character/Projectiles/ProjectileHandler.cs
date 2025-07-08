using Fusion;
using UnityEngine;

namespace HW3
{
    public class ProjectileHandler : NetworkBehaviour
    {
        [SerializeField] private ProjectileSettings settings;

        [Networked] private int Damage { get; set; }
        [Networked] private Vector3 Direction { get; set; }

        private float _lifetime;
        public void NetworkInitialize(int damage,Vector3 direction)
        {
            Damage = damage;
            Direction = direction;
        }

        public override void Spawned()
        {
            _lifetime = settings.Lifetime;
            transform.rotation = Quaternion.LookRotation(Direction);
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


