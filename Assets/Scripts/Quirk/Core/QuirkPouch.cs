using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using MD.UI;

namespace MD.Quirk
{
    [RequireComponent(typeof(Character.DigAction))]
    public class QuirkPouch : NetworkBehaviour
    {
        private int capacity = 1;
        private System.Collections.Generic.List<BaseQuirk> quirks = new System.Collections.Generic.List<BaseQuirk>();

        public override void OnStartAuthority()
        {
            EventSystems.EventManager.Instance.StartListening<UI.QuirkInvokeData>(HandleQuirkInvokeEvent);
        }

        void OnDestroy()
        {
            if (hasAuthority)
                EventSystems.EventManager.Instance.StopListening<UI.QuirkInvokeData>(HandleQuirkInvokeEvent);
        }

        private void HandleQuirkInvokeEvent(QuirkInvokeData quirkInvokeData)
        {
            CmdRequestUse(quirkInvokeData.idx);
        }

        public bool TryInsert(BaseQuirk quirk)
        {
            if (quirks.Count == capacity)
            {
                Debug.Log("Quirk Pouch: Cannot Carry More Quirk");
                return false;
            }

            if (hasAuthority)
            {
                EventSystems.EventManager.Instance.TriggerEvent(new QuirkObtainData(quirk.ObtainSprite, quirk.GetDescription()));
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

            RpcMoveQuirkGOToScene(idxToUse);
            RpcSyncActivate(idxToUse);
            TargetActivate(idxToUse);
            RemoveAt(idxToUse);
        }

        [ClientRpc]
        private void RpcMoveQuirkGOToScene(int idx)
        {
            var quirkToUse = quirks[idx];
            // Obtained quirk is a child of Player GO & Player GO is a DontDestroyOnLoad GO 
            // -> Move obtained quirk from Dont Destroy On Load Scene to Multiplayer scene
            quirkToUse.transform.SetParent(null);
            SceneManager.MoveGameObjectToScene(quirkToUse.gameObject, SceneManager.GetActiveScene()); 
        }

        [ClientRpc]
        private void RpcSyncActivate(int idx) => quirks[idx].SyncActivate(netIdentity);

        [TargetRpc]
        private void TargetActivate(int idx) => quirks[idx].SingleActivate(netIdentity);

        [ClientRpc]
        private void RemoveAt(int idx) => quirks.RemoveAt(idx);

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
