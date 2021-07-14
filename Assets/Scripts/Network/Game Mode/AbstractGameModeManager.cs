using System.Collections.Generic;
using System.Linq;
using MD.Character;
using MD.UI;
using Mirror;
using UnityEngine;

namespace MD.Network.GameMode
{
    public abstract class AbstractGameModeManager : IGameModeManager
    {
        protected NetworkManagerLobby networkManager = NetworkManager.singleton as NetworkManagerLobby;

        protected List<uint> alivePlayerIds = new List<uint>();

        public virtual void StartHost()
        {
            networkManager.RegisterGameModeManager(this);
            networkManager.StartHost();
        }

        public virtual void HandleServerChangeScene() {}

        public virtual void SetupGame(float matchTime, List<Player> players)
        {
            Time.timeScale = 1f; 

            alivePlayerIds = new List<uint>(players.Map(player => player.netId));

            foreach (var player in players)
            {
                player.Movable(true);
                player.TargetNotifyGameReady(matchTime);
            }
        }

        protected void StopCountdown()
        {
            networkManager.CancelInvoke();
            Time.timeScale = 0f;
        }

        public virtual void EndGameByTimeOut(List<Character.Player> players, List<AI.PlayerBot> bots)
        {
            if (players.Count <= 0) 
            {
                return;
            }

            Time.timeScale = 0f;
            
            if (bots.Count > 0)
            {
                players[0].TargetNotifyEndGame(players[0].FinalScore >= bots[0].CurrentScore);
                return;
            }
            
            players.ForEach(player => player.Movable(false));
            var orderedPlayers = players.OrderBy(player => -player.FinalScore).ToList<Player>();
            int highestScore = orderedPlayers[0].FinalScore;
            orderedPlayers[0].TargetNotifyEndGame(true);
            foreach (Player player in orderedPlayers.Skip(1))
            {
                if (player.FinalScore == highestScore)
                {
                    //tied
                    player.TargetNotifyEndGame(true);
                    continue;
                }

                player.TargetNotifyEndGame(false);
            }
        }

        public abstract void HandleOnServerAddPlayer(NetworkConnection conn);
        public abstract bool IsReadyToStart();
    }
}
