using System.Collections.Generic;
using MD.Character;
using Mirror;
using UnityEngine;

namespace MD.Network.GameMode
{
    public class PvPModeManager : AbstractGameModeManager
    {
        private System.Action<CharacterDeathData> deathHandler;

        public PvPModeManager() : base()
        {}
        
        public override void HandleOnServerAddPlayer(NetworkConnection conn)
        {
            networkManager.SpawnRoomPlayer(conn);
        }

        public override void HandleServerChangeScene()
        {
            networkManager.SpawnPvPPlayers();
        }

        public override void SetupGame(float matchTime, List<Player> players)
        {
            base.SetupGame(matchTime, players);
            deathHandler = data => OnPlayerDeath(data, players);
            EventSystems.EventManager.Instance.StartListening<CharacterDeathData>(deathHandler);
        }

        private void OnPlayerDeath(CharacterDeathData data, List<Player> players)
        {
            var eliminatedPlayerId = data.eliminatedId;
            var eliminatedPlayer = players.Find(player => player.netId.Equals(eliminatedPlayerId));
            eliminatedPlayer.TargetNotifyEndGame(false);
            
            if (eliminatedPlayerId == players[0].netId) // Is Host
            {
                players[0].gameObject.SetActive(false);
            }
            else
            {
                NetworkServer.RemovePlayerForConnection(eliminatedPlayer.connectionToClient, true);
            }

            var res = alivePlayerIds.Remove(eliminatedPlayerId);

            if (!res)
            {
                Debug.LogError("Error removing the eliminated Player");
                return;
            }

            var onlyOnePlayerAlive = alivePlayerIds.Count.Equals(1);

            if (!onlyOnePlayerAlive)
            {
                return;
            }

            WinPvPByElimination(alivePlayerIds[0]);
        }

        private void WinPvPByElimination(uint winningPlayerId)
        {
            EventSystems.EventManager.Instance.StopListening<CharacterDeathData>(deathHandler);
            Debug.Log("Player " + winningPlayerId + " won the game");         
            networkManager.Players.Find(player => player.netId.Equals(winningPlayerId)).TargetNotifyEndGame(true);
            StopCountdown();
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
