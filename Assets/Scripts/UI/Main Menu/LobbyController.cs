using UnityEngine;
using Mirror;
using MD.Network.GameMode;

namespace MD.UI.MainMenu
{
    public class LobbyController : MonoBehaviour
    {
        // [SerializeField]
        // private RoomController createRoomWindow = null;

        [SerializeField]
        private RoomController joinRoomWindow = null;

        // [Scene]
        // [SerializeField]
        // private string tutorialScene = null;

        [SerializeField]
        private GameObject tutorial = null;

        private NetworkManagerLobby netManager;
        private NetworkManagerLobby NetManager
        {
            get
            {
                if (netManager != null) return netManager;
                return netManager = NetworkManager.singleton as NetworkManagerLobby;
            }
        }

        public void OpenCreateRoomWindow()
        {
            // OpenRoomWindow(createRoomWindow);
            //NetManager.StartHost();
            IGameModeManager gameModeManager = new PvPModeManager();
            gameModeManager.StartHost();
            NetManager.GetComponent<CustomNetworkDiscovery>().AdvertiseServer(PlayerPrefs.GetString(PlayerNameInput.PLAYER_PREF_NAME_KEY));
        }

        public void OpenJoinRoomWindow()
        {
            OpenRoomWindow(joinRoomWindow);
        }

        private void OpenRoomWindow(RoomController window)
        {
            window.ShowWindow();            
        }

        // public void EnterTutorial()
        // {
        //     UnityEngine.SceneManagement.SceneManager.LoadScene(tutorialScene);
        // }

        public void OpenTutorial()
        {
            tutorial.SetActive(true);
        }
    }
}