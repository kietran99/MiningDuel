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
            CmdUnsubscribeDiggableEvents();
        }

        // TargetRpc callbacks without NetworkConnection as an arg are invoked on every authoritative DigGenComm.
        // TargetRpc callbacks with NetworkConnection as an arg are invoked on the same DigGenComm on each client regardless of its authority.
        [Command]
        private void CmdSubscribeDiggableEvents()
        {
            ServiceLocator
                .Resolve<IDiggableGenerator>()
                .Match(
                    unavailServiceErr => Debug.LogError(unavailServiceErr.Message),
                    digGen => 
                    {
                        digGen.DigProgressEvent         += TargetHandleDigProgressEvent;
                        digGen.GemObtainEvent           += TargetHandleGemObtainEvent;
                        digGen.ProjectileObtainEvent    += TargetHandleProjectileObtainEvent;
                        digGen.DiggableDestroyEvent     += RpcHandleDiggableDestroyEvent;
                        digGen.DiggableSpawnEvent       += RpcHandleDiggableSpawnEvent;
                    }
                );
        }

        [Command]
        private void CmdUnsubscribeDiggableEvents()
        {
            ServiceLocator
                .Resolve<IDiggableGenerator>()
                .Match(
                    unavailServiceErr => Debug.LogError(unavailServiceErr.Message),
                    digGen => 
                    {
                        digGen.DigProgressEvent         -= TargetHandleDigProgressEvent;
                        digGen.GemObtainEvent           -= TargetHandleGemObtainEvent;
                        digGen.ProjectileObtainEvent    -= TargetHandleProjectileObtainEvent;
                        digGen.DiggableDestroyEvent     -= RpcHandleDiggableDestroyEvent;
                        digGen.DiggableSpawnEvent       -= RpcHandleDiggableSpawnEvent;
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
            if (!hasAuthority) return;

            EventManager.Instance.TriggerEvent(gemObtainData);
        }

        [TargetRpc]
        private void TargetHandleProjectileObtainEvent(NetworkConnection target, ProjectileObtainData projectileObtainData)
        {
            if (!hasAuthority) return;

            EventManager.Instance.TriggerEvent(projectileObtainData);
        }

        [ClientRpc]
        private void RpcHandleDiggableDestroyEvent(Diggable.DiggableRemoveData diggableRemoveData)
        {
            EventManager.Instance.TriggerEvent(diggableRemoveData);
        }

        [ClientRpc]
        private void RpcHandleDiggableSpawnEvent(Diggable.DiggableSpawnData diggableSpawnData)
        {
            EventManager.Instance.TriggerEvent(diggableSpawnData);
        }

        [Command]
        public void CmdRequestScanArea(Vector2Int[] positions)
        {
            ServiceLocator
                .Resolve<IDiggableGenerator>()
                .Match(
                    unavailServiceErr => Debug.LogError(unavailServiceErr.Message),
                    digGen => TargetBroadcastScanData(digGen.GetDiggableArea(positions))
                );
        }

        [TargetRpc]
        private void TargetBroadcastScanData(DiggableType[] diggableArea)
        {
            // Debug.Log("");
            EventManager.Instance.TriggerEvent(new UI.ScanData(diggableArea));
        }
    }
}