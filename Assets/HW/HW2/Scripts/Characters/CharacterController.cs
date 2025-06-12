using Fusion;
using UnityEngine;

public class CharacterController : NetworkBehaviour
{
    [field:SerializeField] public CharacterSettings Settings {  get; private set; }
}
