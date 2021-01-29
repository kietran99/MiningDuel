using EventSystems;
using Mirror;
using UnityEngine;

namespace MD.UI
{
    public struct ScanData : IEventData { public DiggableType[] diggableArea; }

    public class  SonarProxy : NetworkBehaviour
    {
        public override void OnStartAuthority()
        {
            ServiceLocator.Register(this);
        }

        [Command]
        public void CmdRequestScanArea(Vector2Int[] positions)
        {
            ServiceLocator
                .Resolve<Map.Core.IDiggableGenerator>()
                .Match(
                    unavailServiceErr => Debug.LogError(unavailServiceErr.Message),
                    digGen => TargetBroadcastScanData(digGen.GetDiggableArea(positions))
                );
        }

        [TargetRpc]
        private void TargetBroadcastScanData(DiggableType[] diggableArea)
        {
            EventManager.Instance.TriggerEvent(new ScanData() { diggableArea = diggableArea });
        }
    }
}