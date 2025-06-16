using Fusion;
using TMPro;
using UnityEngine;

namespace HW2
{
    public class PlayableCharacterController : NetworkBehaviour
    {
        [field: SerializeField] public CharacterSettings Settings { get; private set; }

        [SerializeField] private TMP_Text playerNameText;

        [Networked] public string PlayerName { get; private set; }

        public void Initialize(string playerName)
        {
            PlayerName = playerName;
        }

        public override void Spawned()
        {
            //Not really sure how to inject the camera before spawned, except for a singleton in the scene holding it as aA serializedField
            playerNameText.transform.parent.forward = Camera.main.transform.forward;
            playerNameText.text = PlayerName;
        }

    }
}

