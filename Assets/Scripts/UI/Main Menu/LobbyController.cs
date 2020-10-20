using UnityEngine;

namespace MD.UI.MainMenu
{
    public class LobbyController : MonoBehaviour
    {
        [SerializeField]
        private RoomController createRoomWindow = null;

        [SerializeField]
        private RoomController joinRoomWindow = null;

        public void OpenCreateRoomWindow()
        {
            OpenRoomWindow(createRoomWindow);
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