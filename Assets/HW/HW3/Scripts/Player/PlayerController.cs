using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HW3
{
    public class PlayerController : NetworkBehaviour
    {

        [field: SerializeField] public CharacterSettings Settings { get; private set; }
        [SerializeField] private TMP_Text playerNameText;
        [SerializeField] private PlayerMovementHandler movementHandler;

        [Networked] private string PlayerName { get; set; }
        [Networked] private Color PlayerColor { get; set; }

        private InputSystem_Actions inputSystemActions;

        public void Initialize(string playerName,Color playerColor, Camera camera)
        {
            PlayerName = playerName;
            PlayerColor = playerColor;
        }

        public override void Spawned()
        {
            playerNameText.transform.parent.forward = Camera.main.transform.forward;
            playerNameText.text = PlayerName;
            playerNameText.color = PlayerColor;

            if (Object.HasStateAuthority)
            {
                inputSystemActions = new InputSystem_Actions();
                OnEnable();
            }   
        }

        private void OnEnable()
        {
            if (Object == null) return;

            if (Object.HasStateAuthority)
            {
                inputSystemActions.Player.Enable();
                inputSystemActions.Player.MouseMove.started += OnMoveCommand;
            }
            
        }

        private void OnDisable()
        {
            if (Object == null) return;

            if (Object.HasStateAuthority)
            {
                inputSystemActions.Player.MouseMove.started -= OnMoveCommand;
                inputSystemActions.Player.Disable();
            }        
        }

        private void OnMoveCommand(InputAction.CallbackContext context)
        {
            movementHandler.StartMoving();
        }

    }
}

