﻿using MD.Diggable.Projectile;
using UnityEngine;
using Mirror;
using MD.UI;

namespace MD.Character
{
    [RequireComponent(typeof(ThrowRaycast))]
    public class ThrowAction : NetworkBehaviour
    {
        #region STATES
        private interface IState
        {
            void OnStateEnter();
            void OnStateExit();
            Vector2 NormalizedThrowDirection { get; }
        }

        private class HandFreeState : IState
        {
            public void OnStateEnter(){}

            public void OnStateExit(){}

            public Vector2 NormalizedThrowDirection => Vector2.zero;
        }

        private class FreeThrowState : IState
        {
            private Vector2 lastJoystickDirection = Vector2.zero;

            public void OnStateEnter()
            {
                EventSystems.EventManager.Instance.StartListening<MD.UI.JoystickDragData>(UpdateJoystickLastDirection);
            }

            public void OnStateExit()
            {
                EventSystems.EventManager.Instance.StartListening<MD.UI.JoystickDragData>(UpdateJoystickLastDirection);
            }

            private void UpdateJoystickLastDirection(JoystickDragData joystickDragData)
            {
                if (joystickDragData.InputDirection.Equals(Vector2.zero)) return;

                lastJoystickDirection = joystickDragData.InputDirection;
            }

            public Vector2 NormalizedThrowDirection => lastJoystickDirection;
        }

        private class TrackState : IState
        {
            private Transform throwerTransform;
            private TargetTracker targetTracker;

            public TrackState(Transform throwerTransform, TargetTracker targetTracker) 
            {
                this.throwerTransform = throwerTransform;
                this.targetTracker = targetTracker;
            }

            public void OnStateEnter(){}

            public void OnStateExit(){}

            public Vector2 NormalizedThrowDirection => 
                (targetTracker.TargetPosition - new Vector2(throwerTransform.position.x, throwerTransform.position.y)).normalized;
        }
        #endregion

        #region SERIALIZED FIELDS
        [SerializeField]
        protected GameObject targetTrackerPrefab = null;

        [SerializeField]
        protected float basePower = 100f;

        [SerializeField]
        protected float chargeTime = .5f;
        #endregion

        #region FIELDS
        protected ProjectileLauncher holdingProjectile;
        protected WaitForSecondsRealtime chargeTimeAsWaitForSeconds;  
        protected TargetTracker targetTracker;   
        private IState currentState, handFreeState, freeThrowState, trackState;
        #endregion

        public override void OnStartLocalPlayer()
        {
            GetComponent<Player>().OnSceneLoaded += SetupStates;
            chargeTimeAsWaitForSeconds = new WaitForSecondsRealtime(chargeTime);
            EventSystems.EventManager.Instance.StartListening<MD.UI.ThrowInvokeData>(HandleThrowInvokeData);        
        }

        public override void OnStopAuthority()
        {
            if (!isLocalPlayer) return;

            GetComponent<Player>().OnSceneLoaded -= SetupStates;
            EventSystems.EventManager.Instance.StopListening<MD.UI.ThrowInvokeData>(HandleThrowInvokeData);
        }

        private void HandleThrowInvokeData(MD.UI.ThrowInvokeData _) => StartCoroutine(ChargedThrow());

        public void StartTracking(Transform targetTransform) 
        {
            ShiftState(trackState);
            targetTracker.StartTracking(targetTransform);
        }

        public virtual void SetHoldingProjectile(ProjectileLauncher proj) 
        {
            ShiftState(freeThrowState);
            holdingProjectile = proj;
        }

        private void SetupStates() 
        {
            targetTracker = Instantiate(targetTrackerPrefab).GetComponent<TargetTracker>();
            handFreeState = new HandFreeState();
            freeThrowState = new FreeThrowState();
            trackState = new TrackState(transform, targetTracker);
            currentState = handFreeState;
        }

        private void ShiftState(IState state)
        {
            currentState.OnStateExit();
            currentState = state;
            currentState.OnStateEnter();
        }

        private System.Collections.IEnumerator ChargedThrow()
        {
            yield return chargeTimeAsWaitForSeconds;

            CmdThrow(currentState.NormalizedThrowDirection.x, currentState.NormalizedThrowDirection.y);
            ShiftState(handFreeState);
            targetTracker.StopTracking();
        }

        [Command]
        protected void CmdThrow(float dirX, float dirY)
        {
            if (holdingProjectile == null) 
            {
                Debug.LogError("Not Holding Any Projectile");
                return;
            }
            
            holdingProjectile.Launch(basePower, dirX, dirY);
        }
    }
}