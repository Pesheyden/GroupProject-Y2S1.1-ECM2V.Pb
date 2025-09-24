using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace MultiPlayer.Player
{
    [DefaultExecutionOrder(99)]
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

        [Space]
        [SerializeField] private float _bleepInterval = 10f;
        [SerializeField] private float _bleepIntervalDeviation = 2f;

        [Space]
        public ParticleSystemPrefabList particleSystemPrefabList;

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

        private void Update()
        {
            if (_bleep == null) StartBleep();
        }

        private void StartBleep()
        {
            StopBleep();

            StartCoroutine(_bleep = Bleep());
        }

        private void StopBleep()
        {
            if (_bleep != null)
            {
                StopCoroutine(_bleep);
                _bleep = null;
            }
        }

        private IEnumerator _bleep;
        private IEnumerator Bleep()
        {
            while (_bleep != null)
            {
                float interval = _bleepInterval + Random.Range(-_bleepIntervalDeviation, _bleepIntervalDeviation);
                yield return new WaitForSeconds(interval);
            }
        }

        [ServerRpc]
        public void StartBleepSFX_ServerRpc() => StartBleepSFX_ClientRpc();

        [ClientRpc]
        public void StartBleepSFX_ClientRpc()
        {
            var instance = FMODUnity.RuntimeManager.CreateInstance("event:/Bleep");
            instance.set3DAttributes(default);
            instance.start();
        }

        public FMOD.ATTRIBUTES_3D GetATTRIBUTES_3D()
        {
            FMOD.VECTOR forward = new FMOD.VECTOR
            {
                x = transform.forward.x,
                y = transform.forward.y,
                z = transform.forward.z
            };

            FMOD.VECTOR position = new FMOD.VECTOR
            {
                x = transform.position.x,
                y = transform.position.y,
                z = transform.position.z
            };

            FMOD.VECTOR up = new FMOD.VECTOR
            {
                x = transform.up.x,
                y = transform.up.y,
                z = transform.up.z
            };

            FMOD.ATTRIBUTES_3D attributes_3d = new FMOD.ATTRIBUTES_3D
            {
                forward = forward,
                position = position,
                up = up,
                velocity = default
            };

            return attributes_3d;
        }
    }

}
