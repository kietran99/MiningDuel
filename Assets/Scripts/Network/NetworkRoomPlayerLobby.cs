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
        private Text[] playerNameTexts = new Text[4], playerReadyStatusTexts = new Text[4];

        [SerializeField]
        private CanvasGroup[] playerPanels = null;

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
            if (!isHost) return;
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
            
            for (int i = 0; i < playerNameTexts.Length; i++)
            {
                playerNameTexts[i].text = "Waitting for Player...";
                playerReadyStatusTexts[i].text = string.Empty;
                playerPanels[i].alpha = .5f;
            }

            for (int i = 0; i < Room.RoomPlayers.Count; i++)
            {
                playerNameTexts[i].text = Room.RoomPlayers[i].DisplayName;
                playerReadyStatusTexts[i].text = Room.RoomPlayers[i].isReady ? "Ready" : "Standby";
                playerPanels[i].alpha = 1f;
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
