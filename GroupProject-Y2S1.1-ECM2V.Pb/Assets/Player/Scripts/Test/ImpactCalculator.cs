using UnityEngine;

namespace Test.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class ImpactCalculator : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;

        [Space]
        [SerializeField] private float _launchImpulse = 10f;

        private void Reset()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Awake()
        {
            
        }

        [ContextMenu("Launch Right")]
        private void LaunchRight()
        {
            _rigidbody.AddForce(_launchImpulse * Vector3.right, ForceMode.Impulse);
        }

        [ContextMenu("Launch Up")]
        private void LaunchUp()
        {
            _rigidbody.AddForce(_launchImpulse * Vector3.up, ForceMode.Impulse);
        }

        [ContextMenu("Launch Forward")]
        private void LaunchForward()
        {
            _rigidbody.AddForce(_launchImpulse * Vector3.forward, ForceMode.Impulse);
        }

        private void OnCollisionEnter(Collision collision)
        {
            float impact = collision.contacts[0].impulse.magnitude;

            Debug.Log($"{this}: impact is {impact}");
        }
    }

}
