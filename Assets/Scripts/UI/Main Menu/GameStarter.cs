using MD.Network.GameMode;
using MD.UI;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class GameStarter : MonoBehaviour
{
    [SerializeField]
    private Button startButton = null;

    private bool isStarted = false;
    
    private NetworkManagerLobby room;
    private NetworkManagerLobby Room
    {
        get
        {
            if (room != null) return room;
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }

    private void Start()
    {
        startButton.onClick.AddListener(StartGame);
    }

    private void OnDestroy()
    {
        startButton.onClick.RemoveListener(StartGame);
    }

    private void StartGame()
    {
        if (isStarted) return;
        isStarted = true;
        //Room.StartBotTraining();
        IGameModeManager gameModeManager = new BotTrainingModeManager();
        gameModeManager.StartHost();
    }
}
