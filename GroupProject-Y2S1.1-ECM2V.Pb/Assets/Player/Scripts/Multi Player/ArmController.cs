using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace MultiPlayer.Player
{
    [DefaultExecutionOrder(100)]
    [RequireComponent(typeof(Player))]
    public class ArmController : NetworkBehaviour
    {
        [SerializeField] private Player _player;

        [Space]
        [SerializeField] private Transform[] _arms;
        private Transform[] _anchors;

        [Space]
        [SerializeField] private LineRenderer[] _lineRenderers;

        [Space]
        [SerializeField] private float _range = 10f;
        [SerializeField] private float _radius = 1f;
        [SerializeField][Range(0f, 1f)] private float _spread = 0.5f;
        [SerializeField] private LayerMask _layerMask = ~0;

        [Space]
        [SerializeField] private Transform[] _reticles;

        [Space]
        [SerializeField] private float _force = 4.905f;

        private float[] _baseLengths;
        [Space][SerializeField] private float _maxLengthMultiplier = 1.25f;
        [SerializeField] private float _maxLength = 25f;

        [Space]
        [SerializeField] private Rigidbody[] _armPrefabs;
        [SerializeField] private float _breakImpulseMultiplier = 10f;
        [SerializeField] private float _regenerationTime = 5f;

        private Rigidbody[] _grabbedRigidbodies;

        private void Reset()
        {
            _player = GetComponent<Player>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            NetworkAwake();

            OnEnable();
        }

        private void NetworkAwake()
        {

            if (!IsOwner) return;

            if (!_player) _player = GetComponent<Player>();


            InitializeArrays();

            UnparentReticles();
        }

        private void InitializeArrays()
        {
            if (!IsOwner) return;

            _arms ??= new Transform[2];
            _lineRenderers ??= new LineRenderer[2];
            _anchors ??= new Transform[]
            {
                new GameObject("Anchor").transform,
                new GameObject("Anchor (1)").transform
            };

            _baseLengths ??= new float[2];

            _grabbedRigidbodies ??= new Rigidbody[2];

            _aimHits ??= new RaycastHit[2];

            _isBroken ??= new bool[2];

            _regenerate ??= new IEnumerator[2];
        }

        private void UnparentReticles()
        {
            if (!IsOwner) return;

            for (int i = 0; i < 2; i++)
            {
                _reticles[i].SetParent(null);
            }
        }

        private void OnEnable()
        {
            if (!IsOwner) return;

            Debug.Log($"{this}: OnEnable");

            SubscribeToInputActions();

            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnDisable()
        {
            if (!IsOwner) return;

            UnsubscribeFromInputActions();

            UnparentAnchors();
            ReleaseAll();
            for (int i = 0; i < 2; i++) ReticleSetActiveManual(i, false);

            Cursor.lockState = CursorLockMode.None;
        }

        private void SubscribeToInputActions()
        {
            if (!IsOwner) return;

            Debug.Log($"{this}: SubscribeToInputActions");

            InputHandler.Instance.OnStretchStarted += Grab;
            InputHandler.Instance.OnStretchCanceled += Release;

            InputHandler.Instance.OnStretch1Started += Grab;
            InputHandler.Instance.OnStretch1Canceled += Release;
        }

        private void UnsubscribeFromInputActions()
        {
            if (!IsOwner) return;

            InputHandler.Instance.OnStretchStarted -= Grab;
            InputHandler.Instance.OnStretchCanceled -= Release;

            InputHandler.Instance.OnStretch1Started -= Grab;
            InputHandler.Instance.OnStretch1Canceled -= Release;
        }

        private void UnparentAnchors()
        {
            if (!IsOwner) return;

            for (int i = 0; i < 2; i++) _anchors[i].SetParent(null);
        }

        private void FixedUpdate()
        {
            if (!IsOwner) return;

            Aim();
            AddForce();
            TryBreak();
        }

        private RaycastHit[] _aimHits;
        private void Aim()
        {
            if (!IsOwner) return;

            float x = 0.5f * Screen.width; float xOffset = 0.5f * _spread * Screen.width;
            float y = 0.5f * Screen.height;

            Ray[] rays = new Ray[]
            {
                _player.PlayerCamera.Camera.ScreenPointToRay(new Vector3(x - xOffset, y)),
                _player.PlayerCamera.Camera.ScreenPointToRay(new Vector3(x + xOffset, y))
            };

            for (int i = 0; i < 2; i++)
            {
                if (_isGrabbed[i] || _isBroken[i]) continue;

                if (!Physics.Raycast(rays[i], out _aimHits[i], _range, _layerMask, QueryTriggerInteraction.Ignore))
                {
                    rays[i].origin += _radius * rays[i].direction;
                    Physics.SphereCast(rays[i], _radius, out _aimHits[i], _range, _layerMask, QueryTriggerInteraction.Ignore);
                }

                ReticleSetActive(i);
            }
        }

        private void ReticleSetActive(int i)
        {
            if (!IsOwner) return;

            if (_reticles == null || _reticles.Length <= i || _reticles[i] == null) return;

            if (_aimHits[i].collider) _reticles[i].position = _aimHits[i].point;
            _reticles[i].gameObject.SetActive(!_isGrabbed[i] && _aimHits[i].collider);
        }

        private void ReticleSetActiveManual(int i, bool value)
        {
            if (!IsOwner) return;

            _reticles[i].gameObject.SetActive(value);
        }

        private NetworkList<bool> _isGrabbed = new(new List<bool>() { false, false }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        private void Grab(int i)
        {
            if (!IsOwner) return;

            if (_isGrabbed[i] || _isBroken[i]) return;
            if (!_aimHits[i].collider) return;

            if (_aimHits[i].collider.CompareTag("Flag"))
            {
                Flag.Instance.Grab_ServerRpc(_player.PlayerFlagHandler);
                return;
            }

            _isGrabbed[i] = true;

            _arms[i].gameObject.SetActive(false);

            _anchors[i].SetParent(_aimHits[i].transform);
            _anchors[i].position = _aimHits[i].point;

            _baseLengths[i] = (_anchors[i].position - _player.Rigidbody.position).magnitude;

            _grabbedRigidbodies[i] = _aimHits[i].rigidbody;
        }

        public void Release(int i)
        {
            if (!IsOwner) return;

            if (!_isGrabbed[i]) return;

            _isGrabbed[i] = false;

            _arms[i].gameObject.SetActive(true);

            _anchors[i].SetParent(null);

            _grabbedRigidbodies[i] = null;
        }

        private void ReleaseAll()
        {
            if (!IsOwner) return;

            for (int i = 0; i < 2; i++) Release(i);
        }

        private void AddForce()
        {
            if (!IsOwner) return;

            for (int i = 0; i < 2; i++)
            {
                if (!_isGrabbed[i]) continue;

                if (_grabbedRigidbodies[i] && !_grabbedRigidbodies[i].isKinematic)
                {
                    Vector3 direction = (_player.Rigidbody.position - _anchors[i].position).normalized;
                    _grabbedRigidbodies[i].AddForce(_force * direction);
                }
                else
                {
                    Vector3 direction = (_anchors[i].position - _player.Rigidbody.position).normalized;
                    _player.Rigidbody.AddForce(_force * direction);
                }
            }
        }

        private void TryBreak()
        {
            if (!IsOwner) return;

            for (int i = 0; i < 2; i++)
            {
                if (!_isGrabbed[i]) continue;

                float length = (_anchors[i].position - _player.Rigidbody.position).magnitude;
                float maxLength = Mathf.Max(_maxLength, _maxLengthMultiplier * _baseLengths[i]);

                if (length > maxLength) Break(i);
            }
        }

        private bool[] _isBroken;
        public void Break(int i)
        {
            if (!IsOwner) return;

            if (_isBroken[i]) return;

            Vector3 impulse = _player.Rigidbody.position - _anchors[i].position;
            impulse = _breakImpulseMultiplier * new Vector3(impulse.x, impulse.magnitude, impulse.z);

            ReleaseAll();
            InstantiateArm(i, impulse);

            _isBroken[i] = true;

            // Commented out because the temporary model's arms are not separated, thus disabling them disables the full model.
            //_arms[i].gameObject.SetActive(false);
            StartRegenerate(i);

            _player.RagdollHandler.AddImpulse(impulse);
        }

        private void InstantiateArm(int i, Vector3 impulse)
        {
            if (!IsOwner) return;

            if (_armPrefabs == null || _armPrefabs.Length <= i || _armPrefabs[i] == null) return;

            InstantiateArmServerRpc(i, impulse);
        }

        [ServerRpc]
        private void InstantiateArmServerRpc(int i, Vector3 impulse)
        {
            Rigidbody arm = Instantiate(_armPrefabs[i], _arms[i].position, _arms[i].rotation);
            arm.AddForce(-impulse, ForceMode.Impulse);

            NetworkObject armNetworkObject = arm.GetComponent<NetworkObject>();
            if (armNetworkObject) armNetworkObject.Spawn(true);
        }

        private void StartRegenerate(int i)
        {
            if (!IsOwner) return;

            StopRegenerate(i);

            StartCoroutine(_regenerate[i] = Regenerate(i));
        }

        private void StopRegenerate(int i)
        {
            if (_regenerate[i] != null)
            {
                StopCoroutine(_regenerate[i]);
                _regenerate[i] = null;
            }
        }

        private IEnumerator[] _regenerate;
        private IEnumerator Regenerate(int i)
        {
            yield return new WaitForSeconds(_regenerationTime);

            _isBroken[i] = false;

            _arms[i].gameObject.SetActive(true);

            _regenerate[i] = null;
        }

        private void Update()
        {
            DrawLines();
        }

        private NetworkList<Vector3> _lineStarts = new(new List<Vector3>() { Vector3.zero, Vector3.zero }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        private NetworkList<Vector3> _lineEnds = new(new List<Vector3>() { Vector3.zero, Vector3.zero }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        private void DrawLines()
        {
            for (int i = 0; i < 2; i++)
            {
                if (!_isGrabbed[i])
                {
                    _lineRenderers[i].positionCount = 0;
                    continue;
                }
                else _lineRenderers[i].positionCount = 2;

                if (IsOwner)
                {
                    _lineStarts[i] = _arms[i].position;
                    _lineEnds[i] = _anchors[i].position;
                }

                _lineRenderers[i].startColor = _player.color.Value;
                _lineRenderers[i].endColor = _player.color.Value;

                _lineRenderers[i].SetPosition(0, _lineStarts[i]);
                _lineRenderers[i].SetPosition(1, _lineEnds[i]);
            }
        }
    }

}
