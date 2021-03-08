using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

namespace MD.Character
{
    [RequireComponent(typeof(MoveAction))]
    [RequireComponent(typeof(DigAction))]
    [RequireComponent(typeof(NetworkIdentity))]
    [RequireComponent(typeof(PlayerExplosionHandler))]
    [RequireComponent(typeof(ScoreManager))]
    public class Player : NetworkBehaviour
    {        
        [SerializeField]
        private PlayerColorPicker colorPicker = null;

        [SerializeField]
        private ScoreManager scoreManager = null;

        [SyncVar]
        private string playerName;

        [SyncVar] 
        private bool canMove = false;

        [SyncVar]
        public int colorIdx = 0;

        private Color playerColor;

        private UI.NetworkManagerLobby room;
        private UI.NetworkManagerLobby Room
        {
            get
            {
                if (room != null) return room;
                return room = NetworkManager.singleton as UI.NetworkManagerLobby;
            }
        }
    
        public string PlayerName => playerName;
        public Color PlayerColor => colorPicker.GetColor(colorIdx);
        public bool CanMove => canMove; 
        public int FinalScore => scoreManager.FinalScore;

        public override void OnStartClient()
        {
            DontDestroyOnLoad(this);
            Room.DontDestroyOnLoadObjects.Add(gameObject);
            Room.Players.Add(this);
        }

        public override void OnStopClient()
        {
            Room.Players.Remove(this);           
        }

        public override void OnStartAuthority()
        {
            ServiceLocator.Register(this);
        }
        
        [Server]
        public void SetPlayerNameAndColor(string name)
        {
            playerName = name;
            colorIdx = colorPicker.NextIndex;
        }

        [Server]
        public void Movable(bool value) => canMove = value;

        [TargetRpc]
        public void TargetNotifyGameReady(float time)
        {
            EventSystems.EventManager.Instance.TriggerEvent(new StartGameData());
        }

        [TargetRpc]
        public void TargetNotifyEndGame(bool hasWon)
        {
            EventSystems.EventManager.Instance.TriggerEvent(new EndGameData(hasWon, scoreManager.CurrentScore));
        }

        public void ExitGame()
        {
            if (!hasAuthority) return;
           
            ServiceLocator.Reset();
            if (isServer)
            {
                Debug.Log("Quit match on server");
                NetworkServer.DisconnectAllConnections();
                NetworkManager.singleton.StopHost();
                return;
            }
            
            Debug.Log("Quit match on client");
            NetworkManager.singleton.StopClient();
            (NetworkManager.singleton as UI.NetworkManagerLobby).CleanObjectsOnDisconnect();
            SceneManager.LoadScene(Constants.MAIN_MENU_SCENE_NAME);            
        }       
    }
}