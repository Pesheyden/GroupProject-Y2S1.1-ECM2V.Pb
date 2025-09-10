using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerInputStats _playerInputStats;
    [SerializeField] private Data_PlayerMovement _playerMovementData;
    [SerializeField] private Transform _orientation;

    private Vector3 _moveDirection;
    private Rigidbody _rb;
    private bool _isGrounded;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {

        _isGrounded = Physics.CheckSphere(
            new Vector3(transform.position.x, transform.position.y - _playerMovementData.PlayerHeight * 0.5f,
                transform.position.z),
            _playerMovementData.GroundSphereRadios);

        if (_isGrounded)
            _rb.linearDamping = _playerMovementData.GroundDrag;
        else
            _rb.linearDamping = 0;
        
        MovePlayer();
        SpeedCheck();
    }

    private void MovePlayer()
    {
        _moveDirection = _orientation.forward * _playerInputStats.VerticalCharacterMovement.Value +
                         _orientation.right * _playerInputStats.HorizontalCharacterMovement.Value;
        
        _rb.AddForce(_moveDirection.normalized * _playerMovementData.Speed, ForceMode.Force);
    }

    private void SpeedCheck()
    {
        Vector3 flatVel = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
        if (flatVel.magnitude > _playerMovementData.Speed)
        {
            flatVel = flatVel.normalized * _playerMovementData.Speed;
            _rb.linearVelocity = new Vector3(flatVel.x, _rb.linearVelocity.y, flatVel.z);
        }
    }
}
