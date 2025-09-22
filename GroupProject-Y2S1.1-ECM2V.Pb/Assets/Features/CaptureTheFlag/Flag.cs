using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Flag : NetworkBehaviour
{

    private static Flag _instance;

    public static Flag Instance => _instance;


    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private Vector3 _startScale;
    private Transform _startTarget;
    private Transform _target;
    public ulong PlayerFlagOwner = ulong.MaxValue;

    private void Awake()
    {
        _instance = this;
        _startPosition = transform.position;
        _startRotation = transform.rotation;
        _startScale = transform.localScale;
        _startTarget = transform.parent;
        _target = _startTarget;
    }

    private void Update()
    {
        transform.position = _target.position;
    }


    [ServerRpc(RequireOwnership = false)]
    public void Grab_ServerRpc(NetworkBehaviourReference  playerFlagHandlerReference)
    {
        if (playerFlagHandlerReference.TryGet<PlayerFlagHandler>(out var playerFlagHandler))
        {
            Debug.Log($"Grab: {playerFlagHandler.OwnerClientId}");
            PlayerFlagOwner = playerFlagHandler.OwnerClientId;
            _target = playerFlagHandler.FlagParent;
        }

    }

    [ServerRpc(RequireOwnership = false)]
    public void TrySubmittingTheFlag_ServerRpc(ulong clientId)
    {
        if(clientId != PlayerFlagOwner)
            return;
        ResetFlag();
        AddScore_ClientRPC(new ClientRpcParams()
            { Send = new ClientRpcSendParams() { TargetClientIds = new List<ulong>() { clientId } } });
    }

    [ClientRpc]
    private void AddScore_ClientRPC(ClientRpcParams rpcParams)
    {
        ScoreRedirect.Instance.AddScore(1);
    }
    private void ResetFlag()
    {
        _target = _startTarget;
        transform.position = _startPosition;
        transform.rotation = _startRotation;
        transform.localScale = _startScale;
        PlayerFlagOwner = ulong.MaxValue;
    }
}
