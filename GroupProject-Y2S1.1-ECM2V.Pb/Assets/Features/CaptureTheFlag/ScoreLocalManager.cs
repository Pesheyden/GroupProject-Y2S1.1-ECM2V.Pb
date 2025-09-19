using Unity.Netcode;
using UnityEngine;

public class ScoreLocalManager : NetworkBehaviour
{
    public readonly NetworkVariable<int> Score = new NetworkVariable<int>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    protected override void OnNetworkPostSpawn()
    {
        base.OnNetworkPostSpawn();
        if (!IsOwner)
        {
            return;
        }

        Score.Value = Random.Range(0, 100);
        Debug.Log(OwnerClientId + " " + Score.Value);
    }

    public void AddScore(int amount)
    {
        Score.Value += amount;
    }
}
