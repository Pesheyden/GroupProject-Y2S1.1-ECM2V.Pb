using UnityEngine;
using UnityEngine.InputSystem;

public class HandsInputHandler : MonoBehaviour
{
    [SerializeField] private HandsInputMap _playerInputActions;
    [SerializeField] private HandsInputStats _handsInputStats;
    
        private void SetUpInputActions()
    {
        // Button inputs to trigger events
        _playerInputActions.HandsInput.LeftHandAction.started += _ => _handsInputStats.LeftMouseButtonAction?.Raise();
        _playerInputActions.HandsInput.LeftHandAction.started += _ => _handsInputStats.LeftMouseButtonAction?.Raise();
    }
    
    private void OnEnable()
    {
        EnableInputActions();
    }
    
    private void OnDisable()
    {
        DisableInputActions();
    }

    #region Input Action Management
    
    private void EnableInputActions()
    {
        _playerInputActions.HandsInput.LeftHandAction.Enable();
        _playerInputActions.HandsInput.LeftHandAction.Enable();
    }
    
    private void DisableInputActions()
    {
        _playerInputActions.HandsInput.LeftHandAction.Disable();
        _playerInputActions.HandsInput.LeftHandAction.Disable();
    }

    #endregion
}
