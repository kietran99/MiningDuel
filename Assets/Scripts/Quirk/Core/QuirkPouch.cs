using UnityEngine;
using UnityEngine.SceneManagement;
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

            quirk.transform.SetParent(transform);
            quirks.Add(quirk);
            return true;
        }

        [Command]
        public void CmdRequestUse(int idxToUse)
        {
            if (quirks.Count == 0)
            {
                Debug.Log("Quirk Pouch: Player with ID " + netId + " is Not Holding any Quirk");
                return;
            }

            RpcTryUse(idxToUse);
        }

        [ClientRpc]
        private void RpcTryUse(int idxToUse)
        {
            var quirkToUse = quirks[idxToUse];
            // Obtained quirk is a child of Player GO & Player GO is a DontDestroyOnLoad GO 
            // -> Move obtained quirk from Dont Destroy On Load Scene to Multiplayer scene
            quirkToUse.transform.SetParent(null);
            SceneManager.MoveGameObjectToScene(quirkToUse.gameObject, SceneManager.GetActiveScene()); 

            quirkToUse.Activate(netIdentity);

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
                CmdRequestUse(0);
            }
        }
    }
}
