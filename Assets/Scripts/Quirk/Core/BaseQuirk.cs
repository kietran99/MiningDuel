using UnityEngine;
using Mirror;

namespace MD.Quirk
{
    [System.Serializable]
    public abstract class BaseQuirk : NetworkBehaviour
    {
        [SerializeField]
        private QuirkData quirkData = null;

        public Sprite ObtainSprite => quirkData.ObtainSprite;

        public string GetDescription() => quirkData != null ? quirkData.Description : string.Empty;

        public string GetName() => quirkData != null ? quirkData.Name : string.Empty;

        [Server]
        public virtual void ServerActivate(NetworkIdentity user)
        {
            TargetActivate(user);
            RpcActivate(user);
        }

        [TargetRpc]
        private void TargetActivate(NetworkIdentity user) => SingleActivate(user);
        
        [ClientRpc]
        private void RpcActivate(NetworkIdentity user) => SyncActivate(user);


        public virtual void SingleActivate(NetworkIdentity user) {}

        public virtual void SyncActivate(NetworkIdentity user) {}
    }
}
