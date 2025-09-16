using Unity.Netcode;
using UnityEngine;

public class RagdollHandler : NetworkBehaviour
{
    [SerializeField] private Transform _root;

    [SerializeField] private Transform _axolotlRoot;
    [SerializeField] private Transform _axolotlHips;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    public void EnableRagdoll()
    {
        _root.SetParent(null);
        
    }

    public void DisableRagdoll()
    {
        _axolotlHips.SetParent(null);
        _root.SetParent(_axolotlHips);
    }
}
