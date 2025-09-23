using Unity.Netcode;
using UnityEngine;

namespace MultiPlayer.Player
{
    [DefaultExecutionOrder(100)]
    public class Player : NetworkBehaviour
    {
        [SerializeField] private PlayerCamera _playerCameraPrefab;
        private PlayerCamera _playerCamera;
        public PlayerCamera PlayerCamera => _playerCamera;
        public Transform playerCameraTarget;

        [Space]
        [SerializeField] private Rigidbody _rigidbody;
        public Rigidbody Rigidbody => _rigidbody;

        [SerializeField] private CapsuleCollider _collider;
        public CapsuleCollider Collider => _collider;

        [SerializeField] private Animator _animator;
        public Animator Animator => _animator;

        [Space]
        [SerializeField] private PlayerController _playerController;
        public PlayerController PlayerController => _playerController;

        [SerializeField] private RagdollHandler _ragdollHandler;
        public RagdollHandler RagdollHandler => _ragdollHandler;

        [SerializeField] private ArmController _armController;
        public ArmController ArmController => _armController;

        [SerializeField] private SquishHandler _squishHandler;
        public SquishHandler SquishHandler => _squishHandler;

        [Space]
        [SerializeField] private PlayerFlagHandler _playerFlagHandler;
        public PlayerFlagHandler PlayerFlagHandler => _playerFlagHandler;

        [Space]
        public NetworkVariable<Color> color = new(Color.gray, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Debug.Log($"{this}: OnNetworkSpawn");
            
            NetworkAwake();
        }

        private void NetworkAwake()
        {
            if (!IsOwner) return;

            _playerCamera = Instantiate(_playerCameraPrefab, playerCameraTarget.position, Quaternion.identity);
            _playerCamera.SetTarget(playerCameraTarget);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!IsOwner) return;

            if (_squishHandler && _squishHandler.enabled)
            {
                if (!_squishHandler.AddContact(collision)) AddImpulseToRagdoll(collision);
            }
            else AddImpulseToRagdoll(collision);
        }

        private void AddImpulseToRagdoll(Collision collision)
        {
            if (!IsOwner) return;

            Vector3 impulse = collision.GetContact(0).impulse;
            _ragdollHandler.AddImpulse(-impulse);
        }

        private void OnCollisionExit(Collision collision)
        {
            if (!IsOwner) return;

            if (_squishHandler && _squishHandler.enabled) _squishHandler.RemoveContact(collision);
        }
    }

}
