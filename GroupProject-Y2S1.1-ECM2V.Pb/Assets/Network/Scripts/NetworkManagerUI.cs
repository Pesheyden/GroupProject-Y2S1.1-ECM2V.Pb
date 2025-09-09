using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Test_Network
{
    public class NetworkManagerUI : MonoBehaviour
    {
        [SerializeField] private Button _serverButton;
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _clientButton;

        private void Awake()
        {
            AddListners();
        }

        private void AddListners()
        {
            _serverButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartServer();
            });

            _hostButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartHost();
            });

            _clientButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartClient();
            });
        }
    }

}
