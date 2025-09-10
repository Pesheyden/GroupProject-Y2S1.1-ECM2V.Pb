using System;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private PlayerInputStats _playerInputStats;
    [SerializeField] private Data_PlayerCameraController _playerCameraControllerData;
    [SerializeField] private Transform _orientation;

    private float _rotationX;
    private float _rotationY;
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void FixedUpdate()
    {
        float mouseX = _playerInputStats.HorizontalCameraMovement.Value * Time.fixedDeltaTime * _playerCameraControllerData.SensitivityX;
        float mouseY = _playerInputStats.VerticalCameraMovement.Value * Time.fixedDeltaTime * _playerCameraControllerData.SensitivityY;

        _rotationY += mouseX;
        _rotationX -= mouseY;

        _rotationX = Math.Clamp(_rotationX, _playerCameraControllerData.YRotationClamp.x,
            _playerCameraControllerData.YRotationClamp.y);
        
        transform.rotation = Quaternion.Euler(_rotationX,_rotationY,0);
        _orientation.rotation = Quaternion.Euler(_orientation.rotation.x, _rotationY, _orientation.rotation.z);
    }
}
