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
        [SerializeField] private PlayerHealthHandler healthHandler;
        [SerializeField] private PlayerAbilityHandler abilityHandler;

        [Networked] private string PlayerName { get; set; }
        [Networked] private Color PlayerColor { get; set; }

        private InputSystem_Actions inputSystemActions;

        private Camera _mainCamera;

        public void NetworkInitialize(string playerName,Color playerColor, Camera camera)
        {
            PlayerName = playerName;
            PlayerColor = playerColor; 
            healthHandler.Health = Settings.MaxHealth;
        }

        public override void Spawned()
        {
            _mainCamera = Camera.main;//no other choice... :(
            playerNameText.transform.parent.forward = _mainCamera.transform.forward;
            playerNameText.text = PlayerName;
            playerNameText.color = PlayerColor;

            if (Object.HasStateAuthority)
            {
                inputSystemActions = new InputSystem_Actions();
                OnEnable();
            }   
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            OnDisable();
        }

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority) return;

            movementHandler.FixedUpdateNetworkCall();
            abilityHandler.FixedUpdateNetworkCall();
        }

        private void OnEnable()
        {
            if (Object == null) return;

            if (Object.HasStateAuthority)
            {
                inputSystemActions.Player.Enable();
                inputSystemActions.Player.MouseMove.performed += OnMoveCommand;
                inputSystemActions.Player.RangedAttack.performed += OnRangedAttackCommand;
            }  
        }

        private void OnDisable()
        {
            if (Object == null) return;

            if (Object.HasStateAuthority)
            {
                inputSystemActions.Player.MouseMove.performed -= OnMoveCommand;
                inputSystemActions.Player.RangedAttack.performed -= OnRangedAttackCommand;
                inputSystemActions.Player.Disable();
            }        
        }

        private void OnMoveCommand(InputAction.CallbackContext context)
        {
            movementHandler.StartMoving();
        }

        private void OnRangedAttackCommand(InputAction.CallbackContext context)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector2 playerScreenPosition = _mainCamera.WorldToScreenPoint(transform.position);
            Vector2 direction = mousePosition - playerScreenPosition;
            movementHandler.Stop();
            movementHandler.Turn(direction);

            abilityHandler.RangedAttack();
        }
    }
}

