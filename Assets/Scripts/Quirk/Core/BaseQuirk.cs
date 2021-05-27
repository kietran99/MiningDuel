using UnityEngine;
using Mirror;

namespace MD.Quirk
{
    ///<remarks>
    /// GameObjects that attached any inherited <c>BaseQuirk</c> are disabled on spawn by default
    ///</remarks>
    [System.Serializable]
    public abstract class BaseQuirk : NetworkBehaviour
    {
        [SerializeField]
        private QuirkData quirkData = null;

        public Sprite ObtainSprite => quirkData.ObtainSprite;

        public string Description => quirkData.Description;

        public override void OnStartClient()
        {
            // gameObject.SetActive(false);
        }

        public virtual void Activate(NetworkIdentity user)
        {
            TargetActivate(user);
            RpcActivate(user);
        }

        [TargetRpc]
        private void TargetActivate(NetworkIdentity user) => SingleActivate(user);
        
        [ClientRpc]
        private void RpcActivate(NetworkIdentity user) => SyncActivate(user);


        public virtual void SingleActivate(NetworkIdentity user) {}

        // public virtual void SyncActivate(NetworkIdentity user) => gameObject.SetActive(true);
        public virtual void SyncActivate(NetworkIdentity user) {}
    }
}
