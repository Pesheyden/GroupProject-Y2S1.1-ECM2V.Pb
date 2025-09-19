using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerFlagHandler : NetworkBehaviour
{
    public Transform FlagParent;
    private void OnTriggerEnter(Collider other)
    {
        if(!IsOwner)
            return;
        if (other.CompareTag("Flag"))
        {
            Flag.Instance.GrabServerRpc(this);
        }
        else if(other.CompareTag("FlagPoint"))
        {
            Flag.Instance.TrySubmittingServerRpc(OwnerClientId);
        }
    }
}
