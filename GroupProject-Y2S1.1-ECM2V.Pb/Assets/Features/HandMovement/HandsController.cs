using System;
using System.Collections.Generic;
using BCommands;
using UnityEngine;

public class HandsController : MonoBehaviour
{
    [SerializeField] private Data_HandsController _dataHandsController;
    [SerializeField] private HandProjectile _leftHandProjectile;
    [SerializeField] private HandProjectile _rightHandProjectile;

    private void Awake()
    {
        _dataHandsController.HandsInputStats.LeftMouseButtonAction.RegisterCommand(CommandFactory.GenericCommand(this, nameof(OnLeftHandAction)));
        _dataHandsController.HandsInputStats.LeftMouseButtonAction.RegisterCommand(CommandFactory.GenericCommand(this, nameof(OnRightHandAction)));
    }

    private void OnLeftHandAction()
    {
        _leftHandProjectile.Use();
    }
    
    private void OnRightHandAction()
    {
        
    }
}
