using UnityEngine;
using Mirror;

namespace MD.AI.TheWarden
{
    public class NetWardenAttackChargeIndicator : NetworkBehaviour, IWardenAttackChargeIndicator
    {
        [SerializeField]
        private BaseWardenAttackChargeIndicator baseIndicator = null;

        [Server]
        public void Hide() => RpcHide();

        [Server]
        public void Scale(float scale) => RpcScale(scale);

        [Server]
        public void Show() => RpcShow();

        [ClientRpc]
        private void RpcHide() => baseIndicator.Hide();

        [ClientRpc]
        private void RpcScale(float scale) => baseIndicator.Scale(scale);

        [ClientRpc]
        private void RpcShow() => baseIndicator.Show();
    }
}
