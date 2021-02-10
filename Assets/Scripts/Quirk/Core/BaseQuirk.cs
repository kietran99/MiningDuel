using UnityEngine;
using Mirror;

namespace MD.Quirk
{
    public abstract class BaseQuirk : NetworkBehaviour
    {
        [SerializeField]
        private Sprite obtainSprite = null;

        public Sprite ObtainSprite => obtainSprite;

        [ClientRpc]
        public void RpcActivate()
        {
            Activate();
        }

        public abstract void Activate();
    }
}
