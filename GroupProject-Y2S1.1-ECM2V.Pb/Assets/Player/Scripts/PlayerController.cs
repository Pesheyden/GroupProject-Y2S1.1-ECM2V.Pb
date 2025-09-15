using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private CameraController _cameraControllerPrefab;
    private CameraController _cameraController;

    [Header("Components")]
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private CapsuleCollider _collider;

    [Header("Variables")]
    private Vector2 _moveInput;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _moveAcceleration = 10f;

    [Space]
    [SerializeField] private float _jumpImpulse = 5f;

    [Space]
    [SerializeField] private LayerMask _groundMask = ~0;
    [SerializeField] private float _slopeTolerance = 50f;

    [Header("Ragdoll")]
    [SerializeField] private Collider[] _ragdollColliders = new Collider[0];

    [Space]
    [SerializeField] private float _ragdollLinearVelocityThreshold = 2f;
    [SerializeField] private float _ragdollAngularVelocityThreshold = 2f;

    [Space]
    [SerializeField] private float _minRagdollTime = 2f;
    [SerializeField] private float _maxRagdollTime = 20f;
    private float _ragdollTime = 0f;
    [SerializeField] private float _ragdollTimeReductionPerInput = 0.1f;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        enabled = IsOwner;

        SubscribeToInputActions();

        _cameraController = Instantiate(_cameraControllerPrefab);
        _cameraController.SetTarget(transform);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        UnsubscribeFromInputActions();

        Destroy(_cameraController.gameObject);
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
        if (!IsOwner || !InputHandler.Instance) return;

        _cameraController = InputHandler.Instance.CameraController;
        _cameraController.SetTarget(transform);

        InputHandler.Instance.OnAnyPerformed += ReduceRagdollTime;

        InputHandler.Instance.OnMovePerformed += SetMoveInput;
        InputHandler.Instance.OnMoveCanceled += SetMoveInput;

        InputHandler.Instance.OnJumpPerformed += Jump;
    }

    private void UnsubscribeFromInputActions()
    {
        if (!IsOwner || !InputHandler.Instance) return;

        if (_cameraController.Target == transform) _cameraController.SetTarget(null);
        _cameraController = InputHandler.Instance.CameraController ? null : _cameraController;

        InputHandler.Instance.OnAnyPerformed -= ReduceRagdollTime;

        InputHandler.Instance.OnMovePerformed -= SetMoveInput;
        InputHandler.Instance.OnMoveCanceled -= SetMoveInput;

        InputHandler.Instance.OnJumpPerformed -= Jump;
    }

    private void SetMoveInput(Vector2 input)
    {
        _moveInput = input;
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        if (!IsRagdoll)
        {
            UpdateDisplacement();
            _groundHit = GroundHit();

            Gravity();
            SnapToGround();

            Move();
        }
    }

    private void Move()
    {
        if (!IsOwner) return;

        if (!_cameraController)
        {
            Debug.LogError($"{this}: missing CameraController reference.");
            return;
        }
        else if (!_cameraController.Camera)
        {
            Debug.LogError($"{this}: CameraController missing Camera reference.");
            return;
        }

        Vector3 right = _cameraController.Camera.transform.right;
        Vector3 up = Vector3.up;
        Vector3 forward = _cameraController.Camera.transform.forward;

        Vector3.OrthoNormalize(ref up, ref forward, ref right);
        if (IsGrounded && IsSlopeTolerable)
        {
            up = _groundHit.normal;
            Vector3.OrthoNormalize(ref up, ref forward, ref right);
            // Makes the movement axes perpendicular to the ground.
        }

        float xCurrent = GetDirectionalLinearVelocity(right);
        float xTarget = _moveSpeed * _moveInput.x;
        float xDelta = CurrentExceedsTarget(xCurrent, xTarget) ? 0f : xTarget - xCurrent;
        // If current exceeds target, delta equals 0, otherwise, delta equals target minus current.

        float zCurrent = GetDirectionalLinearVelocity(forward);
        float zTarget = _moveSpeed * _moveInput.y;
        float zDelta = CurrentExceedsTarget(zCurrent, zTarget) ? 0f : zTarget - zCurrent;

        Vector3 force = _moveAcceleration * (xDelta * right + zDelta * forward);
        _rigidbody.AddForce(force);

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
        Vector3 projection = Vector3.Project(_rigidbody.linearVelocity, direction);
        float dot = Vector3.Dot(projection.normalized, direction.normalized);

        return dot * projection.magnitude;
    }

    private void Jump()
    {
        if (!IsOwner) return;
        if (!IsGrounded) return;
        if (IsRagdoll) return;

        _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, 0f, _rigidbody.linearVelocity.z);
        _rigidbody.AddForce(_jumpImpulse * transform.up, ForceMode.Impulse);

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
        if (!IsOwner) return;
        if (IsGrounded && IsSlopeTolerable) return;

        _rigidbody.AddForce(Physics.gravity);
    }

    private void SnapToGround()
    {
        if (!IsOwner) return;
        if (!IsGrounded) return;
        if (_jumping != null) return;

        _rigidbody.MovePosition(_rigidbody.position + (_groundHit.distance - 0.1f) * -transform.up);
        
        if ((_rigidbody.linearVelocity.y > 0f && _rigidbody.linearVelocity.y > _displacement.y / Time.fixedDeltaTime) ||
            _rigidbody.linearVelocity.y < 0f && _rigidbody.linearVelocity.y < _displacement.y / Time.fixedDeltaTime)
        {
            _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, _displacement.y / Time.fixedDeltaTime, _rigidbody.linearVelocity.z);
        }
    }

    #region Displacement
    private Vector3 _oldPosition;
    private Vector3 _displacement;
    private void UpdateDisplacement()
    {
        _displacement = _rigidbody.position - _oldPosition;
        _oldPosition = _rigidbody.position;
    }
    #endregion

    #region Grounded
    private RaycastHit _groundHit;
    private RaycastHit GroundHit()
    {
        Vector3 origin = transform.position + _collider.center - (0.5f * _collider.height - _collider.radius - 0.1f) * transform.up;
        Vector3 direction = -transform.up;

        RaycastHit hit;
        Physics.SphereCast(origin, _collider.radius, direction, out hit, 0.2f, _groundMask, QueryTriggerInteraction.Ignore);

        return hit;
    }

    public bool IsGrounded => _groundHit.collider;
    private float SlopeAngle => Vector3.Angle(Vector3.up, _groundHit.normal);
    private bool IsSlopeTolerable => SlopeAngle <= _slopeTolerance;
    #endregion

    #region Ragdoll
    public bool IsRagdoll { get; private set; }
    public void EnableRagdoll()
    {
        Debug.Log($"{this}: enable ragdoll");

        IsRagdoll = true;

        //_collider.enabled = false;
        _rigidbody.useGravity = true;
        _rigidbody.constraints = RigidbodyConstraints.None;

        foreach (Collider collider in _ragdollColliders)
        {
            collider.enabled = true;
            if (collider.attachedRigidbody)
            {
                collider.attachedRigidbody.linearVelocity = Vector3.zero;
                collider.attachedRigidbody.angularVelocity = Vector3.zero;

                collider.attachedRigidbody.isKinematic = true;
            }
        }

        if (_cameraController) _cameraController.SetPerspective(CameraController.Perspective.ThirdPerson);

        StartCoroutine(_ragdoll = Ragdoll());
    }

    public void DisableRagdoll()
    {
        Debug.Log($"{this}: disable ragdoll");

        if (_ragdoll != null)
        {
            StopCoroutine(_ragdoll);
            _ragdoll = null;
        }

        _collider.enabled = true;
        _rigidbody.useGravity = false;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        transform.rotation = Quaternion.identity;

        foreach (Collider collider in _ragdollColliders)
        {
            collider.enabled = false;
            if (collider.attachedRigidbody)
            {
                collider.attachedRigidbody.linearVelocity = Vector3.zero;
                collider.attachedRigidbody.angularVelocity = Vector3.zero;

                collider.attachedRigidbody.isKinematic = true;
            }
        }

        if (_cameraController) _cameraController.SetPerspective(CameraController.Perspective.FirstPerson);

        IsRagdoll = false;
    }

    private IEnumerator _ragdoll;
    private IEnumerator Ragdoll()
    {
        _ragdollTime = _maxRagdollTime;
        while (_ragdollTime > 0f)
        {
            yield return null;
            _ragdollTime -= Time.deltaTime;
            
            if (_maxRagdollTime - _ragdollTime > _minRagdollTime &&
                IsNotRagdollVelocityThreshold()) DisableRagdoll();
        }

        _ragdoll = null;
    }

    private bool IsNotRagdollVelocityThreshold()
    {
        return _rigidbody.linearVelocity.magnitude < _ragdollLinearVelocityThreshold &&
            _rigidbody.angularVelocity.magnitude < _ragdollAngularVelocityThreshold;
    }

    private void ReduceRagdollTime()
    {
        _ragdollTime -= _ragdollTimeReductionPerInput;
    }

    [ContextMenu("Test_ImpulseRagdoll")]
    private void Test_ImpulseRagdoll()
    {
        EnableRagdoll();

        _rigidbody.AddForce(5f * Random.insideUnitSphere.normalized, ForceMode.Impulse);
    }
    #endregion
}
