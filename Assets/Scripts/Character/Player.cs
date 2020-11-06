using UnityEngine;
using Mirror;

namespace MD.Character
{
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

        // [SyncVar]
        // private bool canMove = true;

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

        public void OnScoreChange(int oldValue, int newValue)
        {
            if (isLocalPlayer)
                ScoreManager.UpdateScoreText(newValue);
        }
        public int GetCurrentScore()
        {
            return score;
        }

    }
}