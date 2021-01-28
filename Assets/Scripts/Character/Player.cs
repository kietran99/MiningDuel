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
        private SpriteRenderer indicator = null;

        [SerializeField]
        private ScoreManager scoreManager = null;

        [SyncVar]
        private string playerName;

        [SyncVar] 
        private bool canMove = false;

        private UI.NetworkManagerLobby room;
        private UI.NetworkManagerLobby Room
        {
            get
            {
                if (room != null) return room;
                return room = NetworkManager.singleton as UI.NetworkManagerLobby;
            }
        }
    
        public string PlayerName { get => playerName; }

        public bool CanMove { get => canMove; }

        public int CurrentScore { get => scoreManager.CurrentScore; }

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
            indicator.color = Color.green;
        }
        
        [Server]
        public void SetPlayerName(string name)
        {
            playerName = name;
        }

        [TargetRpc]
        public void TargetRegisterMapManager(NetworkIdentity mapManager)
        {
            ServiceLocator.Register(mapManager.GetComponent<IMapManager>());
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
                // (NetworkManager.singleton as NetworkManagerLobby).CleanObjectsWhenDisconnect();
                return;
            }
            
            Debug.Log("Quit match on client");
            NetworkManager.singleton.StopClient();
            (NetworkManager.singleton as UI.NetworkManagerLobby).CleanObjectsOnDisconnect();
            SceneManager.LoadScene(Constants.MAIN_MENU_SCENE_NAME);            
        }       
    
        [Command]
        public void CmdAssignAuthority(NetworkIdentity id)
        {
            id.AssignClientAuthority(connectionToClient);
        }
    }
}