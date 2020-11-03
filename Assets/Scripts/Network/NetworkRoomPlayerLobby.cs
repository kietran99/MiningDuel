using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class NetworkRoomPlayerLobby : NetworkBehaviour
{
    [Header("UI")]

    [SerializeField]
    GameObject lobbyUI = null;
    [SerializeField]
    private Text[] playerNameTexts = new Text[4];
    [SerializeField]
    private Text[] playerReadyStatusTexts = new Text[4];
    [SerializeField]
    private Button startGameButton;
    [SerializeField]
    private Button readyButton;
    public bool isHost;
    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading....";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool isReady = true;

    private NetworkManagerLobby room;
    private NetworkManagerLobby Room
    {
        get
        {
            if (room != null) return room;
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    
    }
    public bool IsHost
    {
        set
        {
            isHost = value;
            if (isHost)
            {
                startGameButton.gameObject.SetActive(true);
                readyButton.gameObject.SetActive(false);
                isReady = true;
            }

        }
    }

    public override void OnStartAuthority()
    {
        CmdSetDisplayName(PlayerNameInput.DisplayName);
        lobbyUI.SetActive(true);
    }

    public override void OnStartClient()
    {
        Room.roomPlayers.Add(this);
        UpdateDisplay();
    }

    public override void OnStopClient()
    {
        Room.roomPlayers.Remove(this);
        UpdateDisplay();
    }

    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();
    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();

    private void UpdateDisplay()
    {
        if (!hasAuthority)
        {
            foreach(var player in Room.roomPlayers)
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
        for (int i=0; i< Room.roomPlayers.Count; i++)
        {
            playerNameTexts[i].text = Room.roomPlayers[i].DisplayName;
            playerReadyStatusTexts[i].text = Room.roomPlayers[i].isReady? "Ready":"";
        }
    }

    public void HandleReadyToStart(bool readyToStart)
    {
        if (!isHost) return;
        startGameButton.interactable = readyToStart;
    }

    public void ExitLobby()
    {
        if (netIdentity == room.roomPlayers[0].netIdentity)
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
        if (Room.roomPlayers[0].connectionToClient != connectionToClient) return;
        if (Room.isReadyToStart())
            Debug.Log("start game");
    }
}
