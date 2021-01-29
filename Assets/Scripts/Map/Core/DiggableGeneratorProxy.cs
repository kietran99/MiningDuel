using Mirror;
using UnityEngine;

namespace MD.Map.Core
{
    public class DiggableGeneratorProxy : NetworkBehaviour
    {
        public override void OnStartClient()
        {
            // ServiceLocator.Register(this);
        }

        [TargetRpc]
        public void TargetBroadcastDigProgressEvent(NetworkConnection target, int cur, int max)
        {
            // EventManager.Instance.TriggerEvent(new DigProgressData(cur, max));
        }

        [TargetRpc]
        public void TargetBroadcastGemDigEvent(NetworkConnection target)
        {
            
        } 
    }
}
