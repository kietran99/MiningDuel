using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Net;
using System.Net.Sockets;

namespace MD.UI
{
    public class NetworkRoomPlayerLobby : NetworkBehaviour
    {
        #region FIELDS
        [Header("UI")]

        [SerializeField]
        GameObject lobbyUI = null;

        [SerializeField]
        private ParticipantSlot[] participantSlots = null;

        [SerializeField]
        private Button startGameButton = null, readyButton = null;

        [SerializeField]
        private Color readyColor = Color.green, standbyColor = Color.red;

        [SerializeField]
        private Text readyButtonText = null;
        
        [SyncVar(hook = nameof(SyncDisplayName))]
        public string DisplayName = "Loading....";

        [SyncVar(hook = nameof(SyncPlayerStatus))]
        public bool isReady = true;

        [SyncVar]
        public int charaterIndex = 0;

        private NetworkManagerLobby room;
        private NetworkManagerLobby Room
        {
            get
            {
                room = room ?? NetworkManager.singleton as NetworkManagerLobby;
                return room;
            }   
        }

        private bool isHost;
        public bool IsHost
        {
            set
            {
                isHost = value;
                if (!isHost) return;

                startGameButton.gameObject.SetActive(true);
                readyButton.gameObject.SetActive(false);
                isReady = true;           
            }
        }
        #endregion

        public override void OnStartAuthority()
        {
            CmdSetDisplayName(PlayerNameInput.DisplayName);
            lobbyUI.SetActive(true);
        }

        public string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new System.Exception("No network adapters with an IPv4 address in the system!");
        }

        public override void OnStartClient()
        {
            Room.RoomPlayers.Add(this);
            UpdateDisplay();
        }

        public override void OnStopClient()
        {
            Room.RoomPlayers.Remove(this);
            UpdateDisplay();
        }

        private void SyncDisplayName(string oldValue, string newValue) 
        {
            name = newValue;
            UpdateDisplay();
        }

        private void SyncPlayerStatus(bool oldValue, bool newValue)
        {
            isReady = newValue;
            ToggleReadyButtonState();   
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (!hasAuthority)
            {
                foreach (var player in Room.RoomPlayers)
                {
                    if (player.hasAuthority)
                    {
                        player.UpdateDisplay();
                        break;
                    }
                }

                return;
            }
            
            participantSlots.ForEach(slot => slot.ToggleEmptyState());

            for (int i = 0; i < Room.RoomPlayers.Count; i++)
            {
                participantSlots[i].ToggleOccupiedState(Room.RoomPlayers[i].DisplayName, Room.RoomPlayers[i].isReady);
            }
        }

        public void HandleReadyToStart(bool readyToStart)
        {
            if (!isHost) return;

            startGameButton.interactable = readyToStart;
        }

        public void ExitLobby()
        {
            if (netIdentity == room.RoomPlayers[0].netIdentity)
            {
                EventSystems.EventManager.Instance.TriggerEvent(new RoomWindowToggleData(false));
                room.StopHost();
            }
            else
            {
                room.StopClient();
            }
        }

        [Command]
        public void CmdSetDisplayName(string displayName)
        {
            DisplayName = displayName;
        }

        [Command]
        public void CmdReadyUp()
        {
            isReady = !isReady;                  
            Room.NotifyReadyState();
        }

        public void ToggleReadyButtonState()
        {
            readyButton.GetComponent<Image>().color = isReady ? standbyColor : readyColor;
            readyButtonText.text = isReady ? "Standby" : "Ready";
        }

        [Command]
        public void CmdStartGame()
        {
            if (!Room.RoomPlayers[0].connectionToClient.Equals(connectionToClient)) 
            {
                return;
            }

            if (Room.IsReadyToStart())
            {
                room.StartLobby();
            }
        }

        [Command]
        public void ChangeCharacter(int index)
        {
            //validate
            charaterIndex = index;
        }
    }
}
