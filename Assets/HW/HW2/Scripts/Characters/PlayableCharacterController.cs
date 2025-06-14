using Fusion;
using UnityEngine;

namespace HW2
{
    public class PlayableCharacterController : NetworkBehaviour
    {
        [field: SerializeField] public CharacterSettings Settings { get; private set; }
    }
}

