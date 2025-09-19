using UnityEngine;

namespace SinglePlayer.Player
{
    [DefaultExecutionOrder(100)]
    public class Player : MonoBehaviour
    {
        [SerializeField] private PlayerCamera _playerCamera;
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
        [SerializeField] private Color _color = Color.cyan;
        public Color Color => _color;

        private void Awake()
        {
            _playerCamera.SetTarget(_playerCameraTarget);
        }

        private void OnCollisionEnter(Collision collision)
        {
            AddImpulseToRagdoll(collision);
        }

        private void AddImpulseToRagdoll(Collision collision)
        {
            Vector3 impulse = collision.contacts[0].impulse;
            _ragdollHandler.AddImpulse(-impulse);
        }
    }

}
