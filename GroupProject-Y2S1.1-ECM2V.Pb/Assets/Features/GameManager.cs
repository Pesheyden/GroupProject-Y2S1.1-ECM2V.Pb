using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private GameObject _gameEndUi;
    [SerializeField] private GameObject _gameWinUi;
    public int WinScore;

    private void Awake()
    {
        Instance = this;
    }

    [ServerRpc(RequireOwnership = false)]
    public void GameEnd_ServerRpc(ulong winnerId)
    {
        var allClientIds = NetworkManager.Singleton.ConnectedClients.Keys.ToList();
        allClientIds.Remove(winnerId);
        GameEnd_ClientRpc(false, new ClientRpcParams(){Send = new ClientRpcSendParams(){TargetClientIds = allClientIds}});
        GameEnd_ClientRpc(true, new ClientRpcParams(){Send = new ClientRpcSendParams(){TargetClientIds = new List<ulong>{winnerId}}});
    }

    [ClientRpc]
    private void GameEnd_ClientRpc(bool win, ClientRpcParams rpcParams)
    {
        if(win)
            _gameWinUi.SetActive(true);
        else
            _gameEndUi.SetActive(true);

        foreach (var inputHandler in FindObjectsByType<InputHandler>(FindObjectsSortMode.None))
        {
            inputHandler.enabled = false;
        }


    }
}
