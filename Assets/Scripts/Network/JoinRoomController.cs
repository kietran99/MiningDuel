using System.Collections.Generic;
using Mirror.Discovery;
using UnityEngine;
using Mirror;
namespace MD.UI.MainMenu
{
    public class JoinRoomController : MonoBehaviour
    {
        #region SERIALIZE FIELDS
        // [SerializeField]
        // private NetworkManagerLobby networkManager = null;

        // [SerializeField]
        // private NetworkDiscovery networkDiscovery = null;

        [SerializeField]
        private GameObject roomOrganizer = null, room = null;       
        #endregion
        
        private readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();
        private List<GameObject> rooms = new List<GameObject>();
        private NetworkManagerLobby manager;
        private NetworkManagerLobby Manager
        {
            get
            {
                if (manager != null) return manager;
                return manager = NetworkManager.singleton as NetworkManagerLobby;
            }

        }

        void OnEnable()
        {
            //NetworkManagerLobby.OnClientConnected += HandleClientConnected;
            //NetworkManagerLobby.OnClientDisconnnected += HandleClientDisconnected;

            Manager.GetComponent<NetworkDiscovery>().OnServerFound.AddListener(OnDiscoveredServer);
            discoveredServers.Clear();
            Manager.GetComponent<NetworkDiscovery>().StartDiscovery();
        }

        public void OnDiscoveredServer(ServerResponse info)
        {
            Debug.Log("Discovered a server with ID: " + info.serverId);

            if (discoveredServers.ContainsKey(info.serverId)) return;

            discoveredServers[info.serverId] = info;
            InitRoom(info.EndPoint.Address.ToString());          
        }

        private void InitRoom(string ipAddress)
        {
            var newRoom = Instantiate(room, roomOrganizer.transform);
            newRoom.GetComponent<JoinableRoom>().Init(Manager, ipAddress);
            rooms.Add(newRoom);
        }

        void OnDisable()
        {
            //NetworkManagerLobby.OnClientConnected -= HandleClientConnected;
            //NetworkManagerLobby.OnClientDisconnnected -= HandleClientDisconnected;
            Manager.GetComponent<NetworkDiscovery>().OnServerFound.RemoveListener(OnDiscoveredServer);
            DestroyAllRooms();
        }

        private void DestroyAllRooms()
        {
            rooms.ForEach(Destroy); 
            rooms.Clear();  
        }        

        // public void JoinLobby()
        // {
        //     if (ipAddressInputField.text.Length == 0) return;

        //     string ipAddress = ipAddressInputField.text;
        //     networkManager.networkAddress = ipAddress;
        //     joinButton.interactable  = false;
        //     networkManager.StartClient();
        // }

        // private void HandleClientConnected()
        // {
        //     if (joinButton) joinButton.interactable = true;
        //     // if (CreateRoomWindow) CreateRoomWindow.GetComponent<RoomController>().ShowWindow();
        // }

        // private void HandleClientDisconnected()
        // {
        //     if (joinButton)
        //     joinButton.interactable = true;
        // }
    }
}
