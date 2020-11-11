using UnityEngine;
using UnityEngine.UI;

namespace MD.UI
{
    public class JoinableRoom : MonoBehaviour
    {
        [SerializeField]
        private InputField ipAddressInputField = null;

        [SerializeField]
        private Button joinButton = null;

        private string ipAddress;
        private NetworkManagerLobby networkManager;

        public void Init(NetworkManagerLobby networkManager, string ipAddress)
        {
            this.networkManager = networkManager;
            this.ipAddress = ipAddress;
            ipAddressInputField.text = ipAddress;
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
            if (ipAddressInputField.text.Length == 0) return;

            //string ipAddress = ipAddressInputField.text;
            networkManager.networkAddress = ipAddress;
            joinButton.interactable  = false;
            networkManager.StartClient();
        }

        private void HandleClientConnected()
        {
            if (joinButton) joinButton.interactable = true;
            // if (CreateRoomWindow) CreateRoomWindow.GetComponent<RoomController>().ShowWindow();
        }

        private void HandleClientDisconnected()
        {
            if (joinButton)
            joinButton.interactable = true;
        }     
    }
}
