using Mirror.Discovery;
using UnityEngine;

namespace MD.UI.MainMenu
{
    public class LobbyController : MonoBehaviour
    {
        // [SerializeField]
        // private RoomController createRoomWindow = null;

        [SerializeField]
        private RoomController joinRoomWindow = null;

        [SerializeField]
        private NetworkManagerLobby networkManager = null;

        [SerializeField]
        private NetworkDiscovery networkDiscovery = null;

        public void OpenCreateRoomWindow()
        {
            // OpenRoomWindow(createRoomWindow);
            networkManager.StartHost();
            networkDiscovery.AdvertiseServer();
        }

        public void OpenJoinRoomWindow()
        {
            OpenRoomWindow(joinRoomWindow);
        }

        private void OpenRoomWindow(RoomController window)
        {
            window.ShowWindow();
            
        }
    }
}