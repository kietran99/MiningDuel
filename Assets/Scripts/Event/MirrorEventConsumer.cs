using UnityEngine;

namespace EventSystems
{
    public class MirrorEventConsumer : EventConsumer
    {
        public new static MirrorEventConsumer Attach(GameObject listenerGO) => listenerGO.AddComponent<MirrorEventConsumer>();

        protected override void OnDestroy()
        {
            if (!GetComponent<Mirror.NetworkIdentity>().hasAuthority)
            {
                return;
            }
    
            base.OnDestroy();
        }
    }
}
