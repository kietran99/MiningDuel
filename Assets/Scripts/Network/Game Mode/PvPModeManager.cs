using Mirror;

namespace MD.Network.GameMode
{
    public class PvPModeManager : DefaultGameModeManager
    {
        public PvPModeManager() : base()
        {
        }

        public override void HandleOnServerAddPlayer(NetworkConnection conn)
        {
            networkManager.SpawnRoomPlayer(conn);
        }

        public override void HandleServerChangeScene(NetworkIdentity mapManagerID)
        {
            networkManager.SpawnPvPPlayers();
        }

        public override bool IsReadyToStart()
        {
            if (networkManager.numPlayers < networkManager.MinNumPlayers) return false;

            foreach (var player in networkManager.RoomPlayers)
            {
                if (!player.isReady) return false;
            }

            return true;
        }
    }
}
