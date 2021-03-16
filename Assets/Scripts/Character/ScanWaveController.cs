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
        private float intervalCheckTime;

        [SerializeField]
        private int currentLevel;
        private int maxLevel;

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            currentLevel = 1;
            maxLevel = maxUses*10;
            intervalCheckTime = ReplenishTime/10f;
            StartCoroutine(nameof(Replenish));
        }

        [Command]
        private void CmdSpawnScanWave(NetworkIdentity owner)
        {
            EventSystems.EventManager.Instance.TriggerEvent(new ScanWaveSpawnData(owner));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                if (currentLevel > 10)
                {
                    CmdSpawnScanWave(netIdentity);
                    currentLevel-=10;
                    EventSystems.EventManager.Instance.TriggerEvent(new ScanWaveChangeData(currentLevel, maxLevel));
                }
            }
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
                if (currentLevel == 10 || currentLevel == 20 || currentLevel ==30)
                {
                    EventSystems.EventManager.Instance.TriggerEvent(new ScanWaveChangeData(currentLevel, maxLevel));
                }
            }

        }
    }
}
