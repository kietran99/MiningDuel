using UnityEngine;
using Mirror;
using MD.Character.Animation;
using MD.Map.Core;
using System.Collections.Generic;
using EventSystems;

namespace MD.Character
{
    public class DigAction : NetworkBehaviour
    {
        [SerializeField]
        private int power = 1;

        public int Power => power;

        public override void OnStartAuthority()
        {
            StartListeningToEvents();
        }

        private void OnDestroy()
        {
            StopListeningToEvents();
        }

        protected virtual void StartListeningToEvents()
        {
            EventSystems.EventManager.Instance.StartListening<DigAnimEndData>(HandleDigAnimEnd);
        }

        protected virtual void StopListeningToEvents()
        {
            EventSystems.EventManager.Instance.StopListening<DigAnimEndData>(HandleDigAnimEnd);
        }

        protected void HandleDigAnimEnd(DigAnimEndData data) => CmdDig();

        [Command]
        public void CmdDig()
        {
            ServiceLocator
                .Resolve<IDiggableGenerator>()
                .Match(
                    unavailServiceErr => Debug.LogError(unavailServiceErr.Message),
                    RequestDig                   
                );       
        }

        [Server]
        private void RequestDig(IDiggableGenerator diggableGenerator)
        {
            diggableGenerator
                .DigAt(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), power)
                .Match(
                    invalidTileErr => Debug.LogError(invalidTileErr.Message),
                    TryBroadcastingDiggableEvent
                );
        }

        [Server]
        private void TryBroadcastingDiggableEvent(Functional.Type.Either<InvalidAccessError, ReducedData> maybeReducedData)
        {
            maybeReducedData
                .Match(                               
                    invalidAccessError => Debug.LogError(invalidAccessError.Message),
                    reducedData => 
                    {
                        TargetBroadcastDiggableDugEvent(reducedData);
                        if (reducedData.isEmpty) 
                        {
                            RpcTargetBroadcastDiggableDestroyEvent();
                        }
                    }
                );
        }

        [TargetRpc]    
        private void TargetBroadcastDiggableDugEvent(ReducedData reducedData)
        {
            DiggableEventBroadcast.TriggerDiggableDugEvent(netIdentity, reducedData);
        }

        [ClientRpc]
        private void RpcTargetBroadcastDiggableDestroyEvent()
        {
            DiggableEventBroadcast.TriggerDiggableDestroyEvent(
                Mathf.FloorToInt(transform.position.x), 
                Mathf.FloorToInt(transform.position.y)
            );
        }
    }
}