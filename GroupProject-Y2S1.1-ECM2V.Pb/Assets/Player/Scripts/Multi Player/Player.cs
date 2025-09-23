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
        [SerializeField] private Transform _playerCameraTarget;

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

            _playerCamera = Instantiate(_playerCameraPrefab, _playerCameraTarget.position, Quaternion.identity);
            _playerCamera.SetTarget(_playerCameraTarget);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!IsOwner) return;

            AddImpulseToRagdoll(collision);
        }

        private void AddImpulseToRagdoll(Collision collision)
        {
            if (!IsOwner) return;

            Vector3 impulse = collision.contacts[0].impulse;
            _ragdollHandler.AddImpulse(-impulse);
        }
    }

}
