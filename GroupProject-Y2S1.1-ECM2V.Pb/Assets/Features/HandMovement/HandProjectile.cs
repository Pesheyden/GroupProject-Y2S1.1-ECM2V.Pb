using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HandProjectile : MonoBehaviour
{
    [SerializeField] private Data_HandProjectile _dataHandProjectile;
    [SerializeField] private Transform _shotPivot;
    [SerializeField] private HandsController _handsController;
    private bool _isBeingUsed;
    private bool _isConnected;
    private GameObject _connectionPoint;


    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Use()
    {
        if(!_isBeingUsed)
            Throw();
        else if(_isConnected)
            Disconnect();
        else
            Exit();
    }

    private void Throw()
    {
        _isBeingUsed = true;
        Vector3 force = _shotPivot.forward * _dataHandProjectile.StartForce;
        _rigidbody.AddForce(force,ForceMode.Force);
    }

    private void Disconnect()
    {
        
    }

    private void Exit()
    {
        _isBeingUsed = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("NoConnection"))
        {
            
        }
    }

    private void Connect()
    {
        
    }
}
