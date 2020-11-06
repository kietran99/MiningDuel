using UnityEngine;
using UnityEngine.UI;

namespace MD.UI.MainMenu
{
    public class JoinRoomController : MonoBehaviour
    {
        [SerializeField]
        private InputField ipAddressInputField = null;

        [SerializeField]
        private NetworkManagerLobby networkManager = null;

        [SerializeField]
        private Button joinButton = null;
        
        void OnEnable()
        {
            NetworkManagerLobby.OnClientConnected += HandleClientConnected;
            NetworkManagerLobby.OnClientDisconnnected += HandleClientDisconnected;
        }
        void OnDisable()
        {
            NetworkManagerLobby.OnClientConnected -= HandleClientConnected;
            NetworkManagerLobby.OnClientDisconnnected -= HandleClientDisconnected;
        }
        public void JoinLobby()
        {
            if (ipAddressInputField.text.Length == 0) return;
            string ipAddress = ipAddressInputField.text;
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
