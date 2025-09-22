using System.Collections;
using Unity.Netcode;
using Unity.Services.Vivox;
using UnityEngine;

namespace MultiPlayer.Player
{
    [DefaultExecutionOrder(100)]
    [RequireComponent(typeof(Player))]
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private Player _player;

        private Vector2 _moveInput;
        [Space][SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _moveAcceleration = 10f;

        [Space]
        [SerializeField] private float _jumpImpulse = 5f;

        [Space]
        [SerializeField] private LayerMask _groundMask = ~0;
        [SerializeField] private float _slopeTolerance = 50f;

        private void Reset()
        {
            _player = GetComponent<Player>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            enabled = IsOwner;

            NetworkAwake();
        }

        private void NetworkAwake()
        {
            if (!_player) _player = GetComponent<Player>();
        }

        private void OnEnable()
        {
            SubscribeToInputActions();
        }

        private void OnDisable()
        {
            UnsubscribeFromInputActions();

            _moveInput = Vector2.zero;
        }

        private void SubscribeToInputActions()
        {
            InputHandler.Instance.OnMovePerformed += SetMoveInput;
            InputHandler.Instance.OnMoveCanceled += SetMoveInput;

            InputHandler.Instance.OnJumpPerformed += Jump;
        }

        private void UnsubscribeFromInputActions()
        {
            InputHandler.Instance.OnMovePerformed -= SetMoveInput;
            InputHandler.Instance.OnMoveCanceled -= SetMoveInput;

            InputHandler.Instance.OnJumpPerformed -= Jump;
        }

        private void SetMoveInput(Vector2 input)
        {
            _moveInput = input;
        }

        private void Update()
        {
            Turn();
            StartCoroutine(VivoxUpdate());
        }

        private IEnumerator VivoxUpdate()
        {
            while (true)
            {
                VivoxService.Instance.Set3DPosition(gameObject,LobbyManager.Instance.JoinedLobby.Name);
                yield return new WaitForSeconds(0.2f);
            }
        }

        private void Turn()
        {
            Vector3 upwards = Vector3.up;
            Vector3 forward = _player.PlayerCamera.Camera.transform.forward;

            Vector3.OrthoNormalize(ref upwards, ref forward);

            transform.rotation = Quaternion.LookRotation(forward, upwards);
        }

        private void FixedUpdate()
        {
            UpdateDisplacement();
            _groundHit = GroundHit();

            Gravity();
            SnapToGround();

            Move();
        }

        private void Move()
        {
            if (!_player.PlayerCamera)
            {
                Debug.LogError($"{this}: missing CameraController reference.");
                return;
            }
            else if (!_player.PlayerCamera.Camera)
            {
                Debug.LogError($"{this}: CameraController missing Camera reference.");
                return;
            }

            Vector3 right = _player.PlayerCamera.Camera.transform.right;
            Vector3 up = Vector3.up;
            Vector3 forward = _player.PlayerCamera.Camera.transform.forward;

            Vector3.OrthoNormalize(ref up, ref forward, ref right);
            if (IsGrounded && IsSlopeTolerable)
            {
                up = _groundHit.normal;
                Vector3.OrthoNormalize(ref up, ref forward, ref right);
                // Makes the movement axes parallel to the ground.
            }

            float xCurrent = GetDirectionalLinearVelocity(right);
            float xTarget = _moveSpeed * _moveInput.x;
            float xDelta = CurrentExceedsTarget(xCurrent, xTarget) ? 0f : xTarget - xCurrent;
            // If current exceeds target, delta equals 0, otherwise, delta equals target minus current.

            float zCurrent = GetDirectionalLinearVelocity(forward);
            float zTarget = _moveSpeed * _moveInput.y;
            float zDelta = CurrentExceedsTarget(zCurrent, zTarget) ? 0f : zTarget - zCurrent;

            Vector3 force = _moveAcceleration * (xDelta * right + zDelta * forward);
            _player.Rigidbody.AddForce(force);

            bool CurrentExceedsTarget(float current, float target)
            {
                return Sign(current) == Sign(target) &&
                    Sign(current) != 0f &&
                    Sign(target) != 0f &&
                    Mathf.Abs(current) > Mathf.Abs(target);
            }

            // Mathf.Sign, but returns 0 if [f] equals 0.
            float Sign(float f)
            {
                return f == 0f ? 0f : f / Mathf.Abs(f);
            }
        }

        private float GetDirectionalLinearVelocity(Vector3 direction)
        {
            Vector3 projection = Vector3.Project(_player.Rigidbody.linearVelocity, direction);
            float dot = Vector3.Dot(projection.normalized, direction.normalized);

            return dot * projection.magnitude;
        }

        private void Jump()
        {
            if (!IsGrounded) return;

            _player.Rigidbody.linearVelocity = new Vector3(_player.Rigidbody.linearVelocity.x, 0f, _player.Rigidbody.linearVelocity.z);
            _player.Rigidbody.AddForce(_jumpImpulse * transform.up, ForceMode.Impulse);

            if (_jumping != null)
            {
                StopCoroutine(_jumping);
                _jumping = null;
            }
            StartCoroutine(_jumping = Jumping());
        }

        private IEnumerator _jumping;
        private IEnumerator Jumping()
        {
            yield return new WaitForSeconds(0.3f);
            _jumping = null;
        }

        private void Gravity()
        {
            if (IsGrounded && IsSlopeTolerable) return;

            _player.Rigidbody.AddForce(Physics.gravity);
        }

        private void SnapToGround()
        {
            if (!IsGrounded) return;
            if (_jumping != null) return;

            _player.Rigidbody.MovePosition(_player.Rigidbody.position + (_groundHit.distance - 0.1f) * -transform.up);

            if ((_player.Rigidbody.linearVelocity.y > 0f && _player.Rigidbody.linearVelocity.y > _displacement.y / Time.fixedDeltaTime) ||
                _player.Rigidbody.linearVelocity.y < 0f && _player.Rigidbody.linearVelocity.y < _displacement.y / Time.fixedDeltaTime)
            {
                _player.Rigidbody.linearVelocity = new Vector3(_player.Rigidbody.linearVelocity.x, _displacement.y / Time.fixedDeltaTime, _player.Rigidbody.linearVelocity.z);
            }
        }

        #region Displacement
        private Vector3 _oldPosition;
        private Vector3 _displacement;
        private void UpdateDisplacement()
        {
            _displacement = _player.Rigidbody.position - _oldPosition;
            _oldPosition = _player.Rigidbody.position;
        }
        #endregion

        #region Grounded
        private RaycastHit _groundHit;
        private RaycastHit GroundHit()
        {
            Vector3 origin = transform.position + _player.Collider.center - (0.5f * _player.Collider.height - _player.Collider.radius - 0.1f) * transform.up;
            Vector3 direction = -transform.up;

            RaycastHit hit;
            Physics.SphereCast(origin, _player.Collider.radius, direction, out hit, 0.2f, _groundMask, QueryTriggerInteraction.Ignore);

            return hit;
        }

        public bool IsGrounded => _groundHit.collider;
        private float SlopeAngle => Vector3.Angle(Vector3.up, _groundHit.normal);
        private bool IsSlopeTolerable => SlopeAngle <= _slopeTolerance;
        #endregion
    }

}
