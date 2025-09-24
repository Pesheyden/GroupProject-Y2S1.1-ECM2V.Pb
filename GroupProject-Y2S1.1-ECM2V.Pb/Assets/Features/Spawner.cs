using System;
using Unity.Netcode;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance { get; private set; }
    [SerializeField] private Transform[] _spawnPositions;

    private int _currentIndex;

    private void Awake()
    {
        Instance = this;
    }

    [ServerRpc(RequireOwnership = false)]
    public void Spawn_ServerRpc(Transform player)
    {
        player.position = _spawnPositions[_currentIndex].position;
        _currentIndex++;
    }
}
