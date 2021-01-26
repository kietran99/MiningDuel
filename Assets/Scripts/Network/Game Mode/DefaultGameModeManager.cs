using MD.UI;
using Mirror;

namespace MD.Network.GameMode
{
    public abstract class DefaultGameModeManager : IGameModeManager
    {
        protected NetworkManagerLobby networkManager = NetworkManager.singleton as NetworkManagerLobby;

        public virtual void StartHost()
        {
            networkManager.RegisterGameModeManager(this);
            networkManager.StartHost();
        }

        public abstract void HandleOnServerAddPlayer(NetworkConnection conn);
        public abstract void HandleServerChangeScene(NetworkIdentity mapManagerID);
        public abstract bool IsReadyToStart();       
    }
}
