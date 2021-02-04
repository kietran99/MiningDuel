using MD.Diggable.Projectile;
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
        private GameObject targetTrackerPrefab = null;

        [SerializeField]
        private ThrowChargeIndicator throwChargeIndicator = null;

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
            StartCoroutine(HandleGameplaySceneLoaded());
            handFreeState = new HandFreeState();
            freeThrowState = new FreeThrowState();
            currentState = handFreeState;
            chargeTimeAsWaitForSeconds = new WaitForSecondsRealtime(chargeTime);
            EventSystems.EventManager.Instance.StartListening<MD.UI.ThrowInvokeData>(HandleThrowInvokeData);        
        }

        public override void OnStopAuthority()
        {
            if (!isLocalPlayer) return;

            EventSystems.EventManager.Instance.StopListening<MD.UI.ThrowInvokeData>(HandleThrowInvokeData);
        }

        private System.Collections.IEnumerator HandleGameplaySceneLoaded()
        {
            var gameplaySceneLoaded = false;
            var networkManager = NetworkManager.singleton as NetworkManagerLobby;

            while (!gameplaySceneLoaded)
            {
                yield return null;

                if (!UnityEngine.SceneManagement.SceneManager.GetActiveScene().path.Equals(networkManager.GameplayScene)) continue;

                targetTracker = Instantiate(targetTrackerPrefab).GetComponent<TargetTracker>();
                trackState = new TrackState(transform, targetTracker);

                gameplaySceneLoaded = true;
            }
        }

        private void ShiftState(IState state)
        {
            currentState.OnStateExit();
            currentState = state;
            currentState.OnStateEnter();
        }

        private void HandleThrowInvokeData(MD.UI.ThrowInvokeData _) 
        {
            CmdRequestShowThrowChargeIndicator();
            StartCoroutine(ChargedThrow());
        }

        [Command]
        private void CmdRequestShowThrowChargeIndicator() => ShowThrowChargeIndicator();

        [ClientRpc]
        private void ShowThrowChargeIndicator() 
        {
            throwChargeIndicator.Show();
            Invoke(nameof(HideThrowChargeIndicator), chargeTime);
        }

        [Client]
        private void HideThrowChargeIndicator() => throwChargeIndicator.Hide();

        public void StartTracking(Transform targetTransform) 
        {
            ShiftState(trackState);
            targetTracker.StartTracking(targetTransform);
        }

        public virtual void SetHoldingProjectile(ProjectileLauncher proj) 
        {
            if (isLocalPlayer) ShiftState(freeThrowState);
            holdingProjectile = proj;
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