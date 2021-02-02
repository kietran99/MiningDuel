using MD.Diggable.Projectile;
using UnityEngine;
using Mirror;

namespace MD.Character
{
    [RequireComponent(typeof(ThrowRaycast))]
    public class ThrowAction : NetworkBehaviour
    {
        [SerializeField]
        protected GameObject targetTrackerPrefab = null;

        [SerializeField]
        protected float basePower = 100f;

        [SerializeField]
        protected float chargeTime = .5f;
        
        protected ProjectileLauncher holdingProjectile;
        protected WaitForSecondsRealtime chargeTimeAsWaitForSeconds;  
        protected TargetTracker targetTracker;   

        public override void OnStartLocalPlayer()
        {
            GetComponent<Player>().OnSceneLoaded += SpawnTargetTracker;
            chargeTimeAsWaitForSeconds = new WaitForSecondsRealtime(chargeTime);
            EventSystems.EventManager.Instance.StartListening<MD.UI.ThrowInvokeData>(HandleThrowInvokeData);        
        }

        public override void OnStopAuthority()
        {
            if (!isLocalPlayer) return;

            GetComponent<Player>().OnSceneLoaded -= SpawnTargetTracker;
            EventSystems.EventManager.Instance.StopListening<MD.UI.ThrowInvokeData>(HandleThrowInvokeData);
        }

        private void HandleThrowInvokeData(MD.UI.ThrowInvokeData _) => StartCoroutine(ChargedThrow());

        public void StartTracking(Transform targetTransform)
        {
            targetTracker.StartTracking(targetTransform);
        }

        public virtual void SetHoldingProjectile(ProjectileLauncher proj) => holdingProjectile = proj;

        private void SpawnTargetTracker() 
        {   
            targetTracker = Instantiate(targetTrackerPrefab).GetComponent<TargetTracker>();
        }

        private System.Collections.IEnumerator ChargedThrow()
        {
            yield return chargeTimeAsWaitForSeconds;

            var normalizedThrowDirection = 
                new Vector2(targetTracker.TargetPosition.x - transform.position.x, targetTracker.TargetPosition.y - transform.position.y)
                    .normalized;
            CmdThrow(normalizedThrowDirection.x, normalizedThrowDirection.y);
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