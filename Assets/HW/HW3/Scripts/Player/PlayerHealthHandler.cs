using Fusion;
using HW3;
using UnityEngine;

public class PlayerHealthHandler : NetworkBehaviour
{
    [SerializeField] private PlayerController controller;
    [Networked, OnChangedRender(nameof(HealthChanged))] public int Health { get; set; }

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
        Debug.Log(Health);
        //Update Health UI
    }

    private void OnTriggerEnter(Collider other)
    {
        //Add effect
    }
}
