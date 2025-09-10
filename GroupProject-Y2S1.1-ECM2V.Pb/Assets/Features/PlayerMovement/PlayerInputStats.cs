using BSOAP.Events;
using BSOAP.Variables;
using UnityEngine;

[CreateAssetMenu (menuName = "Data/PlayerMovement/PlayerInputStats", fileName = "PlayerInputStats")]
public class PlayerInputStats : ScriptableObject
{
    [SubAsset] public FloatVariable VerticalCameraMovement;
    [SubAsset] public FloatVariable HorizontalCameraMovement;
    
    [SubAsset] public FloatVariable VerticalCharacterMovement;
    [SubAsset] public FloatVariable HorizontalCharacterMovement;

    [SubAsset] public CommandEventSo Jump;
}
