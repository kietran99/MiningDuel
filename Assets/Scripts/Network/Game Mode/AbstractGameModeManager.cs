using System.Collections.Generic;
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

        public abstract void HandleOnServerAddPlayer(NetworkConnection conn);
        public abstract bool IsReadyToStart();
    }
}
