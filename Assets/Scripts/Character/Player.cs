using UnityEngine;
using Mirror;
using MD.UI;
using UnityEngine.SceneManagement;
namespace MD.Character
{
    [RequireComponent(typeof(MoveAction))]
    [RequireComponent(typeof(DigAction))]
    public class Player : NetworkBehaviour
    {
        [Header("Game Stats")]
        [SyncVar(hook = nameof(OnScoreChange))]
        [SerializeField]
        private int score;

        [SerializeField]
        private SpriteRenderer indicator = null;

        [SyncVar]
        private string playerName;

        [SyncVar] [SerializeField]
        private bool canMove = false;

        // [SyncVar]
        // private bool isReady = true;

        private NetworkManagerLobby room;
        private NetworkManagerLobby Room
        {
            get
            {
                if (room != null) return room;
                return room = NetworkManager.singleton as NetworkManagerLobby;
            }

        }
        private IScoreManager scoreManager = null;
        private IScoreManager ScoreManager
        {
            get
            {
                if (scoreManager != null) return scoreManager;
                ServiceLocator.Resolve(out scoreManager);
                return scoreManager;
            }
        }
        
        public override void OnStartServer()
        {
            base.OnStartServer();
            score = 0;
        }

        public override void OnStartClient()
        {
            DontDestroyOnLoad(this);
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
        public void TargetRegisterIMapManager(NetworkIdentity mapManager)
        {
            ServiceLocator.Register(mapManager.GetComponent<IMapManager>());
        }
        
        [Server]
        public void IncreaseScore(int amount)
        {
            score += amount;
        }

        [Server]
        public void DecreaseScore(int amount)
        {
            score -= amount;
            score = score < 0 ? 0 : score;
        }

        public void OnScoreChange(int oldValue, int newValue)
        {
            if (!isLocalPlayer) return;
            ScoreManager.UpdateScoreText(newValue);
        }

        public int GetCurrentScore() => score;

        [Server]
        public void SetCanMove(bool value) => canMove=value;

        [TargetRpc]
        public void TargetNotifyGameReady(float time)
        {
            IGameCountDown countDown;
            if (ServiceLocator.Resolve<IGameCountDown>(out countDown)) countDown.StartCountDown(0f);
            SceneManager.MoveGameObjectToScene(this.gameObject, SceneManager.GetActiveScene());
        }

        [TargetRpc]
        public void TargetNotifyEndGame(bool hasWon)
        {
            EventSystems.EventManager.Instance.TriggerEvent(new EndGameData(hasWon,score));
        }

        public void ExistGame()
        {
            room.StopHost();
            ServiceLocator.Reset();
        }

        public bool CanMove() => canMove;
    }
}