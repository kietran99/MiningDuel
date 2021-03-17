using MD.Character;
using Mirror;
using UnityEngine;

namespace MD.Network.GameMode
{
    public class BotTrainingModeManager : DefaultGameModeManager
    {
        private Player player;

        public BotTrainingModeManager() : base()
        {
            networkManager.isBotTraining = true;
        }

        public override void HandleOnServerAddPlayer(NetworkConnection conn)
        {
            player = networkManager.SpawnBotTrainingPlayer(conn);
            networkManager.StartGame();
        }

        public override void HandleServerChangeScene()
        {
            ServiceLocator
                .Resolve<Map.Core.IMapGenerator>()
                .Match(
                    err => Debug.LogError(err.Message),
                    mapGenerator => player.transform.position += new Vector3(mapGenerator.MapWidth / 2, mapGenerator.MapHeight / 2, 0f)
                );
        }

        public override bool IsReadyToStart()
        {
            return networkManager.numPlayers == 1;
        }

        public override void SetupGame()
        {
            base.SetupGame();
            networkManager.SetupBotAndPlayerState(player.transform);
        }
    }
}
