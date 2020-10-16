using UnityEngine;

namespace MD.UI.MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private RoomController roomController = null;

        public void OpenRoomWindow()
        {
            roomController.ShowWindow();
        }
    }
}