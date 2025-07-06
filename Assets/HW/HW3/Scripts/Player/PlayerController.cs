using Fusion;
using TMPro;
using UnityEngine;

namespace HW3
{
    public class PlayerController : NetworkBehaviour
    {
        [field: SerializeField] public CharacterSettings Settings { get; private set; }

        [SerializeField] private TMP_Text playerNameText;

        [Networked] private string PlayerName { get; set; }
        [Networked] private Color PlayerColor { get; set; }

        private Camera _camera;

        public void Initialize(string playerName,Color playerColor, Camera camera)
        {
            PlayerName = playerName;
            PlayerColor = playerColor;
            _camera = camera;
        }

        public override void Spawned()
        {
            playerNameText.transform.parent.forward = _camera.transform.forward;
            playerNameText.text = PlayerName;
            playerNameText.color = PlayerColor;
        }

    }
}

