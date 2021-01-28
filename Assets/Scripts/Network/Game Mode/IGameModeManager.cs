using Mirror;

namespace MD.Network.GameMode
{
    public interface IGameModeManager
    {
        void StartHost();
        void HandleOnServerAddPlayer(NetworkConnection conn);
        void HandleServerChangeScene(NetworkIdentity mapManagerID);
        bool IsReadyToStart();
        void SetupGame();
    }
}
