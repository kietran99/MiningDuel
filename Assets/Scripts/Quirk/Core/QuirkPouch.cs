using UnityEngine;
using Mirror;

namespace MD.Quirk
{
    [RequireComponent(typeof(Character.DigAction))]
    public class QuirkPouch : NetworkBehaviour
    {
        private int capacity = 1;
        private System.Collections.Generic.List<BaseQuirk> quirks = new System.Collections.Generic.List<BaseQuirk>();

        public bool TryInsert(BaseQuirk quirk)
        {
            if (quirks.Count == capacity)
            {
                Debug.Log("Quirk Pouch: Cannot Carry More Quirk");
                return false;
            }

            quirks.Add(quirk);
            return true;
        }

        [Command]
        public void CmdRequestUse() => RpcTryUse();

        [ClientRpc]
        public void RpcTryUse()
        {
            if (quirks.Count == 0)
            {
                Debug.Log("Quirk Pouch: Player ID " + netId + " Not Holding any Quirk");
                return;
            }

            var idxToUse = 0;
            var quirkToUse = quirks[idxToUse];
            quirkToUse.Activate();
            quirks.RemoveAt(idxToUse);
        }

        [ClientCallback]
        private void Update()
        {
            if (!hasAuthority)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {             
                CmdRequestUse();
            }
        }
    }
}
