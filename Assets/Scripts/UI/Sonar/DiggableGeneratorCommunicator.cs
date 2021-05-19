using EventSystems;
using MD.Diggable.Gem;
using MD.Diggable.Projectile;
using Mirror;
using UnityEngine;


namespace MD.Diggable.Core
{
    public class DiggableGeneratorCommunicator : NetworkBehaviour
    {

        public override void OnStartServer()
        {
            SubscribeDiggableEvents();
        }
        [ServerCallback]
        private void OnDisable()
        {
            UnsubscribeDiggableEvents();
        }

        // TargetRpc callbacks without NetworkConnection as an arg are invoked on every authoritative DigGenComm.
        // TargetRpc callbacks with NetworkConnection as an arg are invoked on the same DigGenComm on each client regardless of its authority.
        [Server]
        private void SubscribeDiggableEvents()
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

        [Server]
        private void UnsubscribeDiggableEvents()
        {
            ServiceLocator
                .Resolve<IDiggableGenerator>()
                .Match(
                    unavailServiceErr => {},
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
            EventManager.Instance.TriggerEvent(digProgressData);
        }

        [TargetRpc]
        private void TargetHandleGemObtainEvent(NetworkConnection target, GemObtainData gemObtainData)
        {
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

        [ClientRpc]
        private void RpcHandleDiggableSpawnEvent(Diggable.DiggableSpawnData diggableSpawnData)
        {
            EventManager.Instance.TriggerEvent(diggableSpawnData);
        }
    }
}