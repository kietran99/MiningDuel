using UnityEngine;
using UnityEngine.UI;

namespace MD.UI
{
    public class JoinableRoom : MonoBehaviour
    {
        [SerializeField]
        private Text hostNameText = null;

        [SerializeField]
        private Button joinButton = null;

        private string ipAddress;
        private NetworkManagerLobby networkManager;

        public void Init(NetworkManagerLobby networkManager, string ipAddress, string hostName)
        {
            this.networkManager = networkManager;
            this.ipAddress = ipAddress;
            hostNameText.text = hostName;
            NetworkManagerLobby.OnClientConnected += HandleClientConnected;
            NetworkManagerLobby.OnClientDisconnnected += HandleClientDisconnected;
            gameObject.SetActive(true);
        }
        
        private void OnDestroy() 
        {           
            NetworkManagerLobby.OnClientConnected -= HandleClientConnected;
            NetworkManagerLobby.OnClientDisconnnected -= HandleClientDisconnected;
        }

        public void JoinLobby()
        {
            if (hostNameText.text.Length == 0) return;

            networkManager.networkAddress = ipAddress;
            joinButton.interactable  = false;
            networkManager.StartClient();
        }

        private void HandleClientConnected()
        {
            if (joinButton) joinButton.interactable = true;
        }

        private void HandleClientDisconnected()
        {
            if (joinButton) joinButton.interactable = true;
        }     
    }
}
