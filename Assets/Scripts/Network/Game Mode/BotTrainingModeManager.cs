using MD.Character;
using Mirror;

namespace MD.Network.GameMode
{
    public class BotTrainingModeManager : DefaultGameModeManager
    {
        private Player player;

        public BotTrainingModeManager() : base()
        {
            networkManager.isBotTraining = true;
        }

        // public override void StartHost()
        // {
        //     networkManager.RegisterGameModeManager(this);
        //     networkManager.StartHost();
        // }

        public override void HandleOnServerAddPlayer(NetworkConnection conn)
        {
            this.player = networkManager.SpawnNetworkPlayer(conn);
            networkManager.StartGame();
        }

        public override void HandleServerChangeScene(NetworkIdentity mapManagerID)
        {
            player.TargetRegisterMapManager(mapManagerID);
        }

        public override bool IsReadyToStart()
        {
            return networkManager.numPlayers == 1;
        }

        public override void SetupGame()
        {
            base.SetupGame();
            networkManager.SetupBotState();
        }
    }
}
