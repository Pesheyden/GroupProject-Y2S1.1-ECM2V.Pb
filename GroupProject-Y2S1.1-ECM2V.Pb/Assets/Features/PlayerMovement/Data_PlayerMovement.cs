using UnityEngine;

[CreateAssetMenu(menuName = "Data/PlayerMovement/Data_PlayerMovement", fileName = "PlayerMovement_Data")]
public class Data_PlayerMovement : ScriptableObject
{
    public float Speed;
    public float PlayerHeight;
    public float GroundSphereRadios; 
    public LayerMask GroundLayers;
    public float GroundDrag;
    public float JumpForce;
    public float AirMultiplier;
}
