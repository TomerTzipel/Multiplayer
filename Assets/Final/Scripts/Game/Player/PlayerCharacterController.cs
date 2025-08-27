using Fusion;
using UnityEngine;

public class PlayerCharacterController : NetworkBehaviour
{
    [field: SerializeField] public CharacterSettings Settings { get; private set; }
    [Networked] public NetworkString<_8> OwnerName { get; set; }
}
