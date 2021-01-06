using MD.Diggable;
using Mirror;
using UnityEngine;

namespace MD.Character
{
    public class ScoreManager : NetworkBehaviour
    {
        [SyncVar(hook = nameof(SyncScore))]
        private int currentScore;

        public int CurrentScore { get => currentScore; }

        public override void OnStartServer()
        {
            currentScore = 0;
        }

        public override void OnStartClient()
        {
            EventSystems.EventManager.Instance.StartListening<GemDigSuccessData>(HandleGemDigSuccess);
            EventSystems.EventManager.Instance.StartListening<ExplodedData>(HandleExplosion);
        }

        public override void OnStopClient()
        {
            EventSystems.EventManager.Instance.StopListening<GemDigSuccessData>(HandleGemDigSuccess);
            EventSystems.EventManager.Instance.StopListening<ExplodedData>(HandleExplosion);
        }

        private void HandleGemDigSuccess(GemDigSuccessData gemDigSuccessData)
        {
            if (!gemDigSuccessData.diggerID.Equals(netId)) return;

            CmdIncreaseScore(gemDigSuccessData.value);
        }

        private void HandleExplosion(ExplodedData explodedData)
        {
            if (!explodedData.explodedPlayerId.Equals(netId)) return;

            TargetDecreaseScore(explodedData.dropAmount);
        }

        [Command]
        private void CmdIncreaseScore(int amount)
        {
            currentScore += amount;
        }

        [TargetRpc]
        private void TargetDecreaseScore(int amount)
        {
            var oldScore = currentScore;
            currentScore -= amount;
            currentScore = currentScore < 0 ? 0 : currentScore;
            SyncScore(oldScore, currentScore);
        }

        private void SyncScore(int oldValue, int newValue)
        {
            currentScore = newValue;

            if (!hasAuthority) return;

            EventSystems.EventManager.Instance.TriggerEvent(new ScoreChangeData(newValue));
        }

        void Update()
        {
            #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F)) CmdIncreaseScore(10);
            #endif
        }
    }
}
