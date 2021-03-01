﻿using UnityEngine;
using Mirror;

namespace MD.Quirk
{
    ///<remarks>
    /// GameObjects that attached any inherited <c>BaseQuirk</c> are disabled on spawn by default
    ///</remarks>
    public abstract class BaseQuirk : NetworkBehaviour
    {
        [SerializeField]
        private QuirkData quirkData = null;

        public Sprite ObtainSprite => quirkData.ObtainSprite;

        public override void OnStartClient()
        {
            gameObject.SetActive(false);
        }

        public virtual void SingleActivate(NetworkIdentity user) {}

        public virtual void SyncActivate(NetworkIdentity user) => gameObject.SetActive(true);
    }
}
