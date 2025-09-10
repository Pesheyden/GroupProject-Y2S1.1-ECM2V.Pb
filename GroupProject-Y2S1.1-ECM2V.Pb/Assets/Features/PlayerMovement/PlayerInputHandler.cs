using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public PlayerInput _playerInputActions;
    [SerializeField] private PlayerInputStats _playerInputStats;
    private void Awake()
    {
        _playerInputActions = new PlayerInput();
        SetUpInputActions();
    }

    private void SetUpInputActions()
    {
        // Movement and camera input
        _playerInputActions.PlayerInputMap.CameraVerticalMovement.performed += SetVerticalCameraInput;
        _playerInputActions.PlayerInputMap.CameraVerticalMovement.canceled += SetVerticalCameraInput;
        _playerInputActions.PlayerInputMap.CameraHorizontalMovement.performed += SetHorizontalCameraInput;
        _playerInputActions.PlayerInputMap.CameraHorizontalMovement.canceled += SetHorizontalCameraInput;
        
        _playerInputActions.PlayerInputMap.CharacterVerticalMovement.performed += SetVerticalMovementInput;
        _playerInputActions.PlayerInputMap.CharacterVerticalMovement.canceled += SetVerticalMovementInput;
        _playerInputActions.PlayerInputMap.CharacterHorizontalMovement.performed += SetHorizontalMovementInput;
        _playerInputActions.PlayerInputMap.CharacterHorizontalMovement.canceled += SetHorizontalMovementInput;
    }
    
    private void OnEnable()
    {
        EnableInputActions();
    }
    
    private void OnDisable()
    {
        DisableInputActions();
    }
    

    #region Input Setters
    private void SetVerticalMovementInput(InputAction.CallbackContext context)
    {
        _playerInputStats.VerticalCharacterMovement.Value = context.ReadValue<float>(); 
    }
    
    private void SetHorizontalMovementInput(InputAction.CallbackContext context)
    {
        _playerInputStats.HorizontalCameraMovement.Value = context.ReadValue<float>(); 
    }
    
    private void SetVerticalCameraInput(InputAction.CallbackContext context)
    {
        _playerInputStats.VerticalCameraMovement.Value = context.ReadValue<float>(); 
    }
    private void SetHorizontalCameraInput(InputAction.CallbackContext context)
    {
        _playerInputStats.HorizontalCameraMovement.Value = context.ReadValue<float>();
    }

    #endregion

    #region Input Action Management
    
    private void EnableInputActions()
    {
        _playerInputActions.PlayerInputMap.CameraVerticalMovement.Enable();
        _playerInputActions.PlayerInputMap.CameraHorizontalMovement.Enable();
    }
    
    private void DisableInputActions()
    {
        _playerInputActions.PlayerInputMap.CameraVerticalMovement.Disable();
        _playerInputActions.PlayerInputMap.CameraHorizontalMovement.Disable();
    }

    #endregion
}
