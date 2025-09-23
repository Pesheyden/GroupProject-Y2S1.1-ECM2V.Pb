using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace MultiPlayer.Player
{
    [DefaultExecutionOrder(100)]
    [RequireComponent(typeof(Player))]
    public class RagdollHandler : NetworkBehaviour
    {
        [SerializeField] private Player _player;

        [Space]
        [SerializeField] private List<Collider> _colliders = new();
        [SerializeField] private Collider _hipsCollider;
        private Vector3 _hipsLocalPosition;

        [Space]
        [SerializeField] private float _minRagdollTime = 2f;
        [SerializeField] private float _maxRagdollTime = 20f;
        private float _ragdollTime;
        [SerializeField] private float _ragdollTimePerInput = 0.2f;

        [Space]
        [SerializeField] private float _impulseThreshold = 10f;
        [SerializeField] private float _linearVelocityThreshold = 1f;

        private void Reset()
        {
            _player = GetComponent<Player>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsOwner)
            {
                DisableRagdoll();
                enabled = false;
            }

            NetworkAwake();
        }

        private void NetworkAwake()
        {
            if (!_player) _player = GetComponent<Player>();

            _hipsLocalPosition = _hipsCollider.transform.localPosition;
        }

        private void OnEnable()
        {
            SubscribeToInputActions();
        }

        private void OnDisable()
        {
            UnsubscribeFromInputActions();

            DisableRagdoll();
        }

        private void SubscribeToInputActions()
        {
            InputHandler.Instance.OnAnyPerformed += AddRagdollTime;
        }

        private void UnsubscribeFromInputActions()
        {
            InputHandler.Instance.OnAnyPerformed -= AddRagdollTime;
        }

        public bool IsRagdoll { get; private set; }
        public void EnableRagdoll()
        {
            IsRagdoll = true;

            foreach (Collider collider in _colliders)
            {
                collider.gameObject.SetActive(true);

                collider.enabled = true;

                Rigidbody rigidbody = collider.attachedRigidbody;
                if (rigidbody)
                {
                    rigidbody.isKinematic = false;
                    rigidbody.linearVelocity = _player.Rigidbody.linearVelocity;
                    rigidbody.angularVelocity = Vector3.zero;
                }
            }

            _player.PlayerCamera.SetPerspective(PlayerCamera.Perspective.ThirdPerson);

            _player.Collider.enabled = false;

            _player.Rigidbody.linearVelocity = Vector3.zero;
            _player.Rigidbody.angularVelocity = Vector3.zero;
            _player.Rigidbody.isKinematic = true;

            if (_player.Animator) _player.Animator.enabled = false;

            _player.PlayerController.enabled = false;

            if (_player.ArmController) _player.ArmController.enabled = false;

            StartRagdoll();
        }

        public void DisableRagdoll()
        {
            StopRagdoll();

            IsRagdoll = false;

            _player.PlayerCamera.SetPerspective(PlayerCamera.Perspective.FirstPerson);

            _player.Collider.enabled = true;

            _player.Rigidbody.isKinematic = false;
            _player.Rigidbody.linearVelocity = Vector3.zero;
            _player.Rigidbody.angularVelocity = Vector3.zero;

            if (_player.Animator) _player.Animator.enabled = true;

            _player.PlayerController.enabled = true;

            if (_player.ArmController) _player.ArmController.enabled = true;

            foreach (Collider collider in _colliders)
            {
                Rigidbody rigidbody = collider.attachedRigidbody;
                if (rigidbody)
                {
                    rigidbody.linearVelocity = Vector3.zero;
                    rigidbody.angularVelocity = Vector3.zero;
                    rigidbody.isKinematic = true;
                }

                collider.enabled = false;
            }

            ResyncPosition();
        }

        private void ResyncPosition()
        {
            Transform hips = _hipsCollider.transform;

            transform.position = hips.position;
            //hips.TransformPoint(-_hipsLocalPosition);

            hips.localPosition = _hipsLocalPosition;
            hips.rotation = Quaternion.identity;
        }

        private void StartRagdoll()
        {
            StopRagdoll();

            StartCoroutine(_ragdoll = Ragdoll());
        }

        private void StopRagdoll()
        {
            if (_ragdoll != null)
            {
                StopCoroutine(_ragdoll);
                _ragdoll = null;
            }
        }

        private IEnumerator _ragdoll;
        private IEnumerator Ragdoll()
        {
            _ragdollTime = 0f;
            while (_ragdollTime < _maxRagdollTime)
            {
                yield return new WaitForFixedUpdate();
                _ragdollTime += Time.fixedDeltaTime;

                if (_ragdollTime >= _minRagdollTime)
                {
                    Rigidbody rigidbody = _hipsCollider.attachedRigidbody;
                    if (rigidbody && rigidbody.linearVelocity.magnitude < _linearVelocityThreshold) DisableRagdoll();
                }
            }

            DisableRagdoll();
        }

        private void AddRagdollTime()
        {
            if (!IsRagdoll) return;

            _ragdollTime += _ragdollTimePerInput;
        }

        public void AddImpulse(Vector3 impulse)
        {
            if (impulse.magnitude > _impulseThreshold) EnableRagdoll();

            Rigidbody rigidbody = _hipsCollider.attachedRigidbody;
            if (rigidbody) rigidbody.AddForce(impulse, ForceMode.Impulse);
        }
    }

}
