using UnityEngine;
using Mirror;

namespace MD.Quirk
{
    [RequireComponent(typeof(Character.DigAction))]
    public class QuirkPouch : NetworkBehaviour
    {
        // private int capacity = 1;
        private System.Collections.Generic.List<BaseQuirk> quirks = new System.Collections.Generic.List<BaseQuirk>();

        public void Insert(BaseQuirk quirk)
        {
            quirks.Add(quirk);
        }

        [Command]
        public void CmdUse()
        {
            quirks[0].RpcActivate();
        }

        [ClientCallback]
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                CmdUse();
            }
        }
    }
}
