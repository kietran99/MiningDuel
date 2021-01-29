using EventSystems;
using MD.Diggable.Gem;
using MD.Diggable.Projectile;
using Mirror;
using UnityEngine;

namespace MD.Diggable.Core
{
    public class DiggableGeneratorCommunicator : NetworkBehaviour
    {
        public override void OnStartAuthority()
        {
            ServiceLocator.Register(this);
            CmdSubscribeDiggableEvents();
        }

        public override void OnStopAuthority()
        {
            // CmdUnsubscribeDiggableEvents();
        }

        [Command]
        private void CmdSubscribeDiggableEvents()
        {
            ServiceLocator
                .Resolve<Map.Core.IDiggableGenerator>()
                .Match(
                    unavailServiceErr => Debug.LogError(unavailServiceErr.Message),
                    digGen => 
                    {
                        digGen.DigProgressEvent         += TargetHandleDigProgressEvent;
                        digGen.GemObtainEvent           += TargetHandleGemObtainEvent;
                        // digGen.ProjectileObtainEvent    += TargetHandleProjectileObtainEvent;
                        digGen.DiggableDestroyEvent     += RpcHandleDiggableDestroyEvent;
                    }
                );
        }

        [Command]
        private void CmdUnsubscribeDiggableEvents()
        {
            ServiceLocator
                .Resolve<Map.Core.IDiggableGenerator>()
                .Match(
                    unavailServiceErr => Debug.LogError(unavailServiceErr.Message),
                    digGen => 
                    {
                        digGen.DigProgressEvent         -= TargetHandleDigProgressEvent;
                        digGen.GemObtainEvent           -= TargetHandleGemObtainEvent;
                        digGen.ProjectileObtainEvent    -= TargetHandleProjectileObtainEvent;
                        digGen.DiggableDestroyEvent     -= RpcHandleDiggableDestroyEvent;
                    }
                );
        }

        [TargetRpc]
        private void TargetHandleDigProgressEvent(NetworkConnection target, Diggable.Gem.DigProgressData digProgressData)
        {
            if (!hasAuthority) return;

            EventManager.Instance.TriggerEvent(digProgressData);
        }

        [TargetRpc]
        private void TargetHandleGemObtainEvent(NetworkConnection target, GemObtainData gemObtainData)
        {
            // Debug.Log(gemObtainData.diggerID);
            if (!hasAuthority) return;

            EventManager.Instance.TriggerEvent(gemObtainData);
        }

        [TargetRpc]
        private void TargetHandleProjectileObtainEvent(NetworkConnection target, ProjectileObtainData projectileObtainData)
        {
            EventManager.Instance.TriggerEvent(projectileObtainData);
        }

        [ClientRpc]
        private void RpcHandleDiggableDestroyEvent(Diggable.DiggableRemoveData diggableRemoveData)
        {
            EventManager.Instance.TriggerEvent(diggableRemoveData);
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
            EventManager.Instance.TriggerEvent(new UI.ScanData() { diggableArea = diggableArea });
        }
    }
}