using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerFlagHandler : NetworkBehaviour
{
    public Transform FlagParent;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"OnTriggerEnter1 {OwnerClientId}");
        if(!IsOwner)
            return;
        Debug.Log($"OnTriggerEnter2 {OwnerClientId}");
        if (other.CompareTag("Flag"))
        {
            Flag.Instance.Grab_ServerRpc(this);
        }
        else if(other.CompareTag("FlagPoint"))
        {
            Flag.Instance.TrySubmittingTheFlag_ServerRpc(OwnerClientId);
        }
    }
}
