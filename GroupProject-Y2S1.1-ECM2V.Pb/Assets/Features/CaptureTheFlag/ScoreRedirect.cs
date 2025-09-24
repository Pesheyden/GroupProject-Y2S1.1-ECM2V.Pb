using System;
using NaughtyAttributes;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ScoreRedirect : MonoBehaviour
{
    private static ScoreRedirect _instance;
    public static ScoreRedirect Instance => _instance; 
    
    [SerializeField] private TextMeshProUGUI[] _texts;

    [SerializeField] private ScoreLocalManager[] _scoreLocalManagers;

    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnClientConnected(ulong obj)
    {
        _scoreLocalManagers = new ScoreLocalManager[(int)obj + 1];
        for (int i = (int)obj; i >= 0; i--)
        {
            _scoreLocalManagers[i] = NetworkManager.Singleton.ConnectedClients[(ulong)i].PlayerObject.gameObject
                .GetComponentInChildren<ScoreLocalManager>();
        }
        //_scoreLocalManagers[obj] = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.GetComponentInChildren<ScoreLocalManager>();
        foreach (var scoreManager in _scoreLocalManagers)
        {
            scoreManager.Score.OnValueChanged -= OnValueChanged;
            scoreManager.Score.OnValueChanged += OnValueChanged;
        }

        OnValueChanged(0,0);
    }

    private void OnValueChanged(int previousValue, int newValue)
    {
        for (var index = 0; index < _scoreLocalManagers.Length; index++)
        {
            _texts[index].text = _scoreLocalManagers[index].Score.Value.ToString();
        }
    }
    
    public void AddScore(int amount)
    {
        _scoreLocalManagers[NetworkManager.Singleton.LocalClientId].AddScore(amount);
    }
    
    [SerializeField] private int amount;
    [Button]
    public void AddScoreTest()
    {
        AddScore(amount);
    }
}
