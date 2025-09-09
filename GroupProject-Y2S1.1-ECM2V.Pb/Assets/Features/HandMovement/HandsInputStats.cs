using BSOAP.Events;
using UnityEngine;

[CreateAssetMenu(fileName = "HandsInputStats", menuName = "Data/Hands/HandsInputStats")]
public class HandsInputStats : ScriptableObject
{
    [SubAsset] public CommandEventSo LeftMouseButtonAction;
    [SubAsset] public CommandEventSo RightMouseButtonAction;
}
