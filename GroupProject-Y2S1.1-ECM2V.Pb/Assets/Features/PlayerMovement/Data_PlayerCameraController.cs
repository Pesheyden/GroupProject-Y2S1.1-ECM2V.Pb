using UnityEngine;

[CreateAssetMenu(menuName = "Data/PlayerMovement/Data_PlayerCameraController", fileName = "PlayerCameraController_Data")]
public class Data_PlayerCameraController : ScriptableObject
{
    public float SensitivityX;
    public float SensitivityY;
    public Vector2 YRotationClamp;
}
