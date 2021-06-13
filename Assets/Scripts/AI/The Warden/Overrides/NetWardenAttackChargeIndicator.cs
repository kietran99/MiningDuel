using UnityEngine;
using Mirror;

namespace MD.AI.TheWarden
{
    public class NetWardenAttackChargeIndicator : NetworkBehaviour, IWardenAttackChargeIndicator
    {
        [SerializeField]
        private GameObject indicator = null;

        [Server]
        public void Hide() => RpcHide();

        [Server]
        public void Scale(float scale) => RpcScale(scale);

        [Server]
        public void Show() => RpcShow();

        [ClientRpc]
        private void RpcHide()
        {
            indicator.gameObject.SetActive(false);
        }

        [ClientRpc]
        private void RpcScale(float scale)
        {
            indicator.transform.localScale = new Vector3(scale, scale, 1f);
        }

        [ClientRpc]
        private void RpcShow()
        {
            indicator.gameObject.SetActive(true);
            Scale(1f);
        }
    }
}
