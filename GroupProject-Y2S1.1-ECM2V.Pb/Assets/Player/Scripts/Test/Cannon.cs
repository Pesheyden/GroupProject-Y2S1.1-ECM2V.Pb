using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Test.Objects
{
    public class Cannon : NetworkBehaviour
    {
        [SerializeField] private Transform _joint;
        [SerializeField] private Transform _muzzle;

        [Space]
        [SerializeField] private Rigidbody _bulletPrefab;
        private List<Rigidbody> _bulletInstances = new();
        [SerializeField] private int _maxBulletInstances = 10;

        [Space]
        [SerializeField] private float _detectionRadius = 5f;
        [SerializeField] private float _aimTime = 3f;
        [SerializeField] private float _turnSpeed = 120f;
        [SerializeField] private float _impulse = 10f;
        [SerializeField] private float _cooldownTime = 2f;

        private PlayerController _target;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            enabled = IsServer;

            Debug.Log($"{this} is spawned.");
        }

        private void FixedUpdate()
        {
            if (!IsServer) return;

            if (_cooldown == null)
            {
                if (!_target) _target = DetectTarget();
                else if (_aim == null) StartCoroutine(_aim = Aim());
            }
        }

        private PlayerController DetectTarget()
        {
            Collider[] colliders = Physics.OverlapSphere(_joint.position, _detectionRadius);

            foreach (Collider collider in colliders)
            {
                PlayerController playerController = collider.GetComponent<PlayerController>();
                if (playerController)
                {
                    Vector3 delta = playerController.transform.position + Vector3.up - _joint.position;

                    RaycastHit hit;
                    Physics.Raycast(_joint.position, delta.normalized, out hit, delta.magnitude, ~gameObject.layer, QueryTriggerInteraction.Ignore);

                    if (hit.collider && hit.collider == collider)
                    {
                        Debug.Log($"{this} has detected a valid target!");
                        return playerController;
                    }
                }
            }

            return null;
        }

        private IEnumerator _aim;
        private IEnumerator Aim()
        {
            Debug.Log($"{this} has started aiming...");

            Vector3 upwards = Vector3.up;
            Vector3 forward;

            float time = 0f;
            while (time < _aimTime)
            {
                yield return null;
                time += Time.deltaTime;

                forward = (_target.transform.position + Vector3.up - _joint.position).normalized;

                Vector3.OrthoNormalize(ref forward, ref upwards);

                Quaternion lookRotation = Quaternion.LookRotation(forward, upwards);

                _joint.rotation = Quaternion.RotateTowards(_joint.rotation, lookRotation, Time.deltaTime * _turnSpeed);
            }

            Fire();

            _aim = null;
        }

        private void Fire()
        {
            Debug.Log($"{this} has fired!");

            Rigidbody bullet = Instantiate(_bulletPrefab, _muzzle.position, _muzzle.rotation);

            NetworkObject networkObject = bullet.GetComponent<NetworkObject>();
            networkObject.Spawn(true);

            bullet.AddForce(_impulse * _muzzle.forward, ForceMode.Impulse);

            _bulletInstances.Add(bullet);
            if (_bulletInstances.Count > _maxBulletInstances) Destroy(_bulletInstances[0].gameObject);

            _target = null;

            StartCoroutine(_cooldown = Cooldown());
        }

        private IEnumerator _cooldown;
        private IEnumerator Cooldown()
        {
            Debug.Log($"{this} is now entering cooldown...");

            yield return new WaitForSeconds(_cooldownTime);

            _cooldown = null;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_joint.position, _detectionRadius);
        }
    }

}
