using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpectatorHandler : MonoBehaviour
{
    private InputSystem_Actions _inputSystemActions;
    private Vector2 _direction = Vector2.zero;

    private void Awake()
    {
        _inputSystemActions = new InputSystem_Actions();
    }

    public void Init(CinemachineCamera cinemachineCamera)
    {
        cinemachineCamera.LookAt = transform;
        cinemachineCamera.Follow = transform;
    }

    private void OnEnable()
    {
        _inputSystemActions.Player.Move.Enable();
        _inputSystemActions.Player.Move.performed += OnMoveInput;
        _inputSystemActions.Player.Move.canceled += OnMoveInput;
    }

    private void OnDisable()
    {
        _inputSystemActions.Player.Move.Disable();
        _inputSystemActions.Player.Move.performed -= OnMoveInput;
        _inputSystemActions.Player.Move.canceled -= OnMoveInput;
    }
    private void Update()
    {
        if (_direction != Vector2.zero)
        {
            Move();
        }
    }
    private void OnMoveInput(InputAction.CallbackContext context)
    {
        _direction = context.action.ReadValue<Vector2>().normalized;
    }

    private void Move()
    {
        transform.Translate(15f * Time.deltaTime * new Vector3(_direction.x,0, _direction.y));
    }
}
