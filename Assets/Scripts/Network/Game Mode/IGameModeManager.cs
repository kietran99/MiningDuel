namespace MD.Network.GameMode
{
    public interface IGameModeManager
    {
        void StartHost();
        void HandleOnServerAddPlayer(Mirror.NetworkConnection conn);
        void HandleServerChangeScene();
        bool IsReadyToStart();
        void SetupGame(float matchTime, System.Collections.Generic.List<Character.Player> players);
    }
}
