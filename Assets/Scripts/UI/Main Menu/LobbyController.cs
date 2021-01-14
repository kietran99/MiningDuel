using Mirror.Discovery;
using UnityEngine;
using Mirror;

namespace MD.UI.MainMenu
{
    public class LobbyController : MonoBehaviour
    {
        // [SerializeField]
        // private RoomController createRoomWindow = null;

        [SerializeField]
        private RoomController joinRoomWindow = null;

        private NetworkManagerLobby room;
        private NetworkManagerLobby Room
        {
            get
            {
                if (room != null) return room;
                return room = NetworkManager.singleton as NetworkManagerLobby;
            }

        }

        public void OpenCreateRoomWindow()
        {
            // OpenRoomWindow(createRoomWindow);
            Room.StartHost();
            Room.GetComponent<NetworkDiscovery>().AdvertiseServer();
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