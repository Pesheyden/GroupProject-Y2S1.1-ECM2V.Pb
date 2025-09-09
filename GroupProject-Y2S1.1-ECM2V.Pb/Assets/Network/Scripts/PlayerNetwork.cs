using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Test_Network
{
    public class PlayerNetwork : NetworkBehaviour
    {
        [SerializeField] private CharacterController _charCon;
        [SerializeField] private PlayerInput _playerInput;

        private InputAction _move;
        private Vector2 _moveInput;

        [SerializeField] private float _moveSpeed = 5f;

        private void Awake()
        {
            FindActions();
        }

        private void FindActions()
        {
            _move = _playerInput.actions.FindAction("Move");
        }

        private void OnEnable()
        {
            _move.performed += SetMoveInput;
            _move.canceled += SetMoveInput;
        }

        private void OnDisable()
        {
            _move.performed -= SetMoveInput;
            _move.canceled -= SetMoveInput;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (!IsOwner) _playerInput.enabled = false;
        }

        private void Update()
        {
            if (!IsOwner) return;

            Move();
        }

        private void SetMoveInput(InputAction.CallbackContext context)
        {
            if (!IsOwner) return;

            _moveInput = context.ReadValue<Vector2>();
        }

        private void Move()
        {
            if (!IsOwner) return;

            Vector3 direction = new Vector3(_moveInput.x, 0f, _moveInput.y);
            _charCon.Move(Time.deltaTime * _moveSpeed * direction);
        }
    }

}
