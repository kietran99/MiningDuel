using System.Collections;
using UnityEngine;
using Mirror;
using EventSystems;

namespace MD.Character
{
    public class ScanWaveController : NetworkBehaviour
    {
        [SerializeField]
        private int MAX_USES = 3;

        [SerializeField]
        private float ReplenishTime = 5f;

        [SerializeField]
        private float intervalCheckTime = .5f;
        
        [SerializeField]
        private int uses = 0;

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            uses = MAX_USES;
            StartCoroutine(nameof(Replenish));
        }

        [Command]
        private void CmdSpawnScanWave(NetworkIdentity owner)
        {
            EventManager.Instance.TriggerEvent(new ScanWaveSpawnData(owner));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                if (uses > 0)
                {
                    CmdSpawnScanWave(netIdentity);
                    uses--;
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
            int level = 1;
            while (true)
            {
                yield return checkTimeWFS;
                if (uses >= MAX_USES) continue;
                //play animation

                level++;
                if (level >=10)
                {
                    level = 0;
                    uses++;
                }
            }

        }
    }
}
