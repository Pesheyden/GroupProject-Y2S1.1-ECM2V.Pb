using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace MultiPlayer.Player
{
    [DefaultExecutionOrder(100)]
    [RequireComponent(typeof(Player))]
    public class SquishHandler : NetworkBehaviour
    {
        [SerializeField] private Player _player;

        [Space]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Transform _modelRoot;

        [Space]
        [SerializeField] private float _minSquishTime = 2f;
        [SerializeField] private float _maxSquishTime = 4f;
        private float _squishTime;
        [SerializeField] private float _squishTimePerInput = 0.2f;

        [Space]
        [SerializeField] private float _dotThreshold = 0.9f;
        [SerializeField] private float _impulseThreshold = 5f;

        private Dictionary<Collider, ContactPoint> _contacts;

        private void Reset()
        {
            _player = GetComponent<Player>();
        }

        private void OnEnable()
        {
            SubscribeToInputActions();
        }

        private void OnDisable()
        {
            UnsubscribeFromInputActions();
        }

        private void SubscribeToInputActions()
        {
            InputHandler.Instance.OnAnyPerformed += AddSquishTime;
        }

        private void UnsubscribeFromInputActions()
        {
            InputHandler.Instance.OnAnyPerformed -= AddSquishTime;
        }

        public bool AddContact(Collision collision)
        {
            _contacts ??= new();

            ContactPoint contactPoint = collision.GetContact(0);

            if (_contacts.ContainsKey(collision.collider)) _contacts[collision.collider] = contactPoint;
            else _contacts.Add(collision.collider, contactPoint);

            return TrySquish();
        }

        public void RemoveContact(Collision collision)
        {
            _contacts ??= new();

            if (_contacts.ContainsKey(collision.collider)) _contacts.Remove(collision.collider);
        }

        private bool TrySquish()
        {
            foreach (var pair in _contacts)
            {
                foreach (var pair1 in _contacts)
                {
                    if (pair.Key == pair1.Key) continue;

                    float dot = Vector3.Dot(pair.Value.normal, pair1.Value.normal);
                    if (dot <= -Mathf.Abs(_dotThreshold))
                    {
                        if (pair.Value.impulse.magnitude >= _impulseThreshold)
                        {
                            Collider surface = pair1.Value.impulse.magnitude > pair.Value.impulse.magnitude ? pair.Key : pair1.Key;

                            EnableSquish(surface);
                            return true;
                        }
                        else if (pair1.Value.impulse.magnitude >= _impulseThreshold)
                        {
                            EnableSquish(pair.Key);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool IsSquished { get; private set; }
        private void EnableSquish(Collider surface)
        {
            IsSquished = true;

            _player.PlayerCamera.SetTarget(transform);
            _player.PlayerCamera.SetPerspective(PlayerCamera.Perspective.ThirdPerson);

            _modelRoot.gameObject.SetActive(false);

            _player.Collider.enabled = false;

            _player.Rigidbody.isKinematic = true;
            _player.Rigidbody.linearVelocity = Vector3.zero;
            _player.Rigidbody.angularVelocity = Vector3.zero;

            if (_player.Animator) _player.Animator.enabled = false;

            _player.PlayerController.enabled = false;

            if (_player.RagdollHandler) _player.RagdollHandler.enabled = false;

            if (_player.ArmController) _player.ArmController.enabled = false;

            _spriteRenderer.gameObject.SetActive(true);
            _spriteRenderer.color = _player.color.Value;

            if (surface.GetComponent<NetworkObject>()) transform.SetParent(surface.transform);

            ContactPoint contactPoint = _contacts[surface];
            transform.position = contactPoint.point + 0.01f * contactPoint.normal;
            transform.up = contactPoint.normal;

            StartSquish();
        }

        private void DisableSquish()
        {
            StopSquish();

            IsSquished = false;

            _spriteRenderer.gameObject.SetActive(false);

            transform.SetParent(null);
            transform.rotation = Quaternion.identity;

            _player.PlayerCamera.SetTarget(_player.playerCameraTarget);
            _player.PlayerCamera.SetPerspective(PlayerCamera.Perspective.FirstPerson);

            _modelRoot.gameObject.SetActive(true);

            _player.Collider.enabled = true;

            _player.Rigidbody.isKinematic = false;
            _player.Rigidbody.linearVelocity = Vector3.zero;
            _player.Rigidbody.angularVelocity = Vector3.zero;

            if (_player.Animator) _player.Animator.enabled = true;

            _player.PlayerController.enabled = true;

            if (_player.RagdollHandler) _player.RagdollHandler.enabled = true;

            if (_player.ArmController) _player.ArmController.enabled = true;
        }

        private void StartSquish()
        {
            StopSquish();

            StartCoroutine(_squish = Squish());
        }

        private void StopSquish()
        {
            if (_squish != null)
            {
                StopCoroutine(_squish);
                _squish = null;
            }
        }

        private IEnumerator _squish;
        private IEnumerator Squish()
        {
            _squishTime = 0f;
            while (_squishTime < _maxSquishTime)
            {
                yield return null;
                _squishTime += Time.deltaTime;
            }

            DisableSquish();
        }

        private void AddSquishTime()
        {
            _squishTime += _squishTimePerInput;
        }
    }

}
