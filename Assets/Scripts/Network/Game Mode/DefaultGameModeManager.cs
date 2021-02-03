using MD.UI;
using Mirror;

namespace MD.Network.GameMode
{
    public abstract class DefaultGameModeManager : IGameModeManager
    {
        protected NetworkManagerLobby networkManager = NetworkManager.singleton as NetworkManagerLobby;
        protected float matchTime = 120f;

        public virtual void StartHost()
        {
            networkManager.RegisterGameModeManager(this);
            networkManager.StartHost();
        }

        public virtual void HandleServerChangeScene() {}

        public virtual void SetupGame()
        {
            networkManager.SetupPlayerState(matchTime);
        }

        public abstract void HandleOnServerAddPlayer(NetworkConnection conn);
        public abstract bool IsReadyToStart();
    }
}
