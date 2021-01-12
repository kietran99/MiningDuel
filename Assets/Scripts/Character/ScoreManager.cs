using MD.Diggable;
using MD.Diggable.Gem;
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
            EventSystems.EventManager.Instance.StartListening<DropObtainData>(HandleDropObtain);
            EventSystems.EventManager.Instance.StartListening<ExplodedData>(HandleExplosion);
        }

        public override void OnStopClient()
        {
            EventSystems.EventManager.Instance.StopListening<GemDigSuccessData>(HandleGemDigSuccess);
            EventSystems.EventManager.Instance.StopListening<DropObtainData>(HandleDropObtain);
            EventSystems.EventManager.Instance.StopListening<ExplodedData>(HandleExplosion);
        }

        private void HandleGemDigSuccess(GemDigSuccessData gemDigSuccessData)
        {
            HandleScoreChangeEvent(gemDigSuccessData.diggerID, CmdIncreaseScore, gemDigSuccessData.value);
        }

        private void HandleExplosion(ExplodedData explodedData)
        {
            HandleScoreChangeEvent(explodedData.explodedTargetID, DecreaseScore, explodedData.dropAmount);
        }

        private void HandleDropObtain(DropObtainData dropObtainData)
        {            
            HandleScoreChangeEvent(dropObtainData.pickerID, IncreaseScore, dropObtainData.value);
        }

        private void HandleScoreChangeEvent(uint targetID, System.Action<int> handler, int changeValue)
        {
            if (!IsLocalTarget(targetID)) return;

            handler(changeValue);
        }

        private bool IsLocalTarget(uint targetID) => targetID.Equals(netId);

        [Server]
        private void IncreaseScore(int amount)
        {                     
            SyncScore(currentScore, currentScore + amount);        
        }

        [Command]
        private void CmdIncreaseScore(int amount)
        {                     
            SyncScore(currentScore, currentScore + amount);        
        }

        [Server]
        private void DecreaseScore(int amount)
        {
            var newScore = currentScore - amount;
            newScore = newScore < 0 ? 0 : newScore;
            SyncScore(currentScore, newScore);
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
            if (Input.GetKeyDown(KeyCode.I)) CmdIncreaseScore(10);
            else if (Input.GetKeyDown(KeyCode.J)) DecreaseScore(4);
            #endif
        }
    }
}
