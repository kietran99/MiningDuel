﻿using System.Collections.Generic;
using MD.AI;
using MD.Character;
using Mirror;
using UnityEngine;

namespace MD.Network.GameMode
{
    public class BotTrainingModeManager : AbstractGameModeManager
    {
        private Player player;

        private List<uint> aliveBots = new List<uint>();

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

        public override void SetupGame(float matchTime, List<Character.Player> players)
        {
            base.SetupGame(matchTime, players);
            SetupPlayerState(player);
            SetupBotState(players);
            EventSystems.EventManager.Instance.StartListening<CharacterDeathData>(LoseBotTrainingByElimination);
        }

        private void SetupPlayerState(Player player)
        {
            player.transform.position = networkManager.NextSpawnPoint;       
        }

        private void SetupBotState(List<Character.Player> players)
        {
            var bot = networkManager.SpawnBot(networkManager.NextSpawnPoint);
            aliveBots.Add(bot.GetComponent<NetworkIdentity>().netId);
            bot.GetComponent<AI.BotHitPoints>().OnBotDeath += HandleBotEliminated;  
        }

        private void HandleBotEliminated(uint botId)
        {
            var res = aliveBots.Remove(botId);

            if (!res)
            {
                Debug.LogError("Error removing the eliminated Bot");
                return;
            }

            var onlyThePlayerAlive = aliveBots.Count == 0;

            if (!onlyThePlayerAlive)
            {
                return;
            }

            WinBotTrainingByElimination();        
        }

        private void WinBotTrainingByElimination()
        {
            StopListeningToDeathEvent();
            player.TargetNotifyEndGame(true);          
            StopCountdown();        
        }

        private void LoseBotTrainingByElimination(CharacterDeathData data)
        {
            StopListeningToDeathEvent();
            player.TargetNotifyEndGame(false);          
            StopCountdown();
        }

        public override void EndGameByTimeOut(List<Player> players, List<PlayerBot> bots)
        {
            base.EndGameByTimeOut(players, bots);
            StopListeningToDeathEvent();
        }

        private void StopListeningToDeathEvent() => EventSystems.EventManager.Instance.StopListening<CharacterDeathData>(LoseBotTrainingByElimination);
    }
}
