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
            Room.GetComponent<CustomNetworkDiscovery>().AdvertiseServer(PlayerPrefs.GetString(PlayerNameInput.PLAYER_PREF_NAME_KEY));
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