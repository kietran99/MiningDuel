using System.Collections;
using UnityEngine;
using Mirror;

namespace MD.Character
{
    public class ScanWaveController : NetworkBehaviour
    {
        [SerializeField]
        private int maxUses = 3;

        [SerializeField]
        private float ReplenishTime = 5f;

        [SerializeField]
        private int currentLevel;

        private float intervalCheckTime;
        private int maxLevel;

        private bool isStunned = false;

        public override void OnStartAuthority()
        {
            currentLevel = 1;
            maxLevel = maxUses * 10;
            intervalCheckTime = ReplenishTime / 10f;
            StartCoroutine(nameof(Replenish));
            EventSystems.EventManager.Instance.StartListening<ScanInvokeData>(Scan);
            EventSystems.EventManager.Instance.StartListening<StunStatusData>(HandleStunStatusChange);
        }

        void OnDisable()
        {
            StopAllCoroutines();
            if (hasAuthority)
            {
                EventSystems.EventManager.Instance.StopListening<ScanInvokeData>(Scan);
                EventSystems.EventManager.Instance.StopListening<StunStatusData>(HandleStunStatusChange);
            }
        }


        private void HandleStunStatusChange (StunStatusData data) => isStunned = data.isStunned;

        private void Scan(ScanInvokeData data) 
        {
            if (isStunned || currentLevel <= 10)
            {
                return;
            }
            
            CmdSpawnScanWave(netIdentity);
            currentLevel -= 10;
            EventSystems.EventManager.Instance.TriggerEvent(new ScanWaveChangeData(currentLevel, maxLevel));          
        }

        [Command]
        private void CmdSpawnScanWave(NetworkIdentity owner)
        {
            EventSystems.EventManager.Instance.TriggerEvent(new ScanWaveSpawnData(owner));
        }

        private IEnumerator Replenish()    
        {
            WaitForSeconds checkTimeWFS = new WaitForSeconds(intervalCheckTime);
            currentLevel = maxLevel;
            while (true)
            {
                yield return checkTimeWFS;

                if (currentLevel >= maxLevel) continue;

                //play animation
                currentLevel++;

                if (currentLevel == 10 || currentLevel == 20 || currentLevel == 30)
                {
                    EventSystems.EventManager.Instance.TriggerEvent(new ScanWaveChangeData(currentLevel, maxLevel));
                }
            }
        }
    }
}
