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

        public override void OnStartAuthority()
        {
            currentLevel = 1;
            maxLevel = maxUses * 10;
            intervalCheckTime = ReplenishTime / 10f;
            StartCoroutine(nameof(Replenish));
            EventSystems.EventManager.Instance.StartListening<ScanInvokeData>(Scan);
        }

        void OnDestroy()
        {
            if (hasAuthority)
                EventSystems.EventManager.Instance.StopListening<ScanInvokeData>(Scan);
        }

        private void Scan(ScanInvokeData data) 
        {
            if (currentLevel <= 10)
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

        private void OnDisable()
        {
            StopAllCoroutines();
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
