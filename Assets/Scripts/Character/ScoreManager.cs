using MD.Diggable.Gem;
using Mirror;
using UnityEngine;

namespace MD.Character
{
    public class ScoreManager : NetworkBehaviour
    {
        [System.Serializable]
        public struct MultiplierThreshold
        {
            public int score;
            public float multiplier;

            public MultiplierThreshold(int score, float multiplier)
            {
                this.score = score;
                this.multiplier = multiplier;
            }
        }

        [SerializeField]
        private MultiplierThreshold[] multiplierThresholds = null;

        [SyncVar(hook=nameof(SyncScore))]
        private int currentScore;

        private int finalScore;

        [SyncVar(hook=nameof(SyncMultiplier))]
        private float currentMultiplier;

        public int CurrentScore { get => currentScore; }

        public int FinalScore { get => finalScore; }

        public float CurrentMultiplier { get => currentMultiplier; }

        private Player player;

        public override void OnStartServer()
        {
            currentScore = 0;
            currentMultiplier = 1f;
            EventSystems.EventManager.Instance.StartListening<StoreFinishedData>(StoreGem);
            player = GetComponent<Player>();
        }
        public override void OnStopServer()
        {
            EventSystems.EventManager.Instance.StartListening<StoreFinishedData>(StoreGem);
        }

        public override void OnStartClient()
        {
            EventSystems.EventManager.Instance.StartListening<GemObtainData>(HandleGemDug);
            EventSystems.EventManager.Instance.StartListening<DropObtainData>(HandleDropObtain);
            EventSystems.EventManager.Instance.StartListening<ExplodedData>(HandleExplosion);
        }

        public override void OnStopClient()
        {
            EventSystems.EventManager.Instance.StopListening<GemObtainData>(HandleGemDug);
            EventSystems.EventManager.Instance.StopListening<DropObtainData>(HandleDropObtain);
            EventSystems.EventManager.Instance.StopListening<ExplodedData>(HandleExplosion);
        }

        private void HandleGemDug(GemObtainData gemObtainData)
        {
            HandleScoreChangeEvent(gemObtainData.diggerID, CmdIncreaseScore, gemObtainData.value);
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
        private void IncreaseScore(int amount) => SyncScore(currentScore, currentScore + Mathf.FloorToInt(amount * currentMultiplier));       

        [Command]
        private void CmdIncreaseScore(int amount) => SyncScore(currentScore, currentScore + Mathf.FloorToInt(amount * currentMultiplier));        

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
            UpdateMultiplier(currentScore);
            if (!hasAuthority) return;
            
            EventSystems.EventManager.Instance.TriggerEvent(new ScoreChangeData(newValue, finalScore));
        }

        private void UpdateMultiplier(int currentScore)
        {
            // if (currentMultiplier >= multiplierThresholds[multiplierThresholds.Length - 1].multiplier) return;

            var maxMultIdx = multiplierThresholds.LookUp(threshold => threshold.score > currentScore).idx;
            maxMultIdx = maxMultIdx == -1 ? multiplierThresholds.Length : maxMultIdx;
            SyncMultiplier(currentMultiplier, multiplierThresholds[maxMultIdx - 1].multiplier);
        }

        private void SyncMultiplier(float oldMult, float newMult)
        {
            currentMultiplier = newMult;
            if (!hasAuthority) return;

            EventSystems.EventManager.Instance.TriggerEvent(new MultiplierChangeData(newMult));            
        }

        // Cheat codes
        void Update()
        {
            #if UNITY_EDITOR
            if (!hasAuthority) return;
            if (Input.GetKeyDown(KeyCode.I)) CmdIncreaseScore(10);
            else if (Input.GetKeyDown(KeyCode.J)) DecreaseScore(4);
            else if (Input.GetKeyDown(KeyCode.Alpha6)) 
            {
                Debug.Log("[Cheat] Increase Multiplier");
                EventSystems.EventManager.Instance.TriggerEvent(new MultiplierChangeData(UnityEngine.Random.Range(1, 100)));
            }
            #endif
        }

        [Server]
        void StoreGem(StoreFinishedData data)
        {
            if (data.player != player.netIdentity) return;
            finalScore += currentScore;
            currentScore = 0;
        } 
    }
}
