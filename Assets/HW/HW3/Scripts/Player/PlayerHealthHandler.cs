using Fusion;
using HW3;
using Unity.Properties;
using UnityEngine;

public class PlayerHealthHandler : NetworkBehaviour
{
    [SerializeField] private PlayerController controller;
    [Networked] public int Health { get; set; }
}
