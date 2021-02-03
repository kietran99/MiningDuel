using System.Collections.Generic;
using Mirror.Discovery;
using UnityEngine;
using Mirror;

namespace MD.UI.MainMenu
{
    public class JoinRoomController : MonoBehaviour
    {
        #region SERIALIZE FIELDS        
        [SerializeField]
        private GameObject roomOrganizer = null, room = null;       
        #endregion
        
        private readonly Dictionary<long, CustomServerResponse> discoveredServers = new Dictionary<long, CustomServerResponse>();
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
            var networkDiscovery = Manager.GetComponent<CustomNetworkDiscovery>();      
            networkDiscovery.OnServerFound.AddListener(OnServerDiscovered);
            discoveredServers.Clear();
            networkDiscovery.StartDiscovery();
        }

        public void OnServerDiscovered(CustomServerResponse info)
        {
            Debug.Log("Discovered a server with ID: " + info.serverId);

            if (discoveredServers.ContainsKey(info.serverId)) return;

            discoveredServers[info.serverId] = info;
            InitRoom(info.EndPoint.Address.ToString(), info.hostName);          
        }

        private void InitRoom(string ipAddress, string hostName)
        {
            var newRoom = Instantiate(room, roomOrganizer.transform);
            newRoom.GetComponent<JoinableRoom>().Init(Manager, ipAddress, hostName);
            rooms.Add(newRoom);
        }

        void OnDisable()
        {
            DestroyAllRooms();
            var networkDiscovery = Manager.GetComponent<CustomNetworkDiscovery>();
            networkDiscovery.OnServerFound.RemoveListener(OnServerDiscovered);            
        }

        private void DestroyAllRooms()
        {
            rooms.ForEach(Destroy); 
            rooms.Clear();  
        }        
    }
}
