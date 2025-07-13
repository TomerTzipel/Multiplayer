using Fusion;
using System.Collections;
using UnityEngine;

namespace HW3
{
    public class ProjectileHandler : NetworkBehaviour
    {
        private const string PLAYER_TAG = "Player";

        [SerializeField] private ProjectileSettings settings;

        [Networked] private int Damage { get; set; }

        private float _lifetime;
        private bool _destroyFlag = false;

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
            if (_destroyFlag) return;

            if (!other.CompareTag(PLAYER_TAG)) return;

            if (!HasStateAuthority) return;

            PlayerHealthHandler healthHandler = other.GetComponent<PlayerHealthHandler>();

            //Self Hit Check
            if (healthHandler.HasStateAuthority) return;

            //Feel good damage reduction, will be overriden by RTC if validation failed
            healthHandler.UpdateHealthBarByValue(-Damage);

            healthHandler.TakeDamage_RPC(Damage,transform.position);
            _destroyFlag = true;
            StartCoroutine(DespawnOnDelay(0.1f));
            
        }

        private IEnumerator DespawnOnDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            Runner.Despawn(Object);
        }
    }
}


