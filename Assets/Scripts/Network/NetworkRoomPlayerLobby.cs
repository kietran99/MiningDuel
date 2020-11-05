using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class NetworkRoomPlayerLobby : NetworkBehaviour
{
    [Header("UI")]

    [SerializeField]
    GameObject lobbyUI = null;

    [SerializeField]
    private Text[] playerNameTexts = new Text[4], playerReadyStatusTexts = new Text[4];

    [SerializeField]
    private Button startGameButton = null, readyButton = null;
    
    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading....";

    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
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

    public override void OnStartAuthority()
    {
        CmdSetDisplayName(PlayerNameInput.DisplayName);
        lobbyUI.SetActive(true);
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

    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();
    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();

    private void UpdateDisplay()
    {
        if (!hasAuthority)
        {
            foreach(var player in Room.RoomPlayers)
            {
                if (player.hasAuthority)
                {
                    player.UpdateDisplay();
                    break;
                }
            }

            return;
        }

        for (int i= 0; i< playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Waitting for Player...";
            playerReadyStatusTexts[i].text = string.Empty;
        }

        for (int i=0; i< Room.RoomPlayers.Count; i++)
        {
            playerNameTexts[i].text = Room.RoomPlayers[i].DisplayName;
            playerReadyStatusTexts[i].text = Room.RoomPlayers[i].isReady? "Ready":"";
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
        Room.NotifyPlayersOfReadyState();
    }

    [Command]
    public void CmdStartGame()
    {
        if (Room.RoomPlayers[0].connectionToClient != connectionToClient) return;
        if (Room.IsReadyToStart())
            room.StartGame();
    }

    [Command]
    public void ChangeCharacter(int index)
    {
        //validate
        charaterIndex = index;
    }
}
