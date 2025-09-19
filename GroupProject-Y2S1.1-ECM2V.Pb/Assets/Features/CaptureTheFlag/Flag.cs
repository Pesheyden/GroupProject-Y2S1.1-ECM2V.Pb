using System;
using Unity.Netcode;
using UnityEngine;

public class Flag : MonoBehaviour
{

    private static Flag _instance;

    public static Flag Instance => _instance;


    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private Vector3 _startScale;
    private Transform _parent;
    public ulong PlayerFlagOwner = ulong.MaxValue;

    private void Awake()
    {
        _instance = this;
        _startPosition = transform.position;
        _startRotation = transform.rotation;
        _startScale = transform.localScale;
        _parent = transform.parent;
    }

    public void ResetFlag()
    {
        transform.parent = _parent;
        transform.position = _startPosition;
        transform.rotation = _startRotation;
        transform.localScale = _startScale;
        PlayerFlagOwner = ulong.MaxValue;
    }

    [ServerRpc]
    public void GrabServerRpc(PlayerFlagHandler playerFlagHandler)
    {
        PlayerFlagOwner = playerFlagHandler.OwnerClientId;
        transform.parent = playerFlagHandler.FlagParent;
    }

    [ServerRpc]
    public void TrySubmittingServerRpc(ulong clientId)
    {
        if(clientId != PlayerFlagOwner)
            return;
        ResetFlag();
    }

}
