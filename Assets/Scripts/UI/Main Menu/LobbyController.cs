using UnityEngine;

namespace MD.UI.MainMenu
{
    public class LobbyController : MonoBehaviour
    {
        [SerializeField]
        private RoomController createRoomWindow = null;

        [SerializeField]
        private RoomController joinRoomWindow = null;
        [SerializeField]
        private NetworkManagerLobby networkManager = null;

        [SerializeField]
        private GameObject mainMenu = null;

        public void OpenCreateRoomWindow()
        {
            // OpenRoomWindow(createRoomWindow);
            networkManager.StartHost();
            // mainMenu.SetActive(false);
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