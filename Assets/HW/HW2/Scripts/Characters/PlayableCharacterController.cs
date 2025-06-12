using Fusion;
using UnityEngine;

public class PlayableCharacterController : NetworkBehaviour
{
    [field:SerializeField] public CharacterSettings Settings {  get; private set; }
}
